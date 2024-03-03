using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : GameBehaviour
{
    public WeaponClass weapon;
    EnemyStats enemyClass;
    bool hasAttacked;
    float attackDelay;

    private void Start()
    {
        enemyClass = transform.root.gameObject.GetComponent<EnemyStats>();
        attackDelay = enemyClass.stats.attackDelay;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!hasAttacked)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                hasAttacked = true;

                print("hit player");
                Health health;
                if (health = collision.gameObject.GetComponent<Health>())
                {
                    health.GetHit(enemyClass.stats.attack + weapon.attack, transform.root.gameObject);
                }

                ExecuteAfterSeconds(attackDelay,()=> hasAttacked = false);

            }
        }
        
    }
}
