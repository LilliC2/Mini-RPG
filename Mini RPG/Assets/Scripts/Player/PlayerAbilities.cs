using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAbilities : GameBehaviour
{
    PlayerController playerControllerScript;
    bool calledAbility;
    GameObject tempRangeIndicator;

    bool ability0cooldown = false;
    bool ability1cooldown = false;
    bool ability2cooldown = false;

    [SerializeField]
    GameObject fireball_projectile;
    [SerializeField]
    GameObject snowball_projectile;
    [SerializeField]
    GameObject lightningBolt_projectile;

    private void Awake()
    {
        ability0cooldown = false;
        ability1cooldown = false;
        ability2cooldown = false;

        playerControllerScript = GetComponent<PlayerController>();
        tempRangeIndicator = GameObject.Find("TempRangeIndicator");
        StartCoroutine(RegainMana(1));
    }
    // Update is called once per frame
    
    IEnumerator RegainMana(int manaRegain)
    {
        if(playerControllerScript.playerInfo.currentMana < playerControllerScript.playerInfo.maxMana)
        {
            _UI.combatUI.UpdatePlayerHealthMana(playerControllerScript.playerNum, playerControllerScript.healthScript.currentHealth,
                playerControllerScript.healthScript.maxHealth, playerControllerScript.playerInfo.currentMana, 
                playerControllerScript.playerInfo.maxMana);

            playerControllerScript.playerInfo.currentMana += manaRegain;
            if (playerControllerScript.playerInfo.currentMana > playerControllerScript.playerInfo.maxMana) playerControllerScript.playerInfo.currentMana = playerControllerScript.playerInfo.maxMana;
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(RegainMana(1));
    }

    bool CheckIfOnCooldown(int abilityNum)
    {
        //print("Checking if ability " + abilityNum + " is on cooldown");
        bool isOnCooldown = true;

        switch (abilityNum) 
        {
            case 0:

                //check if ability is on cooldown
                if(!ability0cooldown)
                    isOnCooldown = false;

                break; 
        
            case 1:

                //check if ability is on cooldown
                if(!ability1cooldown)
                    isOnCooldown = false;

                break; 
            case 2:

                //check if ability is on cooldown
                if(!ability2cooldown)
                    isOnCooldown = false;

                break; 
        
        }

        return isOnCooldown;
    }

    void BeginAbilityCooldown(int abilityNum, float cooldownTime)
    {
        switch (abilityNum)
        {
            case 0:

                //check if ability is on cooldown
                ability0cooldown = true;
                ExecuteAfterSeconds(cooldownTime,()=> ability0cooldown = false);

                break;

            case 1:

                //check if ability is on cooldown
                ability1cooldown = true;
                ExecuteAfterSeconds(cooldownTime, () => ability1cooldown = false);
                break;
            case 2:

                ability2cooldown = true;
                ExecuteAfterSeconds(cooldownTime, () => ability2cooldown = false);

                break;

        }
    }
    public void CallAbility(AbilityCardClass ability, int abilityNumber)
    {
        if (!CheckIfOnCooldown(abilityNumber) && ability != null)
        {
            if (!calledAbility)
            {
                print("Try Call Ability " + ability.name );
                calledAbility = true;
                if (playerControllerScript.playerInfo.currentMana >= ability.manaCost)
                {
                    playerControllerScript.playerInfo.currentMana -= ability.manaCost;
                    switch (ability.name)
                    {
                        case "Dash":
                            Dash(ability);
                            break;

                        case "Shield":
                            Shield(ability);
                            break;
                        case "Swing":
                            SwiningCharge(ability);
                            break;
                        case "Fireball":
                            Fireball(ability);
                            break;
                        case "Lighting Bolt":
                            LightingBolt(ability);
                            break;
                        case "Snowball":
                            Snowball(ability);
                            break;
                        case "Inspire":
                            StartCoroutine(Inspire(ability));
                            break;
                        case "Bless":
                            StartCoroutine(Bless(ability));
                            break;

                    }

                    //put ability on cooldown
                    BeginAbilityCooldown(abilityNumber, ability.cooldown);
                }

                ExecuteAfterSeconds(1, () => calledAbility = false);
            }
        }
        else print("ON COOLDOWN");



    }


    /// <summary>
    /// Used by: All
    /// Increase movement speed for a period of time
    /// </summary>
    void Dash(AbilityCardClass ability)
    {
        print("Dash");
        //Increase speed by 25% for 5s for a mana cost of 3
        var playerSpeed = playerControllerScript.playerInfo.movSpeed; //speed before increase
        var speedIncrease = playerSpeed * ability.boostValue;
        playerControllerScript.playerInfo.movSpeed += speedIncrease;

        ExecuteAfterSeconds(ability.duration, () => playerControllerScript.playerInfo.movSpeed -= speedIncrease);



    }

    /// <summary>
    /// Used by: All
    /// Increase defence for a period of time
    /// </summary>
    /// <param name="ability"></param>
    void Shield(AbilityCardClass ability)
    {
        //increase defense
        print("Shield");
        //Increase speed by 25% for 5s for a mana cost of 3
        var playerDefence = playerControllerScript.playerInfo.defence; //speed before increase
        int defenceIncrease = Mathf.RoundToInt(playerDefence * ability.boostValue);
        print("Shield increase " + defenceIncrease);    
        playerControllerScript.playerInfo.defence += defenceIncrease;

        ExecuteAfterSeconds(ability.duration, () => playerControllerScript.playerInfo.defence -= defenceIncrease);

    }

    /// <summary>
    /// Used by: Bard, Warrior
    /// Allies within range gain a temporary attack and defence boost
    /// </summary>
    /// <param name="ability"></param>
    IEnumerator Inspire(AbilityCardClass ability)
    {
        tempRangeIndicator.transform.localScale = new Vector3(ability.range, 0.03f, ability.range);

        print("Inspire");
        //get people in range
        var collidersInRange = Physics.OverlapSphere(playerControllerScript.gameObject.transform.position, ability.range, _GM.playerMask);
        List<PlayerController> playerScripts = new List<PlayerController>();
        List<int> playerAttackPrev = new List<int>();
        List<int> playerDefencePrev = new List<int>();
        foreach (var collider in collidersInRange)
        {
            var playerScript = collider.gameObject.GetComponent<PlayerController>();
            playerScripts.Add(playerScript);
            print(collider.name);
            var playerDefence = playerScript.playerInfo.defence; //speed before increase
            int defenceIncrease = Mathf.RoundToInt(playerDefence * ability.boostValue);
            playerDefencePrev.Add(defenceIncrease);

            print("Defence increase " + defenceIncrease);
            playerScript.playerInfo.defence += defenceIncrease;
            
            var playerAttack = playerScript.playerInfo.attack; //speed before increase
            int attackincrease = Mathf.RoundToInt(playerAttack * ability.boostValue);
            playerAttackPrev.Add(attackincrease);

            print("Attack increase " + attackincrease);
            playerScript.playerInfo.attack += attackincrease;
        }

        yield return new WaitForSeconds(ability.duration);

        for (int i = 0; i < playerScripts.Count; i++)
        {
            playerScripts[i].playerInfo.defence -= playerDefencePrev[i];
            playerScripts[i].playerInfo.atkSpd -= playerAttackPrev[i];
        }


    }
    IEnumerator Bless(AbilityCardClass ability)
    {
        tempRangeIndicator.transform.localScale = new Vector3(ability.range, 0.03f, ability.range);

        print("Inspire");
        //get people in range
        var collidersInRange = Physics.OverlapSphere(playerControllerScript.gameObject.transform.position, ability.range, _GM.playerMask);
        List<PlayerController> playerScripts = new List<PlayerController>();
        List<int> playerDefencePrev = new List<int>();
        foreach (var collider in collidersInRange)
        {
            var playerScript = collider.gameObject.GetComponent<PlayerController>();
            playerScripts.Add(playerScript);
            print(collider.name);
            var playerDefence = playerScript.playerInfo.defence; //speed before increase
            int defenceIncrease = Mathf.RoundToInt(playerDefence * ability.boostValue);
            playerDefencePrev.Add(defenceIncrease);

            print("Defence increase " + defenceIncrease);
            playerScript.playerInfo.defence += defenceIncrease;

        }

        yield return new WaitForSeconds(ability.duration);

        for (int i = 0; i < playerScripts.Count; i++)
        {
            playerScripts[i].playerInfo.defence -= playerDefencePrev[i];
        }


    }

    void Fireball(AbilityCardClass ability)
    {
        var proj = Instantiate(fireball_projectile, playerControllerScript.transform.position, playerControllerScript.transform.rotation);
        proj.GetComponent<Fireball_Projectile>().dmg = playerControllerScript.playerInfo.attack;
        proj.GetComponent<Fireball_Projectile>().AddProjForce(playerControllerScript.transform.forward);
     
    }
    
    void Snowball(AbilityCardClass ability)
    {
        var proj = Instantiate(snowball_projectile, playerControllerScript.transform.position, playerControllerScript.transform.rotation);
        proj.GetComponent<Snowball_Projectile>().dmg = playerControllerScript.playerInfo.attack;
        proj.GetComponent<Snowball_Projectile>().AddProjForce(playerControllerScript.transform.forward);
     
    }
    
    void LightingBolt(AbilityCardClass ability)
    {
        var proj = Instantiate(lightningBolt_projectile, playerControllerScript.transform.position, playerControllerScript.transform.rotation);
        proj.GetComponent<LightingBolt_Projectile>().dmg = playerControllerScript.playerInfo.attack;
        proj.GetComponent<LightingBolt_Projectile>().AddProjForce(playerControllerScript.transform.forward);
     
    }



    void SwiningCharge(AbilityCardClass ability)
    {

        print("Swinging charge");
        playerControllerScript.controller.enabled = false;

        playerControllerScript.rb.AddForce(playerControllerScript.transform.forward * 30, ForceMode.Impulse);

        ExecuteAfterSeconds(ability.duration, () => playerControllerScript.controller.enabled = true);


    }

    void LetEmRip(AbilityCardClass ability)
    {
        print("Swinging charge");

        //temp turning, animation will do it 

        playerControllerScript.controller.enabled = false;

        playerControllerScript.rb.AddForce(playerControllerScript.transform.forward * 20, ForceMode.Impulse);

        ExecuteAfterSeconds(ability.duration, () => playerControllerScript.controller.enabled = true);
    }
}
