using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class ZombieEnemy : GameBehaviour
{

    EnemyStats enemyStats;
    [SerializeField] EnemyClass zombieStats;
    [SerializeField] Health healthScript;

    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent<GameObject> OnAttack;

    public enum CurrentState { Idle, Chase, Attack, Howl, Dead }

    public CurrentState currentState;

    bool hasAttacked;
    [SerializeField]

    bool hasDestination;
    bool hasHowled = false;

    float attackDelay = 1;
    float passedTime = 1;

    Vector3 currentDestination;

    GameObject targetPlayer;

    Animator anim;

    NavMeshAgent agent;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        zombieStats = enemyStats.stats;

        attackDelay = zombieStats.attackDelay;

        anim = GetComponentInChildren<Animator>();

        agent = GetComponent<NavMeshAgent>();
        healthScript.InitilizeHealth(zombieStats.health, zombieStats.defence);

        agent.speed = zombieStats.movementSpeed;

       InvokeRepeating("ChanceToHowl", 10, Random.Range(5, 20));

        healthScript.ApplyParalysisEvent.AddListener(ApplyParalysis);
        healthScript.ApplySlownessEvent.AddListener(ApplySlowness);
        healthScript.ApplyBurnEvent.AddListener(ApplyBurn);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != CurrentState.Dead)
        {
            if (passedTime < attackDelay) passedTime += Time.deltaTime;
            anim.SetFloat("Speed", agent.velocity.magnitude);
            if (targetPlayer == null) FindClosetsPlayer();

            if (targetPlayer != null)
            {
                var distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
                //if (distance <= zombieStats.attackRange) print("distance " + distance);

                if (distance <= zombieStats.visionRange && currentState != CurrentState.Howl && currentState != CurrentState.Attack && distance >= zombieStats.attackRange)
                    currentState = CurrentState.Chase;
                else if (distance <= zombieStats.attackRange && currentState != CurrentState.Howl)
                {
                    //print("enter attack state");
                    currentState = CurrentState.Attack;

                }
                else if(distance > zombieStats.visionRange) currentState = CurrentState.Idle;

            }
            else currentState = CurrentState.Idle;

        }

        switch (currentState)
        {
            case CurrentState.Idle:

                agent.isStopped = false;

                break;

            case CurrentState.Chase:

                agent.isStopped = false;

                agent.SetDestination(targetPlayer.transform.position);


            break;

            case CurrentState.Attack:

                //stop moving
                agent.SetDestination(transform.position);

                if (passedTime >= attackDelay)
                {
                    gameObject.transform.LookAt(targetPlayer.transform.position);
                    //print("Attack");
                    passedTime = 0;
                    OnAttack?.Invoke(targetPlayer);

                    hasDestination = false;
                    currentState = CurrentState.Chase;
                }

                break;

            case CurrentState.Howl:
                
                agent.isStopped = true;
                //print("called howl");

                if(!hasHowled)
                {  
                    hasHowled = true;
                    //animation here
                    print("HOWL");
                    SummonZombie();

                    ExecuteAfterSeconds(5, ()=>hasHowled = false);
                    currentState = CurrentState.Chase;
                }
                
                
                
            break;

        }

    }


    void ChanceToHowl()
    {
        //print("Chance to howl");
        if (currentState == CurrentState.Chase)
        {

            int r = Random.Range(0, 4);
            if (r == 1) currentState = CurrentState.Howl;

        }
    }

    void SummonZombie()
    {
        var summonVec3 = transform.position + Random.insideUnitSphere * 2;

        var zombie = Instantiate(transform.gameObject, summonVec3, Quaternion.identity);
        zombie.GetComponent<ZombieEnemy>().currentState = CurrentState.Idle;
    }

    /// <summary>
    /// Find closets player to gameobject out of active players
    /// </summary>
    void FindClosetsPlayer()
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

    #region Status Effects


    public void ApplyBurn(float duration, float tickDmg)
    {
        StartCoroutine(Burn(duration, tickDmg));
        ExecuteAfterSeconds(duration, () => StopCoroutine(Burn(duration, tickDmg)));
    }

    IEnumerator Burn(float duration, float tickDmg)
    {
        healthScript.currentHealth -= Mathf.RoundToInt(tickDmg);
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
        print("Apply slowness");
        float speedBeforeSlow = zombieStats.movementSpeed;

        agent.speed = zombieStats.movementSpeed * strength;

        ExecuteAfterSeconds(duration, () => agent.speed = speedBeforeSlow);

    }

    #endregion


    public void Attack()
    {
        if (!hasAttacked)
        {
            print("ATTACK ZOMVIE");
            hasAttacked = true;
            anim.SetTrigger("Attack");
            ExecuteAfterSeconds(attackDelay, () => hasAttacked = false);
        }
    }

    private Vector3 SearchWalkPoint()
    {

        return transform.position + Random.insideUnitSphere * zombieStats.visionRange;

    }

    public void Die()
    {
        currentState = CurrentState.Dead;

        anim.SetTrigger("Die");

        //anim.enabled = false;

        ExecuteAfterSeconds(1.5f, () => transform.DOScale(0, 0.5f));
        ExecuteAfterSeconds(2f, () => Destroy(gameObject));
    }

    /// <summary>
    /// To be used in OnHitWithRef(GameObject) to change target to player who hit it
    /// </summary>
    /// <param name="sender"></param>
    public void ChangeTarget(GameObject sender)
    {
        print("Change Target");
        targetPlayer = sender;
    }


}
