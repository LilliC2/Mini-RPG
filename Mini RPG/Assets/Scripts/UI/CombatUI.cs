using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatUI : GameBehaviour
{

    [SerializeField] GameObject playercard_P1;
    [SerializeField] GameObject playercard_P2;
    [SerializeField] GameObject playercard_P3;
    [SerializeField] GameObject playercard_P4;

    [SerializeField] TMP_Text[] healthText;
    [SerializeField] Image[] healthSlider;
    [SerializeField] TMP_Text[] manaText;
    [SerializeField] Image[] manaSlider;

    // Start is called before the first frame update
    void Start()
    {
        _GM.event_EnteredCombatScene.AddListener(DisplayAbilityDecks);
    }

    public void DisplayAbilityDecks()
    {

        if (_GM.playerGameObjList[0] != null) { playercard_P1.SetActive(true); } else playercard_P1.SetActive(false);
        //if (_GM.playerGameObjList[1] != null) { abilityDeckGO_P2.SetActive(true); } else abilityDeckGO_P2.SetActive(false);
        //if (_GM.playerGameObjList[2] != null) { abilityDeckGO_P3.SetActive(true); } else abilityDeckGO_P3.SetActive(false);
        //if (_GM.playerGameObjList[3] != null) { abilityDeckGO_P4.SetActive(true); } else abilityDeckGO_P4.SetActive(false);

    }

    public void UpdatePlayerHealthMana(int playerIndex, int currentHealth, int maxHealth, int currentMana, int maxMana)
    {
        healthText[0].text = currentHealth.ToString();
        manaText[0].text = currentMana.ToString();

        float hp = currentHealth;
        float mana = currentMana;

        healthSlider[playerIndex].fillAmount = hp/maxHealth;
        manaSlider[playerIndex].fillAmount = mana/maxMana;

    }

}
