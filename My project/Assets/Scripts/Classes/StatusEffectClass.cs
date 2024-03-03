using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectClass
{
    public string name;

    [Header("If effect reduces something by percentage")]
    public float effectStrength;

    [Header("If effect does tick damage")]
    public float tickDamage;

}
