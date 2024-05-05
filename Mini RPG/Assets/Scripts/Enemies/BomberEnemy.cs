using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;


public class BomberEnemy : GameBehaviour
{
    EnemyStats enemyStats;
    [SerializeField] EnemyClass bomberStats;
    [SerializeField] float explosionRange;
    [SerializeField] float burnLength;
    [SerializeField] float burnTickDmg;

    ParticleSystem explosionPS;

    [SerializeField] Health healthScript;

    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent<GameObject> OnAttack;

    public enum CurrentState { Idle, Chase, Dead, Explode }

    public CurrentState currentState;

    bool hasAttacked;
    [SerializeField]

    bool hasDestination;
    bool hasExploded = false;

    float attackDelay = 1;
    float passedTime = 1;


    GameObject targetPlayer;

    Animator anim;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        bomberStats = enemyStats.stats;

        explosionPS = GetComponentInChildren<ParticleSystem>();

        attackDelay = bomberStats.attackDelay;

        anim = GetComponentInChildren<Animator>();

        agent = GetComponent<NavMeshAgent>();
        healthScript.InitilizeHealth(bomberStats.health, bomberStats.defence);

        agent.speed = bomberStats.movementSpeed;

        InvokeRepeating("FindClosetsPlayer", 0, 10);

        healthScript.ApplyParalysisEvent.AddListener(ApplyParalysis);
        healthScript.ApplySlownessEvent.AddListener(ApplySlowness);
        healthScript.ApplySlownessEvent.AddListener(ApplyBurn);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState != CurrentState.Dead)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);


            if (targetPlayer == null) FindClosetsPlayer();

            if (targetPlayer != null)
            {
                var distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
                if (distance <= bomberStats.visionRange && distance > bomberStats.attackRange)
                    currentState = CurrentState.Chase;
                else if (distance < bomberStats.attackRange)
                    StartCoroutine(Explode());
                else currentState = CurrentState.Idle;
            }
        }


        switch (currentState)
        {
            case CurrentState.Idle:

                break;

            case CurrentState.Chase:


                agent.SetDestination(targetPlayer.transform.position);

                break;

        }
    }


    void FindClosetsPlayer()
    {
        if(currentState == CurrentState.Idle)
        {
            GameObject closestPlayer = null;
            float distance;
            float mindistance = -1;
            foreach (var player in _GM.playerGameObjList)
            {
                distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
                if (distance < mindistance || mindistance == -1)
                {
                    closestPlayer = player;
                    mindistance = distance;
                }
            }

            targetPlayer = closestPlayer;
        }


    }

    public void Die()
    {
        currentState = CurrentState.Dead;

        anim.SetTrigger("Die");

        //anim.enabled = false;

        ExecuteAfterSeconds(1.5f, () => transform.DOScale(0, 0.5f));
        ExecuteAfterSeconds(2f, () => Destroy(gameObject));
    }


    IEnumerator Explode()
    {

        if (!hasExploded)
        {
            hasExploded = true;

            agent.SetDestination(targetPlayer.transform.position);
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);

            //get everything in explosion range with health script
            var healthScriptHoldersInRange = Physics.OverlapSphere(gameObject.transform.position, explosionRange);


            List<Health> thingsHit = new List<Health>();

            foreach (var holder in healthScriptHoldersInRange)
            {
                if (holder.TryGetComponent<Health>(out healthScript))
                {
                    if (holder.transform.root.gameObject != gameObject) thingsHit.Add(healthScript);

                }
            }


            //damage all and apply burn to all
            foreach (var thing in thingsHit)
            {
                print(thing.name + " got hit");
                thing.GetHit(bomberStats.attack, transform.gameObject);
                thing.ApplyBurn(burnLength, burnTickDmg);
            }

            Destroy(gameObject);
        }



    }

    #region Status Effects

    public void ApplyBurn(float duration, float tickDmg)
    {
        StartCoroutine(Burn(duration, tickDmg));
        ExecuteAfterSeconds(duration, () => StopCoroutine(Burn(duration, tickDmg)));
    }

    IEnumerator Burn(float duration, float tickDmg)
    {
        healthScript.currentHealth -= Mathf.RoundToInt( tickDmg);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Burn(duration, tickDmg));
    }

    void ApplyParalysis(float duration)
    {
        agent.isStopped = true;
        print("Paralysed");
        //paralysis animation plays here

        ExecuteAfterSeconds(duration, () => agent.isStopped = false);
    }

    void ApplySlowness(float duration, float strength)
    {
        float speedBeforeSlow = bomberStats.movementSpeed;

        agent.speed = bomberStats.movementSpeed * strength;

        ExecuteAfterSeconds(duration, () => agent.speed = speedBeforeSlow);

    }

    #endregion
}
