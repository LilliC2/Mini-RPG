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
    }

    public void CallAbility(AbilityCardClass ability)
    {
        if(!calledAbility)
        {
            calledAbility = true;
            if (playerControllerScript.playerInfo.currentMana >= ability.manaCost)
            {
                playerControllerScript.playerInfo.currentMana -= ability.manaCost;
                switch (ability.name)
                {
                    case "Dash":
                        Dash(ability);
                        break;

                }
            }

            ExecuteAfterSeconds(1,()=> calledAbility = false);
        }


    }


    /// <summary>
    /// Used by: All
    /// 
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
