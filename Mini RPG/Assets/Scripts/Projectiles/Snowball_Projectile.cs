using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Snowball_Projectile : GameBehaviour
{
    public int dmg;
    [SerializeField]
    float slownessDuration;
    [SerializeField]
    float slownessStrength;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ExecuteAfterSeconds(5, () => DestroyProj()); //if doesnt hit anything
    }

    public void AddProjForce(Vector3 direction)
    {
        rb.AddForce(direction * 50, ForceMode.Impulse);

    }

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyHealth = collision.gameObject.GetComponent<Health>();

            enemyHealth.ApplySlowness(slownessDuration, slownessStrength);
            enemyHealth.GetHit(dmg, gameObject);


        }
        DestroyProj();

    }

    void DestroyProj()
    {
        //destroy animation here
        transform.DOScale(Vector3.zero, 1f);
        Destroy(gameObject);
    }
}