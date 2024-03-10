using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpot : GameBehaviour
{
    public string sceneName;

    public List<GameObject> connections;

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
