using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDummy : MonoBehaviour
{
    [SerializeField]
    int health;
    [SerializeField]
    int defence;


    public UnityEvent OnAttack;
    [SerializeField]
    Health healthScript;

    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthScript.InitilizeHealth(health, defence);
    }

    // Update is called once per frame
    void Update()
    {
    }


}
