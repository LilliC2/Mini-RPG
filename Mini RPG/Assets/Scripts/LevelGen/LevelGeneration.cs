using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LevelGeneration : Singleton<LevelGeneration>
{
    HashSet<Vector3> tilesHashSet;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Grid grid;
    [SerializeField] GameObject tempRoom;
    [SerializeField] GameObject tempCorridor;
    [SerializeField] List<Vector2> roomCoOrdsList;
    [SerializeField] List<GameObject> roomGOList;


    [SerializeField] List<GameObject> mainlineRooms;
    [SerializeField] List<GameObject> spawnRooms;
    [SerializeField] List<GameObject> endRooms;


    [SerializeField] List<GameObject> forwardDoorRooms;
    [SerializeField] List<GameObject> backDoorRooms;
    [SerializeField] List<GameObject> rightDoorRooms;
    [SerializeField] List<GameObject> leftDoorRooms;


    [SerializeField] float minCorridorDistance;
    [SerializeField] float maxCorridorDistance;

    [SerializeField] int currentNumOfAdditionalRooms;
    [SerializeField] int totalNumOfAdditionalRooms;

    GameObject selectedRoomPrefab;
    GameObject doorOfPlacedRoom;

    [SerializeField] List<GameObject> notConnectedDoorList;

    /*
     * Forward = z +1
     * Back = z -1
     * Right = x +1
     * Left = x -1
     */

    private void Start()
    {
        tilesHashSet = new HashSet<Vector3>();
        GenerateDungeon();

    }

    private void Update()
    {

    }

    void GenerateDungeon()
    {
        PlaceMainLineRooms();

       // totalNumOfAdditionalRooms = 5; //Random.Range(5, 10);

        while(currentNumOfAdditionalRooms != totalNumOfAdditionalRooms && notConnectedDoorList.Count != 0)
        {
            FindValidRoomSpot();
        }

        foreach (var door in notConnectedDoorList)
        {
            door.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void FindValidRoomSpot()
    {

        //pick random door
        var selectedDoor = notConnectedDoorList[Random.Range(0, notConnectedDoorList.Count)];
        var selecteRoom = selectedDoor.transform.parent.gameObject;
        var selectedDoorBlueprint = selecteRoom.GetComponent<RoomBlueprint>();

        print(selecteRoom);
        int direction = 0;

        /*
         * Forward = z +1
         * Back = z -1
         * Right = x +1
         * Left = x -1
         */

        Vector2 roomCoOrds = selectedDoorBlueprint.roomTileData.roomCoOrds;
        print("Checking door at coords " + roomCoOrds);
        Vector2 checkingCoOrds = new();
        //find its direction
        if (selectedDoor.name.Contains("Forward"))
        {
            checkingCoOrds = new Vector2(roomCoOrds.x, roomCoOrds.y + 1);
            direction = 1;
        }
        if (selectedDoor.name.Contains("Back"))
        {
            checkingCoOrds = new Vector2(roomCoOrds.x, roomCoOrds.y - 1);
            direction = 2;
        }
        if (selectedDoor.name.Contains("Right"))
        {
            checkingCoOrds = new Vector2(roomCoOrds.x+1, roomCoOrds.y);
            direction = 3;
        }
        if (selectedDoor.name.Contains("Left"))
        {
            checkingCoOrds = new Vector2(roomCoOrds.x-1, roomCoOrds.y);
            direction = 4;
        }

        //check if theres a room in the coords of where we want to place
        if(!roomCoOrdsList.Contains(checkingCoOrds))
        {
            print("Room is valid go to next step with coords " + checkingCoOrds + " from checking coords " + roomCoOrds + " door is " + selectedDoor.name);
            print("Room is valid go to next step with coords " + checkingCoOrds + " from checking coords " + roomCoOrds + " door is " + selectedDoor.name + " direction " + direction);
            //yes we can place a room here
            FindValidRoomType(selectedDoor,checkingCoOrds,direction);
        }
        else
        {
            print("Room is invalid, return");

            //close door
            //selectedDoor.GetComponent<Renderer>().material.color = Color.red;
            notConnectedDoorList.Remove(selectedDoor);
        }


    }

    void FindValidRoomType(GameObject connectingDoor, Vector2 roomCoOrds, int direction)
    {

        switch(direction)
        {
            case 1:
                //forward
                selectedRoomPrefab = backDoorRooms[Random.Range(0, backDoorRooms.Count)];

                break;
            case 2:
                //back
                selectedRoomPrefab = forwardDoorRooms[Random.Range(0, forwardDoorRooms.Count)];

                break;
            case 3:
                //right
                selectedRoomPrefab = leftDoorRooms[Random.Range(0, leftDoorRooms.Count)];

                break;
            case 4:
                //left
                selectedRoomPrefab = rightDoorRooms[Random.Range(0, rightDoorRooms.Count)];

                break;
        }

        if (selectedRoomPrefab != null) print("Attempting to place: " + connectingDoor.name + ", " + selectedRoomPrefab.name + ", " + roomCoOrds + ", " + direction);

        currentNumOfAdditionalRooms++;
        PlaceRoom(connectingDoor, selectedRoomPrefab, roomCoOrds, direction);
        selectedRoomPrefab = null;

    }

    void PlaceMainLineRooms()
    {
        /*Would be more instresting if the end room was placed first in a random spot numOfRoomsOnMainLine away from spawn
         * then the mainline builds towards it, then would encourage twists and turns in the critical path
         */


        //place spawn room
        PlaceRoom(null, spawnRooms[0], new Vector2(0, 0), 0);


        int numOfRoomsOnMainLine = 5;// Random.Range(3, 7);
        print("Adding " + numOfRoomsOnMainLine + " to mainline");

        for (int i = 1; i <= numOfRoomsOnMainLine; i++)
        {
            //place end room
            print("Placing room " + i);
            RoomBlueprint previousRoomBlueprint;


            previousRoomBlueprint = roomGOList[i - 1].GetComponent<RoomBlueprint>();

            //all these rooms are being placed in 1 direction, forward
            var connectingDoor = previousRoomBlueprint.roomTileData.forwardDoor;

            //here i would generate the room type
            if (i == numOfRoomsOnMainLine)
            {
                print("End room");
                PlaceRoom(connectingDoor, endRooms[0], new Vector2(0, i), 1);
            }
            else
            {
                PlaceRoom(connectingDoor, mainlineRooms[Random.Range(0, mainlineRooms.Count)], new Vector2(0, i), 1);

            }



        }




    }

    void PlaceRoom(GameObject connectingDoor, GameObject roomPrefab, Vector2 roomCoOrds, int direction)
    {
        /*
         * Forward = z +1
         * Back = z -1
         * Right = x +1
         * Left = x -1
         */

        //check if room is at those coOrds
        if (!roomCoOrdsList.Contains(roomCoOrds))
        {
            print("roomCoOrds " + roomCoOrds + " are free");

            var roomPlaced = Instantiate(roomPrefab);

            //get bounds of room
            var bounds = roomPlaced.GetComponent<Collider>().bounds;
            float distanceToAdd = 0;
            Vector3 placePosition = new();
            //if connecting door is null, it is the spawn room
            if (connectingDoor == null)
            {
                placePosition = Vector3.zero;
            }
            else
            {
                placePosition = new Vector3(connectingDoor.transform.position.x, 0, connectingDoor.transform.position.z);

            }

            //if spawn room, direction = 0
            if (direction == 1 || direction == 2) //forwards or back
            {
                //get Z axis for extents
                distanceToAdd = bounds.extents.z + Mathf.RoundToInt(Random.Range(minCorridorDistance,maxCorridorDistance));

                if (direction ==1)//forwards
                    placePosition = new Vector3(placePosition.x, placePosition.y, placePosition.z + distanceToAdd);
                else if (direction == 2)//back
                    placePosition = new Vector3(placePosition.x, placePosition.y, placePosition.z - distanceToAdd);

            }
            else if (direction == 3 || direction == 4) //right or left
            {
                //get X axis for extents

                distanceToAdd = bounds.extents.x + Mathf.RoundToInt(Random.Range(minCorridorDistance, maxCorridorDistance));
                if (direction == 3)//right
                    placePosition = new Vector3(placePosition.x + distanceToAdd, placePosition.y, placePosition.z);
                else if (direction == 4)//left
                    placePosition = new Vector3(placePosition.x - distanceToAdd, placePosition.y, placePosition.z);
            }

            //var tilePos = grid.WorldToCell(placePosition);
            //print("PlacePos = " + placePosition + " TilePos = " + tilePos);
            //roomPlaced.transform.position = tilePos;
            var bluePrints = roomPlaced.GetComponent<RoomBlueprint>();


            if (connectingDoor != null) connectingDoor.GetComponent<Renderer>().material.color = Color.green;
            //door from this room go green
              
            switch (direction)
            {
                case 1:
                    //forward

                    doorOfPlacedRoom = bluePrints.roomTileData.backDoor;

                    break;
                case 2:
                    //back
                    doorOfPlacedRoom = bluePrints.roomTileData.forwardDoor;


                    break;
                case 3:
                    //right
                    doorOfPlacedRoom = bluePrints.roomTileData.leftDoor;


                    break;
                case 4:
                    //left
                    doorOfPlacedRoom = bluePrints.roomTileData.rightDoor;
                    break;
                default: //for spawn room
                    doorOfPlacedRoom = bluePrints.roomTileData.allDoors[0];
                    break;
            }
            doorOfPlacedRoom.GetComponent<Renderer>().material.color = Color.green;
            notConnectedDoorList.Remove(doorOfPlacedRoom);


            roomPlaced.transform.position = placePosition;
            bluePrints.roomTileData.roomCoOrds = roomCoOrds;

            roomCoOrdsList.Add(roomCoOrds);
            roomGOList.Add(roomPlaced);
            notConnectedDoorList.AddRange(roomPlaced.GetComponent<RoomBlueprint>().roomTileData.allDoors);

            if(connectingDoor != null) PlaceCorridor(connectingDoor, doorOfPlacedRoom, direction);

        }
    }

    void PlaceCorridor(GameObject doorA, GameObject doorB, int direction)
    {
        //travel from A to B
        var distanceBetweenDoors = Mathf.RoundToInt(Vector3.Distance(doorA.transform.position, doorB.transform.position));
        Vector3 lastPlacePos = doorA.transform.parent.transform.position;
        Vector3 currentPlacePos = new();
        print(distanceBetweenDoors);
        for (int i = 0; i < distanceBetweenDoors; i++)
        {
            
            switch (direction)
            {
                case 1:
                    //forward
                    currentPlacePos = new Vector3(lastPlacePos.x, lastPlacePos.y, lastPlacePos.z + 1);

                    break;
                case 2:
                    //back
                    currentPlacePos = new Vector3(lastPlacePos.x, lastPlacePos.y, lastPlacePos.z - 1);


                    break;
                case 3:
                    //right
                    currentPlacePos = new Vector3(lastPlacePos.x+1, lastPlacePos.y, lastPlacePos.z);


                    break;
                case 4:
                    //left
                    currentPlacePos = new Vector3(lastPlacePos.x - 1, lastPlacePos.y, lastPlacePos.z);

                    break;

            }

            
            var corridor = Instantiate(tempCorridor, currentPlacePos, doorA.transform.rotation);
            lastPlacePos = currentPlacePos;
        }
    }
}
