using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatUI : GameBehaviour
{

    [SerializeField] GameObject abilityDeckGO_P1;
    [SerializeField] GameObject abilityDeckGO_P2;
    [SerializeField] GameObject abilityDeckGO_P3;
    [SerializeField] GameObject abilityDeckGO_P4;


    [SerializeField] GameObject[] abilityDeckCardsGO_P1;
    [SerializeField] GameObject[] abilityDeckCardsGO_P2;
    [SerializeField] GameObject[] abilityDeckCardsGO_P3;
    [SerializeField] GameObject[] abilityDeckCardsGO_P4;

    [SerializeField] TMP_Text[] abilityDeckCards_card1_P1;
    [SerializeField] TMP_Text[] abilityDeckCards_card2_P1;
    [SerializeField] TMP_Text[] abilityDeckCards_card3_P1;


    // Start is called before the first frame update
    void Start()
    {
        _GM.event_EnteredCombatScene.AddListener(DisplayAbilityDecks);
    }

    public void DisplayAbilityDecks()
    {

        if (_GM.playerGameObjList[0] != null) { abilityDeckGO_P1.SetActive(true); } else abilityDeckGO_P1.SetActive(false);
        //if (_GM.playerGameObjList[1] != null) { abilityDeckGO_P2.SetActive(true); } else abilityDeckGO_P2.SetActive(false);
        //if (_GM.playerGameObjList[2] != null) { abilityDeckGO_P3.SetActive(true); } else abilityDeckGO_P3.SetActive(false);
        //if (_GM.playerGameObjList[3] != null) { abilityDeckGO_P4.SetActive(true); } else abilityDeckGO_P4.SetActive(false);

    }

    public void UpdateSelectedAbilityCard(int playerNum, int cardIndex)
    {

        print(cardIndex);
        switch (playerNum)
        {
            case 0:

                for (int i = 0; i < abilityDeckCardsGO_P1.Length; i++)
                {
                    abilityDeckCardsGO_P1[i].GetComponent<Image>().color = Color.white;
                }
                abilityDeckCardsGO_P1[cardIndex].GetComponent<Image>().color = Color.yellow;

                break;

            case 1:

                for (int i = 0; i < abilityDeckCardsGO_P2.Length; i++)
                {
                    if (i == cardIndex) abilityDeckCardsGO_P2[i].GetComponent<Image>().color = Color.yellow;
                    else abilityDeckCardsGO_P2[cardIndex].GetComponent<Image>().color = Color.white;
                }

                break;
            case 2:

                for (int i = 0; i < abilityDeckCardsGO_P3.Length; i++)
                {
                    if (i == cardIndex) abilityDeckCardsGO_P3[i].GetComponent<Image>().color = Color.yellow;
                    else abilityDeckCardsGO_P3[cardIndex].GetComponent<Image>().color = Color.white;
                }

                break;
            case 3:

                for (int i = 0; i < abilityDeckCardsGO_P4.Length; i++)
                {
                    if (i == cardIndex) abilityDeckCardsGO_P4[i].GetComponent<Image>().color = Color.yellow;
                    else abilityDeckCardsGO_P4[cardIndex].GetComponent<Image>().color = Color.white;
                }

                break;


        }
    }

    public void UpdateP1AbilityCards(AbilityCardClass[] abilityCards)
    {
        abilityDeckCards_card1_P1[0].text = abilityCards[0].name;
        abilityDeckCards_card1_P1[1].text = abilityCards[0].manaCost.ToString();
        abilityDeckCards_card1_P1[2].text = abilityCards[0].cooldown.ToString();

        abilityDeckCards_card2_P1[0].text = abilityCards[1].name;
        abilityDeckCards_card2_P1[1].text = abilityCards[1].manaCost.ToString();
        abilityDeckCards_card2_P1[2].text = abilityCards[1].cooldown.ToString();

        abilityDeckCards_card3_P1[0].text = abilityCards[2].name;
        abilityDeckCards_card3_P1[1].text = abilityCards[2].manaCost.ToString();
        abilityDeckCards_card3_P1[2].text = abilityCards[2].cooldown.ToString();
    }


    // Update is called once per frame
    void Update()
    {

    }


}
