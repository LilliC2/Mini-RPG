using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleScreenUI : GameBehaviour
{
    [SerializeField]
    AbiltyDecks abilityDecks;

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

                //add avatar stuff here

                break;
        }

        if (CheckIfAllPlayersAreReady()) _UI.LoadScene("Assembly");
    }

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
