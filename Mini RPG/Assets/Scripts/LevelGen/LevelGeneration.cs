using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LevelGeneration : Singleton<LevelGeneration>
{
    HashSet<Vector3> tilesHashSet;
    [SerializeField] GameObject dungeonRoomsParent;

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
    [SerializeField] float corridorTileSize;

    [SerializeField] int currentNumOfAdditionalRooms;
    [SerializeField] int totalNumOfAdditionalRooms;

    GameObject selectedRoomPrefab;
    GameObject doorOfPlacedRoom;
    [SerializeField] bool addingToMainline;
    [SerializeField] bool spawnAdditionalRooms = false;

    [SerializeField] List<GameObject> notConnectedDoorList;

    /*
     * Forward = z +1
     * Back = z -1
     * Right = x +1
     * Left = x -1
     */


    /*THINGS TO ADD
     * 
     * - Any rooms more than 1 co-ord away after not active to improve peformance
     * - make levels child of something
     * - system to delete current level so new level can be placed
     * 
     */


    private void Start()
    {
        tilesHashSet = new HashSet<Vector3>();
        GenerateDungeon();

    }

    private void Update()
    {
        while (spawnAdditionalRooms)
        {
            print("Add additional rooms");
            FindValidRoomSpot();
            if (currentNumOfAdditionalRooms == totalNumOfAdditionalRooms || notConnectedDoorList.Count == 0) spawnAdditionalRooms = false; 
        }
    }

    void GenerateDungeon()
    {
        addingToMainline = true;

        PlaceMainLineRooms();

        // totalNumOfAdditionalRooms = 5; //Random.Range(5, 10);
        var allFloorTiles = GameObject.FindGameObjectsWithTag("FloorTile");
        foreach (var tile in allFloorTiles)
        {
            tilesHashSet.Add(tile.transform.position);
        }

        spawnAdditionalRooms = true;

        //if(currentNumOfAdditionalRooms == totalNumOfAdditionalRooms)
        //{
        //    foreach (var door in notConnectedDoorList)
        //    {
        //        door.GetComponent<Renderer>().material.color = Color.red;
        //    }
        //}

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
        if (selectedDoorBlueprint.roomTileData.forwardDoor == selectedDoor)
        {
            checkingCoOrds = new Vector2(roomCoOrds.x, roomCoOrds.y + 1);
            print("Door go forward");
            direction = 1;
        }
        if (selectedDoorBlueprint.roomTileData.backDoor == selectedDoor)
        {
            checkingCoOrds = new Vector2(roomCoOrds.x, roomCoOrds.y - 1);
            print("Door go back");

            direction = 2;
        }
        if (selectedDoorBlueprint.roomTileData.rightDoor == selectedDoor)
        {
            checkingCoOrds = new Vector2(roomCoOrds.x+1, roomCoOrds.y);
            print("Door go right");

            direction = 3;
        }
        if (selectedDoorBlueprint.roomTileData.leftDoor == selectedDoor)
        {
            checkingCoOrds = new Vector2(roomCoOrds.x-1, roomCoOrds.y);
            print("Door go left");

            direction = 4;
        }

        print("Checking coords " + checkingCoOrds + " are free");

        //check if theres a room in the coords of where we want to place
        if(!roomCoOrdsList.Contains(checkingCoOrds))
        {
            print("Room is valid go to next step with coords " + checkingCoOrds + " from checking coords " + roomCoOrds + " door is " + selectedDoor.name + " direction " + direction + " add to mainline " + addingToMainline);
            //yes we can place a room here
            var room = FindValidRoomType(selectedDoor,checkingCoOrds,direction);
            PlaceRoom(selectedDoor, room, checkingCoOrds, direction);
            if (!addingToMainline) currentNumOfAdditionalRooms++;

        }
        else
        {
            print("Room is invalid at " + checkingCoOrds +", return");

            //close door
            //selectedDoor.GetComponent<Renderer>().material.color = Color.red;
            notConnectedDoorList.Remove(selectedDoor);

        }



    }

    GameObject FindValidRoomType(GameObject connectingDoor, Vector2 roomCoOrds, int direction)
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

        if (selectedRoomPrefab != null)
        {
            print("Attempting to place: " + connectingDoor.name + ", " + selectedRoomPrefab.name + ", " + roomCoOrds + ", " + direction + mainlineRooms);
        }
        else print("NO PREFAB FOUND");


        return selectedRoomPrefab;
    }

    void PlaceMainLineRooms()
    {
        /*Would be more instresting if the end room was placed first in a random spot numOfRoomsOnMainLine away from spawn
         * then the mainline builds towards it, then would encourage twists and turns in the critical path
         */

        //place spawn room
        PlaceRoom(null, spawnRooms[0], new Vector2(0, 0), 0);

        int numOfRoomsOnMainLine = 5;// Random.Range(3, 7);

        //calculate end room
        int x = Random.Range(0, numOfRoomsOnMainLine + 1);
        int z = numOfRoomsOnMainLine - x;

        //50/50 to make x a negative
        if (Random.Range(0, 2) == 0) x = -x;

        Vector2 endRoomCoords = new Vector2(x, z);

        ////place end room
        //PlaceRoom(null, endRooms[0], new Vector2(x, z), 0);

        Vector2 lastPlacePos = roomCoOrdsList[0];

        for (int i = 1; i <= numOfRoomsOnMainLine; i++)
        {
            RoomBlueprint previousRoomBlueprint;

            //move x first
            Vector2 placePos = lastPlacePos;
            int direction = 0;


            if(lastPlacePos.x != x)
            {
                if (x > lastPlacePos.x)
                {
                    placePos = new Vector2(placePos.x + 1, placePos.y);
                    direction = 3;
                }
                else if (x < lastPlacePos.x)
                {
                    placePos = new Vector2(placePos.x - 1, placePos.y);
                    direction = 4;
                }
            }
            if(x == lastPlacePos.x && z != lastPlacePos.y)
            {
                if (z > lastPlacePos.y)
                {
                    placePos = new Vector2(placePos.x, placePos.y + 1);
                    direction = 1;

                }
                else if (z < lastPlacePos.y)
                {
                    placePos = new Vector2(placePos.x, placePos.y - 1);
                    direction = 2;

                }
            }
            print("Last place = " + lastPlacePos + " heading to " + endRoomCoords + " in direction " + direction);


            previousRoomBlueprint = roomGOList[i - 1].GetComponent<RoomBlueprint>();
            print(previousRoomBlueprint);
            switch (direction)
            {
                case 1:
                    //forward
                    print("Forward");
                    doorOfPlacedRoom = previousRoomBlueprint.roomTileData.forwardDoor;

                    break;
                case 2:
                    //back
                    print("back");

                    doorOfPlacedRoom = previousRoomBlueprint.roomTileData.backDoor;


                    break;
                case 3:
                    //right
                    print("right");

                    doorOfPlacedRoom = previousRoomBlueprint.roomTileData.rightDoor;

                    break;
                case 4:
                    //left
                    print("left");

                    doorOfPlacedRoom = previousRoomBlueprint.roomTileData.leftDoor;
                    break;
                default: //for spawn room
                    print("default");

                    doorOfPlacedRoom = previousRoomBlueprint.roomTileData.allDoors[0];
                    break;
            }

            print("Connecting to " + doorOfPlacedRoom.name);


            if (i == numOfRoomsOnMainLine)
            {
                PlaceRoom(doorOfPlacedRoom, endRooms[0], endRoomCoords, direction);
                addingToMainline = false;
            }
            else
            {
                //calculate next direction
                int nextDirection = 0;

                if (placePos.x != x)
                {
                    if (x > placePos.x)
                    {
                        nextDirection = 3;
                    }
                    else if (x < lastPlacePos.x)
                    {
                        nextDirection = 4;
                    }
                }
                if (x == placePos.x && z != placePos.y)
                {
                    if (z > placePos.y)
                    {
                        nextDirection = 1;
                    }
                    else if (z < placePos.y)
                    {
                        nextDirection = 2;
                    }
                }
                var room = MainlineValidPlace(direction, nextDirection);
                PlaceRoom(doorOfPlacedRoom, room, placePos, direction);
            }
            lastPlacePos = placePos;
            doorOfPlacedRoom = null;
        }

    }

    GameObject MainlineValidPlace(int currentDirection, int nextDirection)
    {
        GameObject selectedRoom = null;

        List<GameObject> possibleRooms = new List<GameObject>();
        print("Current direction " + currentDirection + " Next Direction " + nextDirection);
        //last door
        if(currentDirection == nextDirection)
        {
            //heading in same direction
            if(currentDirection == 1 || currentDirection == 2) //door must be forward or back
            {
                for (int i = 0; i < forwardDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(backDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(backDoorRooms[i]);
                    }
                }

            }
            else if(currentDirection ==3  || currentDirection == 4)
            {
                for (int i = 0; i < leftDoorRooms.Count; i++)
                {
                    if (leftDoorRooms.Contains(rightDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(rightDoorRooms[i]);
                    }
                }
            }
        }
        else if(currentDirection != nextDirection)
        {
            //turning
            print("turn");
            if((currentDirection == 1 && nextDirection == 2) || (currentDirection == 2 && nextDirection == 1))
            {
                for (int i = 0; i < forwardDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(backDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(backDoorRooms[i]);
                    }
                }
            }
            if((currentDirection == 3 && nextDirection == 4) || (currentDirection == 4 && nextDirection == 3))
            {
                for (int i = 0; i < leftDoorRooms.Count; i++)
                {
                    if (leftDoorRooms.Contains(rightDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(rightDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 1 && nextDirection == 3)
            {
                for (int i = 0; i < forwardDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(rightDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(rightDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 1 && nextDirection == 4)
            {
                for (int i = 0; i < forwardDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(leftDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(leftDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 2 && nextDirection == 4)
            {
                for (int i = 0; i < backDoorRooms.Count; i++)
                {
                    if (backDoorRooms.Contains(leftDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(leftDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 2 && nextDirection == 3)
            {

                for (int i = 0; i < backDoorRooms.Count; i++)
                {
                    if (backDoorRooms.Contains(rightDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(rightDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 4 && nextDirection == 1)
            {

                for (int i = 0; i < rightDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(rightDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(rightDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 4 && nextDirection == 2)
            {

                for (int i = 0; i < rightDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(rightDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(rightDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 3 && nextDirection == 1)
            {

                for (int i = 0; i < leftDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(leftDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(leftDoorRooms[i]);
                    }
                }
            }
            else if(currentDirection == 3 && nextDirection == 2)
            {

                for (int i = 0; i < leftDoorRooms.Count; i++)
                {
                    if (forwardDoorRooms.Contains(leftDoorRooms[i]))
                    {
                        //room is in both
                        possibleRooms.Add(leftDoorRooms[i]);
                    }
                }
            }



        }

        print(possibleRooms.Count);
        selectedRoom = possibleRooms[Random.Range(0, possibleRooms.Count)];

        return selectedRoom;
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
            print("Create "+ roomPrefab.name);

            var roomPlaced = Instantiate(roomPrefab);

            //get bounds of room
            var bounds = roomPlaced.GetComponent<Collider>().bounds;
            float distanceToAdd = 0;
            Vector3 placePosition = new();
            //if connecting door is null, it is the spawn room
            if (roomCoOrds == Vector2.zero)
            {
                placePosition = Vector3.zero;
            }
            else
            {
                placePosition = new Vector3(connectingDoor.transform.parent.transform.position.x, 0, connectingDoor.transform.parent.transform.position.z);

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


            //if (connectingDoor != null) connectingDoor.GetComponent<Renderer>().material.color = Color.green;
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

            print("Direction " + direction + " door " + doorOfPlacedRoom);
            if(notConnectedDoorList.Contains(doorOfPlacedRoom))notConnectedDoorList.Remove(doorOfPlacedRoom);


            roomPlaced.transform.position = placePosition;
            bluePrints.roomTileData.roomCoOrds = roomCoOrds;

            roomCoOrdsList.Add(roomCoOrds);
            roomGOList.Add(roomPlaced);
            notConnectedDoorList.AddRange(roomPlaced.GetComponent<RoomBlueprint>().roomTileData.allDoors);

            if(connectingDoor != null && doorOfPlacedRoom != null) SetDoorTeleports(connectingDoor, doorOfPlacedRoom);


            //if(connectingDoor != null) PlaceCorridor(connectingDoor, doorOfPlacedRoom, direction);

        }
    }

    void SetDoorTeleports(GameObject doorA, GameObject doorB)
    {
        var doorATPscript = doorA.GetComponentInChildren<RoomTP>();
        var doorBTPscript = doorB.GetComponentInChildren<RoomTP>();

        doorATPscript.GetGoalDoorSpawnPoints(doorB);
        doorBTPscript.GetGoalDoorSpawnPoints(doorA);

        doorA.GetComponent<Renderer>().material.color = Color.green;
        doorB.GetComponent<Renderer>().material.color = Color.green;

    }

    //void PlaceCorridor(GameObject doorA, GameObject doorB, int direction)
    //{
    //    //travel from A to B
    //    var distanceBetweenDoors = Vector3.Distance(doorA.transform.position, doorB.transform.position);
    //    if (distanceBetweenDoors < 1) distanceBetweenDoors = 0;
    //    distanceBetweenDoors = Mathf.RoundToInt(distanceBetweenDoors);
    //    Vector3 lastPlacePos = doorA.transform.position;
    //    Vector3 currentPlacePos = new();
    //    print(distanceBetweenDoors);
    //    if(distanceBetweenDoors != 0)
    //    {
    //        for (int i = 0; i < distanceBetweenDoors; i++)
    //        {

    //            switch (direction)
    //            {
    //                case 1:
    //                    //forward
    //                    currentPlacePos = new Vector3(lastPlacePos.x, lastPlacePos.y, lastPlacePos.z + corridorTileSize);

    //                    break;
    //                case 2:
    //                    //back
    //                    currentPlacePos = new Vector3(lastPlacePos.x, lastPlacePos.y, lastPlacePos.z - corridorTileSize);


    //                    break;
    //                case 3:
    //                    //right
    //                    currentPlacePos = new Vector3(lastPlacePos.x + corridorTileSize, lastPlacePos.y, lastPlacePos.z);


    //                    break;
    //                case 4:
    //                    //left
    //                    currentPlacePos = new Vector3(lastPlacePos.x - corridorTileSize, lastPlacePos.y, lastPlacePos.z);

    //                    break;

    //            }

    //            if(!tilesHashSet.Contains(currentPlacePos))
    //            {
    //                var corridor = Instantiate(tempCorridor, currentPlacePos, doorA.transform.rotation);
    //                tilesHashSet.Add(currentPlacePos);
    //            }
    //            lastPlacePos = currentPlacePos;

    //        }
    //    }

    //}
}

