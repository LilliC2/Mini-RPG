using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : GameBehaviour
{
    PlayerController playerControllerScript;
    bool calledAbility;

    private void Awake()
    {
        playerControllerScript = GetComponent<PlayerController>();
        StartCoroutine(RegainMana(1));
    }
    // Update is called once per frame
    
    IEnumerator RegainMana(int manaRegain)
    {
        if(playerControllerScript.playerInfo.currentMana < playerControllerScript.playerInfo.maxMana)
        {
            playerControllerScript.playerInfo.currentMana += manaRegain;
            if (playerControllerScript.playerInfo.currentMana > playerControllerScript.playerInfo.maxMana) playerControllerScript.playerInfo.currentMana = playerControllerScript.playerInfo.maxMana;
        }

        yield return new WaitForSeconds(2);
        StartCoroutine(RegainMana(1));
    }

    public void CallAbility(AbilityCardClass ability)
    {
        if(!calledAbility)
        {
            print("Call ability");
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

                }
            }

            ExecuteAfterSeconds(1,()=> calledAbility = false);
        }


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
        var speedIncrease = playerSpeed * 0.25f;
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
        int defenceIncrease = Mathf.RoundToInt(playerDefence * 0.20f);
        print("Shield increase " + defenceIncrease);    
        playerControllerScript.playerInfo.defence += defenceIncrease;

        ExecuteAfterSeconds(ability.duration, () => playerControllerScript.playerInfo.defence -= defenceIncrease);



    }


}
