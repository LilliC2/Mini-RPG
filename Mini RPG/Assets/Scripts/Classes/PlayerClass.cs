using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerClass
{
    public int attack;
    public int health;
    public float movSpeed;
    public int defence;
    public int mana;
    public float atkSpd;

    public enum Alignments { Lawful_Good, Lawful_Neutral, Lawful_Evil, Neutral_Good, True_Neutral, Neutral_Evil, Chaotic_Good, Chaotic_Neutral, Chatotic_Evil }
    public Alignments alignment;

    public enum CombatStyles { Priest, Warrior, Mage, Bard, Cleric, Monk, Ranger}
    public CombatStyles combatStyle;

    public AbilityCardClass[] abilityDeck;

}
