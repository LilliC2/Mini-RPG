using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyClass 
{
    public string name;
    public int attack;
    public int health;
    public int defence;
    public int magic;
    public float movementSpeed;

    public float attackRange;
    public float visionRange;

    public float attackDelay;
}
