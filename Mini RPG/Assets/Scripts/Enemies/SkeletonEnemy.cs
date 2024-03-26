using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem.XR;

public class SkeletonEnemy : GameBehaviour
{
    EnemyStats enemyStats;
    [SerializeField] EnemyClass skeletonStats;
    [SerializeField] Health healthScript;

    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent<GameObject> OnAttack;

    public enum CurrentState { Patrol, Chase ,Attack, Dead }
    public CurrentState currentState;

    bool hasAttacked;
    bool hasDestination;

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
        skeletonStats = enemyStats.stats;

        attackDelay = skeletonStats.attackDelay;

        anim = GetComponentInChildren<Animator>();

        agent = GetComponent<NavMeshAgent>();
        healthScript.InitilizeHealth(skeletonStats.health, skeletonStats.defence);

        agent.speed = skeletonStats.movementSpeed;
        currentDestination = SearchWalkPoint();


        healthScript.ApplyParalysisEvent.AddListener(ApplyParalysis);
        healthScript.ApplySlownessEvent.AddListener(ApplySlowness);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState != CurrentState.Dead)
        {
            if (passedTime < attackDelay) passedTime += Time.deltaTime;

            anim.SetFloat("Speed", agent.velocity.magnitude);

            if (targetPlayer != null)
            {
                if (Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position) <= skeletonStats.visionRange && Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position) >= skeletonStats.attackRange)
                    currentState = CurrentState.Chase;
                else if (Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position) <= skeletonStats.attackRange)
                    currentState = CurrentState.Attack;
                else currentState = CurrentState.Patrol;

            }
            else currentState = CurrentState.Patrol;
        }



        switch (currentState)
        {
            case CurrentState.Patrol:

                agent.speed =1;

                if (!hasDestination)
                {
                    hasDestination = true;
                    ExecuteAfterSeconds(Random.Range(10f, 15), () => currentDestination = SearchWalkPoint());
                    agent.SetDestination(currentDestination);
                }

                if (Vector3.Distance(gameObject.transform.position, currentDestination) <= 1) hasDestination = false;

                InvokeRepeating("FindClosetsPlayer", 0, 5);

                break;
            case CurrentState.Chase:

                agent.speed = skeletonStats.movementSpeed;
                agent.SetDestination(targetPlayer.transform.position);

                break;
            case CurrentState.Attack:
                agent.speed = skeletonStats.movementSpeed;
                //stop moving
                agent.SetDestination(transform.position);

                if(passedTime >= attackDelay)
                {
                    gameObject.transform.LookAt(targetPlayer.transform.position);
                    print("Attack");
                    passedTime = 0;
                    OnAttack?.Invoke(targetPlayer);
                }

                break;
            default:
                break;
        }

    }

    public void Attack()
    {
        if(!hasAttacked)
        {
            hasAttacked = true;
            anim.SetTrigger("Attack");
            ExecuteAfterSeconds(attackDelay, ()=> hasAttacked = false);
        }
    }

    private Vector3 SearchWalkPoint()
    {

        return transform.position + Random.insideUnitSphere * skeletonStats.visionRange;

    }

    public void Die()
    {
        currentState = CurrentState.Dead;

        anim.SetTrigger("Die");
        //ExecuteAfterSeconds(2, ()=> transform.DOScale(0, 0.5f));
        //ExecuteAfterSeconds(2.5f, () => Destroy(gameObject));
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

    void ApplyParalysis(float duration)
    {
        agent.isStopped = true;
        print("Paralysed");
        //paralysis animation plays here

        ExecuteAfterSeconds(duration, () => agent.isStopped = false);
    }

    void ApplySlowness(float duration, float strength)
    {
        float speedBeforeSlow = skeletonStats.movementSpeed;

        agent.speed = skeletonStats.movementSpeed * strength;

        ExecuteAfterSeconds(duration, () => agent.speed = speedBeforeSlow);

    }
}
