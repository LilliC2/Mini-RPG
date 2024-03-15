using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{

    public GameObject[] uiOverlays;

    public TitleScreenUI titleScreenUI;
    public CombatUI combatUI;
    public OverworldMapUI overworldMapUI;

    private void Awake()
    {
        _GM.event_ChangeActionMap.AddListener(ToggleCursors);
        _GM.event_LoadedNewScene.AddListener(ChangeUIOverlay);
        _GM.event_LoadedNewScene.AddListener(GetScripts);
        GetScripts();
        ChangeUIOverlay();
    }

    public void ChangeUIOverlay()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        int indexSetScene = -1;

        if (sceneName.Contains("Title")) indexSetScene =0;
        if (sceneName.Contains("Map")) indexSetScene = 1;
        if (sceneName.Contains("Combat")) indexSetScene = 2;

        for (int i = 0; i < uiOverlays.Length; i++)
        {
            if (i != indexSetScene) uiOverlays[i].SetActive(false);
            else uiOverlays[i].SetActive(true);
        }

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

    void GetScripts()
    {
        titleScreenUI = FindAnyObjectByType<TitleScreenUI>();
        combatUI = FindAnyObjectByType<CombatUI>();
        overworldMapUI = FindAnyObjectByType<OverworldMapUI>();
    }

    private void OnEnable()
    {
        GetScripts();
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName.Contains("Combat")) _GM.gameState = GameManager.GameState.Combat;
        SceneManager.LoadScene(sceneName);

    }
}
