using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleScreenUI : GameBehaviour
{

    int combatStyleCount_P1 = 0;
    int avatarCount_P1 = 0;

    [SerializeField]
    GameObject[] playerCustomisation;


    [SerializeField]
    TMP_Text[] playerJoinText; //temp
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
    }

    #endregion
}
