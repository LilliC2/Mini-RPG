using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTP : GameBehaviour
{
    [SerializeField]
    List<Vector3> goalDoorSpawnPoints;
    public GameObject goalDoor;

    public void GetGoalDoorSpawnPoints(GameObject _goalDoor)
    {
        goalDoor = _goalDoor;
        for (int i = 0; i < goalDoor.transform.childCount; i++)
        {
            if (goalDoor.transform.GetChild(i).gameObject.name.Contains("TP"))
            {
                print("Added " + goalDoor.transform.GetChild(i).gameObject.name);
                goalDoorSpawnPoints.Add(goalDoor.transform.GetChild(i).transform.position);

            }
        }

    }

    void TeleportPlayers()
    {
        foreach (var player in _GM.playerGameObjList)
        {
            for (int i = 0; i < goalDoorSpawnPoints.Count; i++)
            {
                //player.GetComponent<CharacterController>().enabled = false;
                player.transform.position = goalDoorSpawnPoints[i];
              //  player.GetComponent<CharacterController>().enabled = true;

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) TeleportPlayers();
    }
}
