using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTP : GameBehaviour
{
    List<GameObject> playersInTrigger = new List<GameObject>();

    void GenerateNextLevel()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger.Add(other.gameObject);

            if(playersInTrigger.Count == _GM.playerGameObjList.Count)
                GenerateNextLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
            if (other.CompareTag("Player"))
            {
                playersInTrigger.Remove(other.gameObject);

            }
    }
}
