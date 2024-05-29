using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityCardClass
{
    public string name;
    public string description;

    public float cooldown;
    public float dmg;
    public float duration;
    public float range;
    public float boostValue;
    public int manaCost;

    public Sprite sprite;

    public enum Rarity { Common, Uncommon, Rare, Legenardy}
    public Rarity rarity;
}
