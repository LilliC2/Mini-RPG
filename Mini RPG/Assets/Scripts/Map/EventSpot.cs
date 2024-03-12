using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class EventSpot : GameBehaviour
{
    public string levelName;
    public string eventType;

    public List<GameObject> connections;
    public string scene;

    public bool entry;
    public bool exit;

    public bool CheckConnection(GameObject location)
    {
        bool canTravelThere = false;

        foreach (var connection in connections)
        {
            if (location == connection) canTravelThere = true;
        }

        return canTravelThere;
    }

}
