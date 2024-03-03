using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityCardClass
{
    public string name;
    public string description;

    public float cooldown;
    public int manaCost;

    public enum Rarity { Common, Uncommon, Rare, Legenardy}
    public Rarity rarity;
}
