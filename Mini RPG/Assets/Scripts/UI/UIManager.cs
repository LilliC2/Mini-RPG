using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public TitleScreenUI titleScreenUI;
    public CombatUI combatUI;

    private void Awake()
    {
        _GM.event_ChangeActionMap.AddListener(ToggleCursors);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Backspace)) ToggleCursors();
    }

    public void ToggleCursors()
    {
        var mice = GameObject.FindGameObjectsWithTag("Cursor");

        if(_GM.gameState == GameManager.GameState.PlayersInUI)
        {
            foreach (var cursor in mice)
            {
                cursor.SetActive(true);
            }
        }
        else
        {
            foreach (var cursor in mice)
            {
                cursor.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        titleScreenUI = FindAnyObjectByType<TitleScreenUI>();
        combatUI = FindAnyObjectByType<CombatUI>();
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName.Contains("Combat")) _GM.gameState = GameManager.GameState.Combat;
        SceneManager.LoadScene(sceneName);

    }
}
