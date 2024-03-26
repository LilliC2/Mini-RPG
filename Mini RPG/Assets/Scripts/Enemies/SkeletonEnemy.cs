using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class SkeletonEnemy : GameBehaviour
{
    EnemyStats enemyStats;
    [SerializeField] EnemyClass skeletonStats;
    [SerializeField] Health healthScript;

    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent<GameObject> OnAttack;

    public enum CurrentState { Patrol, Chase ,Attack, Orbit, Dead }

    public CurrentState currentState;

    bool hasAttacked;
    [SerializeField]

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

            var distance = Vector3.Distance(transform.position, targetPlayer.transform.position);


            if (distance <= skeletonStats.visionRange && currentState != CurrentState.Orbit && currentState != CurrentState.Attack)
                currentState = CurrentState.Chase;
            else if( distance < 5 && currentState != CurrentState.Attack)
                currentState = CurrentState.Orbit;
            //else if (Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position) <= skeletonStats.attackRange)
            //    currentState = CurrentState.Attack;
            else if(currentState != CurrentState.Attack) currentState = CurrentState.Patrol;

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


        //speed
        if(currentState == CurrentState.Orbit || currentState == CurrentState.Patrol) agent.speed = 1;
        else agent.speed = skeletonStats.movementSpeed;


        switch (currentState)
        {
            case CurrentState.Patrol:


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

                hasDestination = false;
                agent.speed = skeletonStats.movementSpeed;
                agent.SetDestination(targetPlayer.transform.position);

                if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= 5) currentState = CurrentState.Orbit;

                break;
            case CurrentState.Attack:

                //go forwards

                agent.SetDestination(targetPlayer.transform.position);

                if (Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position) <= skeletonStats.attackRange)
                {
                    //stop moving
                    agent.SetDestination(transform.position);

                    if (passedTime >= attackDelay)
                    {
                        gameObject.transform.LookAt(targetPlayer.transform.position);
                        print("Attack");
                        passedTime = 0;
                        OnAttack?.Invoke(targetPlayer);

                        hasDestination = false;
                        currentState = CurrentState.Orbit;
                    }
                }




                break;

            case CurrentState.Orbit:

                if(targetPlayer == null) FindClosetsPlayer();

                gameObject.transform.LookAt(targetPlayer.transform.position);

                var distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
                print("Disatnce is " + distance);
                //reach radius of player



                if (distance < 5 && distance > 1)
                {
                    print("IN RANGE");
                    if (!hasDestination)
                    {
                        print("Has destination");
                        hasDestination = true;
                        currentDestination = OrbitAroundPlayer();
                        agent.SetDestination(currentDestination);

                    }
                }
                else if(distance > 5) //go towards player
                {
                    currentState = CurrentState.Chase;

                }
                else if (distance < 2) //back away from player
                {
                    print("backing up");
                    agent.SetDestination(transform.position - targetPlayer.transform.position * 3);
                }


                if (agent.velocity.magnitude == 0) hasDestination = false;

                //chance to attack after x seconds



            break;

            default:
                break;
        }

    }


    Vector3 OrbitAroundPlayer()
    {
        bool foundValidPoint = false;
        Vector3 orbitPoint = new();

        do
        {
            orbitPoint = transform.position + Random.insideUnitSphere * 3;

            if (Vector3.Distance(orbitPoint, targetPlayer.transform.position) > 3 && Vector3.Distance(orbitPoint,gameObject.transform.position) > 1f) 
                foundValidPoint = true;


        } while (!foundValidPoint);



        return orbitPoint;
    }

    void ChanceToAttack()
    {
        int r = Random.Range(0, 4);
        if(r == 1) currentState = CurrentState.Attack;
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
