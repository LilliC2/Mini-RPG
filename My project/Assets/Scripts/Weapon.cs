using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : GameBehaviour
{
    public WeaponClass weapon;
    PlayerController player;

    private void Start()
    {
        player = transform.root.gameObject.GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Health health;
            if (health = collision.gameObject.GetComponent<Health>()) 
            {
                health.GetHit(player.playerInfo.attack + weapon.attack, transform.root.gameObject);
            }

        }
    }
}
