using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;


public class OverworldMapManager : GameBehaviour
{

    public GameObject levelRoot; //holds everything visual in the level, this is so we can keep all the level data when we additivly load another level

    Camera Camera;
    MapData mapData;
    SceneDatabase sceneDatabase;
    OverworldMapUI overworldMapUI;

    public string mapTheme; //different for each overworld map scene

    [Header("Overworld Map")]

    [SerializeField] int Columns = 4;
    [SerializeField] float Space = 2.0f;

    [SerializeField]
    GameObject[,] mapLocations;

    public GameObject currentPartyLocation;
    [SerializeField]
    GameObject partyHolder;

    private void Awake()
    {
        _GM.gameState = GameManager.GameState.OverworldMap;
        Camera = Camera.main;
        mapData = FindAnyObjectByType<MapData>();
        overworldMapUI = FindAnyObjectByType<OverworldMapUI>();
        sceneDatabase = GetComponent<SceneDatabase>();

        _GM.event_newMap.AddListener(SetPartyLocationOnLoad);
        _GM.event_ChangeActionMap.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        //check if there is map data currently saved
        if(!mapData.hasMapBeenGenerated)
        {
            GenerateMap();
        }
        else
        {
            LoadMap();
        }

    }

    // Update is called once per frame
    void Update()
    {

        //on load, generate the map
        /* 1. map layout
         *  - pick from already made layouts
         *  - generate layout at start
         *      2D array thing, where each coloumn is a column on the baord
         * 2. generate event spot types for each spot
         * 3. chose a scene that fits the event spot and map theme
         *
         *NEXT - players chose destination
         *
         * 1. find players current location on map
         * 2. find connecting locations on the map
         * 3. vote
         * 4. move to majority
         * 5. load new scene
         *
         *

        */

        if (Input.GetKeyDown(KeyCode.O)) SetPartyLocationOnLoad();

    }

    public void SelectDestination(GameObject origin, GameObject desiredLocation)
    {
        //check if locations are connected
        if(origin.GetComponent<EventSpot>().CheckConnection(desiredLocation))
        {
            print("can travel there");
        }
    }
    
    public void SetPartyLocationOnLoad()
    {
        partyHolder.transform.position = currentPartyLocation.transform.position;

        var parent =  partyHolder.transform;

        ArrangeChildren(parent);

        for (int i = 0; i < _GM.playerGameObjList.Count; i++)
        {
            _GM.playerGameObjList[i].transform.position = parent.GetChild(i).transform.position;
        }

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
    }


    private void ArrangeChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            int row = i / Columns;
            int column = i % Columns;
            parent.GetChild(i).position = new Vector2(column * Space, row * Space);
        }
    }

    /// <summary>
    /// Generate map layout
    /// </summary>
    void GenerateMap()
    {
        GameObject boss;
        _GM.mapCounter++;
        mapData.hasMapBeenGenerated = true;

        //first location is a spawn

        //get location later

        int numOfcol = Random.Range(2, 3) + 1; //max can increase as it gets harder?
        int numOfRow = Random.Range(2, 3);
        print("Num of row "+numOfRow);
        print("Num of col " +numOfcol);

        mapLocations = new GameObject[numOfRow, numOfcol+1]; //+1 account for boss

        Vector3 spawn = GameObject.Find("Spawn").gameObject.transform.position;

        mapLocations[0, 0] = Instantiate(Resources.Load<GameObject>("MapEventSpots/EventSpot_Spawn"),spawn,Quaternion.identity, levelRoot.transform);

        #region Generate Event Spots

        for (int col = 1; col < numOfcol; col++)
        {
            
            for (int row = 0; row < numOfRow; row++)
            {
                string eventSpotType = GenerateEventSpotType();
                print(eventSpotType + " at " + row + ", " + col);

                var spot = Instantiate(Resources.Load<GameObject>("MapEventSpots/EventSpot_" + eventSpotType),levelRoot.transform);
                mapLocations[row, col] = spot;
                //give it a scene based on event type and map theme


                //NOT DYNAMICS
                string fileDir = "LevelScenes/" + mapTheme + "/" + mapTheme + "_" + eventSpotType + "_" + 0;
                print(fileDir);
                Scene scene = SceneManager.GetSceneByName(mapTheme + "_" + eventSpotType + "_" + 0);//Resources.Load<SceneAsset>(fileDir);
                spot.GetComponent<EventSpot>().scene = scene;
                spot.GetComponent<EventSpot>().sceneName = mapTheme + "_" + eventSpotType + "_" + 0;
                spot.GetComponent<EventSpot>().levelName = fileDir;
                spot.GetComponent<EventSpot>().eventType = eventSpotType;

            }
        }
        #endregion

        #region Generate Boss Event Spot
        //last location is a mini boss
        if (_GM.mapCounter % 5 == 0)
        {
            print("Big Boss at 0, " + numOfcol);


            //grand boss
            mapLocations[0, numOfcol] = Instantiate(Resources.Load<GameObject>("MapEventSpots/EventSpot_BigBoss"),levelRoot.transform);
            boss = mapLocations[0, numOfcol];
        }
        else
        {
            print("Mini Boss at 0, " + (numOfcol).ToString());

            //mini boss
            mapLocations[0, numOfcol] = Instantiate(Resources.Load<GameObject>("MapEventSpots/EventSpot_MiniBoss"), levelRoot.transform);
            boss = mapLocations[0, numOfcol];

        }
        #endregion

        //generate location for events on map
        #region Place Event Spots on Map
        Vector3 lastRPos = new Vector3(0, 0, 0);

        for (int col = 1; col < numOfcol+1; col++)
        {
            //print("Moving objects in col " + col);
            GameObject toSpawn = new GameObject("Col" + col + "_Radius");
            Vector3 pos = new Vector3(0f, 0f, col * 8);
            toSpawn.transform.position = pos;

            for (int row = 0; row < numOfRow; row++)
            {
                //print("Moving object " + row + ", " + col);


                Vector3 rPos = Random.insideUnitSphere * 3 + pos;
                while(CheckDistanceBetweenOtherEventSpots(rPos, numOfcol, numOfRow))
                {
                    rPos = Random.insideUnitSphere * 3 + pos;
                    print("find enw pos");
                }


                if (mapLocations[row, col] != null)
                {
                    if (rPos.x > 0) rPos.x += Random.Range(1,3);
                    if (rPos.x < 0) rPos.x -= Random.Range(1, 3);
                    mapLocations[row, col].transform.position = new Vector3(rPos.x, 0, rPos.z);
                    lastRPos = rPos;


                }

            }

        }
        #endregion

        #region Create lines/path between Event Spots


        for (int col = 0; col < numOfcol; col++)
        {
            for (int row = 0; row < numOfRow; row++)
            {
                //get map location where lines originate from
                print("Looking at " + row + ", " + col);

                var origin = mapLocations[row, col];
                if (origin == null) break;
                print("Origin is " + origin.name + " at " + row + ", " + col);

                //find how many locations are in the next column (x)

                int numOfLocationsInNextCol = 0;

                for (int row2 = 0; row2 < numOfRow; row2++)
                {
                    if (mapLocations[row2, col + 1] != null) numOfLocationsInNextCol++;
                }
                print("There are " + numOfLocationsInNextCol + " locations in the next col");

                //add x number of line renders
                List<LineRenderer> lineRenderers = new();


                //make list of all locations in next col which dont have a connection
                List<GameObject> locationsWithNoEntry = new();

                for (int i = 0; i < numOfLocationsInNextCol; i++)
                {
                    if (mapLocations[i, col + 1].GetComponent<EventSpot>().entry != true) locationsWithNoEntry.Add(mapLocations[i, col + 1]);
                }


                //check is
                if (row == 0 && col == 0)
                {
                    if (lineRenderers.Count != numOfLocationsInNextCol)
                    {
                        for (int locationNextCol = 0; locationNextCol < numOfLocationsInNextCol; locationNextCol++)
                        {

                            print("Adding line from origin to " + mapLocations[locationNextCol, col + 1].name + " at " + row + ", " + col + 1);

                            //create obj to hold line render
                            GameObject lineHolder = Instantiate(Resources.Load<GameObject>("MapEventSpots/LineHolder"), levelRoot.transform);
                            lineHolder.transform.SetParent(origin.transform);
                            var line = lineHolder.GetComponent<LineRenderer>();
                            lineRenderers.Add(line);


                            //change point 1 of line renderer to location in next column, location 0 to origin
                            line.SetPosition(0, origin.transform.position);
                            line.SetPosition(1, mapLocations[locationNextCol, col + 1].transform.position);

                            //Add to EventSpot script
                            origin.GetComponent<EventSpot>().connections.Add(mapLocations[locationNextCol, col + 1]);

                            mapLocations[locationNextCol, col + 1].GetComponent<EventSpot>().entry = true;

                            print("Total num of line renderes is " + lineRenderers.Count);
                        }
                    }
                }
                else
                {
                    int connectionNum = Random.Range(1, numOfLocationsInNextCol);

                    for (int locationNextCol = 0; locationNextCol < connectionNum; locationNextCol++)
                    {
                        GameObject location;
                        //check if any locations dont have a connection
                        if(locationsWithNoEntry.Count != 0)
                        {
                            location = locationsWithNoEntry[Random.Range(0, locationsWithNoEntry.Count)];
                            location.GetComponent<EventSpot>().entry = true;
                            locationsWithNoEntry.Remove(location);
                        }
                        else //if not chose any
                        {
                            location = mapLocations[Random.Range(0, numOfLocationsInNextCol), col + 1];
                        }

                        print("Adding line from origin to " + mapLocations[locationNextCol, col + 1].name + " at " + row + ", " + col + 1);

                        //create obj to hold line render
                        GameObject lineHolder = Instantiate(Resources.Load<GameObject>("MapEventSpots/LineHolder"));
                        lineHolder.transform.SetParent(origin.transform);
                        var line = lineHolder.GetComponent<LineRenderer>();
                        lineRenderers.Add(line);


                        //change point 1 of line renderer to location in next column, location 0 to origin
                        line.SetPosition(0, origin.transform.position);
                        line.SetPosition(1, location.transform.position);

                        //Add to EventSpot script
                        origin.GetComponent<EventSpot>().connections.Add(location);


                        print("Total num of line renderes is " + lineRenderers.Count);

                    }

                }

                print("Num without entry = " + locationsWithNoEntry.Count);

            }
        }






        #endregion  

        Camera.GetComponent<CameraController>().CameraPan_OverworldMap(boss.transform.position);

        currentPartyLocation = mapLocations[0, 0];
        SetPartyLocationOnLoad();

        overworldMapUI.SetOptions(currentPartyLocation);

        SaveMap();
    }

    bool CheckDistanceBetweenOtherEventSpots(Vector3 origin, int colLength, int rowLength)
    {
        bool tooClose = false;

        for (int col = 1; col < colLength; col++)
        {
            for (int row = 0; row < rowLength; row++)
            {
                if (Vector3.Distance(origin, mapLocations[row, col].transform.position) < 4f) tooClose = true;
            }
        }


        return tooClose;
    }

    string GenerateEventSpotType()
    {
        string eventType = "";

        float num = Random.Range(0, 100);

        /* shop = 10%
         * rest = 10%
         * combat = 40%
         * story = 40%
         */

        if (num < 10)
        {
            eventType = "Shop";
        }
        else if (num >= 10 && num < 20)
        {
            eventType = "Rest";

        }
        else if (num >= 20 && num < 60)
        {
            eventType = "Combat";
        }
        else if (num >= 60 && num <= 100)
        {
            eventType = "Story";
        }
        else print(num);


        return eventType;
    }

    /// <summary>
    /// Save current map data using data persistance before travelling to new scene
    /// </summary>
    public void SaveMap()
    {
        print("Saved Map");
        mapData.mapLocations = mapLocations;
        mapData.currentPartyLocation = currentPartyLocation;


    }


    /// <summary>
    /// Load map based on saved data
    /// </summary>
    void LoadMap()
    {
        print("Load Map");
        //load map
        foreach (var item in mapData.mapLocations)
        {
            print(item.name);
        }

        //move players to correct location
    }
}
