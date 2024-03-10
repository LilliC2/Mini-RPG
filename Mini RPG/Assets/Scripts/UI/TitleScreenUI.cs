using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleScreenUI : GameBehaviour
{
    [SerializeField]
    AbiltyDecks abilityDecks;

    [SerializeField]
    CombatStyleStats combatStyleStats;

    int combatStyleCount_P1 = 0;
    int combatStyleCount_P2 = 0;
    int combatStyleCount_P3 = 0;
    int combatStyleCount_P4 = 0;
    int avatarCount_P1 = 0;
    int avatarCount_P2 = 0;
    int avatarCount_P3 = 0;
    int avatarCount_P4 = 0;

    [SerializeField]
    GameObject[] playerCustomisation;
    [SerializeField]
    GameObject[] generateAlignmentButtons;

    bool[] playersReady;

    [SerializeField]
    TMP_Text[] playerJoinText; //temp
    [SerializeField]
    TMP_Text[] generatedAlignmentText;
    [SerializeField]
    TMP_Text[] combatStyleText;

    private void Start()
    {
        playersReady = new bool[4];
    }

    public void PlayerJoinedGame(int playerNum)
    {
        playerJoinText[playerNum].text = "Player joined";
        playerCustomisation[playerNum].SetActive(true);
        playerJoinText[playerNum].gameObject.SetActive(false);


    }

    public void PlayerLeftGame(int playerNum)
    {
        playerJoinText[playerNum].text = "Hold A or Space to Join";
        playerCustomisation[playerNum].SetActive(false);
        playerJoinText[playerNum].gameObject.SetActive(true);


    }


    #region Character Customisation

    public void GenerateAlignment(int playerNum)
    {
        var alignment = (PlayerClass.Alignments)(Random.Range(0,9));
        print(alignment);
        _GM.playerGameObjList[playerNum].GetComponent<PlayerController>().playerInfo.alignment = alignment;

        generateAlignmentButtons[playerNum].SetActive(false);
        //remove underscore
        string alignmentStr = alignment.ToString();
        alignmentStr = alignmentStr.Replace("_", " ");
        generatedAlignmentText[playerNum].text = alignmentStr;
        generatedAlignmentText[playerNum].gameObject.SetActive(true);
    }

    public void CombatStyleNext(int playerNum)
    {
        switch(playerNum)
        {
            case 0:
                combatStyleCount_P1++;
                if (combatStyleCount_P1 > 6) combatStyleCount_P1 = 0;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P1)).ToString();

                break;
            case 1:
                combatStyleCount_P2++;
                if (combatStyleCount_P2 > 6) combatStyleCount_P2 = 0;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P2)).ToString();

                break;
            case 2:
                combatStyleCount_P3++;
                if (combatStyleCount_P3 > 6) combatStyleCount_P3 = 0;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P3)).ToString();

                break;
            case 3:
                combatStyleCount_P4++;
                if (combatStyleCount_P4 > 6) combatStyleCount_P4 = 0;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P4)).ToString();

                break;
        }


    }
    
    public void CombatStyleBack(int playerNum)
    {
        switch(playerNum)
        {
            case 0:
                combatStyleCount_P1--;
                if (combatStyleCount_P1 < 0) combatStyleCount_P1 = 6;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P1)).ToString();

                break;
            case 1:
                combatStyleCount_P2--;
                if (combatStyleCount_P2 < 0) combatStyleCount_P2 = 6;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P2)).ToString();

                break;
            case 2:
                combatStyleCount_P3--;
                if (combatStyleCount_P3 < 0) combatStyleCount_P3 = 6;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P3)).ToString();

                break;
            case 3:
                combatStyleCount_P4--;
                if (combatStyleCount_P4 < 0) combatStyleCount_P4 = 6;
                combatStyleText[playerNum].text = ((PlayerClass.CombatStyles)(combatStyleCount_P4)).ToString();

                break;
        }

    }

    public void ReadyUp(int playerNum)
    {
        switch (playerNum)
        {
            case 0:

                var P1 = _GM.playerGameObjList[playerNum].GetComponent<PlayerController>();
                P1.playerInfo.combatStyle = (PlayerClass.CombatStyles)(combatStyleCount_P1);
                playersReady[playerNum] = true;

                //set ability deck
                P1.playerInfo.abilityDeck = GetAbilityDeck(P1.playerInfo.combatStyle);
                SetBaseStats(playerNum); //set stats to base for that combat style

                //add avatar stuff here

                break;
            
            case 1:

                var P2 = _GM.playerGameObjList[playerNum].GetComponent<PlayerController>();
                P2.playerInfo.combatStyle = (PlayerClass.CombatStyles)(combatStyleCount_P2);
                playersReady[playerNum] = true;

                //set ability deck
                P2.playerInfo.abilityDeck = GetAbilityDeck(P2.playerInfo.combatStyle);
                SetBaseStats(playerNum); //set stats to base for that combat style

                //add avatar stuff here

                break;
            
            case 2:

                var P3 = _GM.playerGameObjList[playerNum].GetComponent<PlayerController>();
                P3.playerInfo.combatStyle = (PlayerClass.CombatStyles)(combatStyleCount_P3);
                playersReady[playerNum] = true;

                //set ability deck
                P3.playerInfo.abilityDeck = GetAbilityDeck(P3.playerInfo.combatStyle);
                SetBaseStats(playerNum); //set stats to base for that combat style

                //add avatar stuff here

                break;
            
            case 3:

                var P4 = _GM.playerGameObjList[playerNum].GetComponent<PlayerController>();
                P4.playerInfo.combatStyle = (PlayerClass.CombatStyles)(combatStyleCount_P4);
                playersReady[playerNum] = true;

                //set ability deck
                P4.playerInfo.abilityDeck = GetAbilityDeck(P4.playerInfo.combatStyle);
                SetBaseStats(playerNum); //set stats to base for that combat style

                //add avatar stuff here

                break;
        }

        if (CheckIfAllPlayersAreReady()) _UI.LoadScene("Map");
    }

    /// <summary>
    /// Assign base stats according to chosen combat style
    /// </summary>
    /// <param name="playerNum"></param>
    void SetBaseStats(int playerNum)
    {
        var playerInfo = _GM.playerGameObjList[playerNum].GetComponent<PlayerController>().playerInfo;

        switch (playerInfo.combatStyle)
        {
            case PlayerClass.CombatStyles.Priest:

                playerInfo.attack = combatStyleStats.priest_stats.attack;
                playerInfo.defence = combatStyleStats.priest_stats.defence;
                playerInfo.movSpeed = combatStyleStats.priest_stats.movSpeed;
                playerInfo.health = combatStyleStats.priest_stats.health;
                playerInfo.mana = combatStyleStats.priest_stats.mana;

                break;
            case PlayerClass.CombatStyles.Warrior:


                playerInfo.attack = combatStyleStats.warrior_stats.attack;
                playerInfo.defence = combatStyleStats.warrior_stats.defence;
                playerInfo.movSpeed = combatStyleStats.warrior_stats.movSpeed;
                playerInfo.health = combatStyleStats.warrior_stats.health;
                playerInfo.mana = combatStyleStats.warrior_stats.mana;

                break;
            case PlayerClass.CombatStyles.Mage:
                playerInfo.attack = combatStyleStats.mage_stats.attack;
                playerInfo.defence = combatStyleStats.mage_stats.defence;
                playerInfo.movSpeed = combatStyleStats.mage_stats.movSpeed;
                playerInfo.health = combatStyleStats.mage_stats.health;
                playerInfo.mana = combatStyleStats.mage_stats.mana;
                break;
            case PlayerClass.CombatStyles.Bard:
                playerInfo.attack = combatStyleStats.bard_stats.attack;
                playerInfo.defence = combatStyleStats.bard_stats.defence;
                playerInfo.movSpeed = combatStyleStats.bard_stats.movSpeed;
                playerInfo.health = combatStyleStats.bard_stats.health;
                playerInfo.mana = combatStyleStats.bard_stats.mana;
                break;
            case PlayerClass.CombatStyles.Cleric:
                playerInfo.attack = combatStyleStats.cleric_stats.attack;
                playerInfo.defence = combatStyleStats.cleric_stats.defence;
                playerInfo.movSpeed = combatStyleStats.cleric_stats.movSpeed;
                playerInfo.health = combatStyleStats.cleric_stats.health;
                playerInfo.mana = combatStyleStats.cleric_stats.mana;
                break;
            case PlayerClass.CombatStyles.Monk:
                playerInfo.attack = combatStyleStats.monk_stats.attack;
                playerInfo.defence = combatStyleStats.monk_stats.defence;
                playerInfo.movSpeed = combatStyleStats.monk_stats.movSpeed;
                playerInfo.health = combatStyleStats.monk_stats.health;
                playerInfo.mana = combatStyleStats.monk_stats.mana;
                break;
            case PlayerClass.CombatStyles.Ranger:
                playerInfo.attack = combatStyleStats.ranger_stats.attack;
                playerInfo.defence = combatStyleStats.ranger_stats.defence;
                playerInfo.movSpeed = combatStyleStats.ranger_stats.movSpeed;
                playerInfo.health = combatStyleStats.ranger_stats.health;
                playerInfo.mana = combatStyleStats.ranger_stats.mana;
                break;


        }


    }

    /// <summary>
    /// Return ability deck according to chosen combat style
    /// </summary>
    /// <param name="combatStyle"></param>
    /// <returns></returns>
    AbilityCardClass[] GetAbilityDeck(PlayerClass.CombatStyles combatStyle)
    {
        switch(combatStyle)
        {
            case PlayerClass.CombatStyles.Priest:
                return abilityDecks.priest_AbilityDeck;
            case PlayerClass.CombatStyles.Warrior:
                return abilityDecks.warrior_AbilityDeck;
            case PlayerClass.CombatStyles.Mage:
                return abilityDecks.mage_AbilityDeck;
            case PlayerClass.CombatStyles.Bard:
                return abilityDecks.bard_AbilityDeck;
            case PlayerClass.CombatStyles.Cleric:
                return abilityDecks.cleric_AbilityDeck;
            case PlayerClass.CombatStyles.Monk:
                return abilityDecks.monk_AbilityDeck;
            case PlayerClass.CombatStyles.Ranger:
                return abilityDecks.ranger_AbilityDeck;
            default:
                return null;

        }

    }

    /// <summary>
    /// Check number of ready players against number of players
    /// </summary>
    /// <returns></returns>
    bool  CheckIfAllPlayersAreReady()
    {
        int readyCount = 0;
        bool partyReady = false;
        for (int i = 0; i < playersReady.Length; i++)
        {
            if (playersReady[i] == true) readyCount++;
        }

        if (readyCount == _GM.playerGameObjList.Count) partyReady = true;

        return partyReady;
    }

    #endregion
}
