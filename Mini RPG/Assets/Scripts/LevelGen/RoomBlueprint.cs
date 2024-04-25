using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBlueprint : GameBehaviour
{
    [System.Serializable]
    public class RoomTileData
    {
        string roomName;
        public Vector2 roomCoOrds;
        enum Direction { Forward, Back, Right, Left }
        Direction[] validDoorDirections;
        public GameObject forwardDoor;
        public GameObject backDoor;
        public GameObject leftDoor;
        public GameObject rightDoor;

        public List<GameObject> allDoors;
    }

    public RoomTileData roomTileData;

    // Update is called once per frame
    void Update()
    {
        
    }
}
