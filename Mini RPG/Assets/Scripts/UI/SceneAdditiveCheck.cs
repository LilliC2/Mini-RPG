using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Checks if the UI scene is addivitly loaded
/// </summary>
public class SceneAdditiveCheck : MonoBehaviour
{
    Scene UIscene;

    private void Awake()
    {
        UIscene = SceneManager.GetSceneByName("UI");
        if (!UIscene.isLoaded) SceneManager.LoadScene("UI", LoadSceneMode.Additive);
    }
}
