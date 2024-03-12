using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OverworldMapUI : GameBehaviour
{
    OverworldMapManager overworldMapManager;

    [SerializeField]
    GameObject[] optionUIGO;

    List<GameObject> eventSpotOptions;

    GameObject selectedOptionGO;

    int optionCountMax;
    bool buttonCooldown;

    int[] optionSelectedByPlayer;

    int[] optionVoteCount;
    bool[] hasPlayerVoted;


    [SerializeField] GameObject[] P1_icons;
    [SerializeField] GameObject[] P2_icons;
    [SerializeField] GameObject[] P3_icons;
    [SerializeField] GameObject[] P4_icons;


    [SerializeField] Sprite combatImage;
    [SerializeField] Sprite storyImage;
    [SerializeField] Sprite restImage;
    [SerializeField] Sprite shopImage;
    [SerializeField] Sprite miniBossImage;
    [SerializeField] Sprite bigBossImage;

    // Start is called before the first frame update
    void Start()
    {
        overworldMapManager = FindAnyObjectByType<OverworldMapManager>();
        optionSelectedByPlayer = new int[_GM.playerGameObjList.Count];
        hasPlayerVoted = new bool[_GM.playerGameObjList.Count];

        //Turn off cursors
        var mice = GameObject.FindGameObjectsWithTag("Cursor");
        foreach (var cursor in mice)
        {
            cursor.SetActive(false);
        }
    }

    public void ChangePlayerOptionChoice(int direction, int playerNum)
    {
        int optionMax = optionCountMax - 1; //to include 0 in count

        //check if player has not confirmed their vote
        if (!hasPlayerVoted[playerNum])
        {
            if (!buttonCooldown)
            {
                buttonCooldown = true;
                ExecuteAfterSeconds(0.5f, () => buttonCooldown = false);

                optionSelectedByPlayer[playerNum] += direction;

                if (optionSelectedByPlayer[playerNum] > optionMax) { optionSelectedByPlayer[playerNum] = 0; }
                if (optionSelectedByPlayer[playerNum] < 0) { optionSelectedByPlayer[playerNum] = optionMax; }

                //change player icon image
                ChangePlayerIconPos(playerNum);
            }
        }

      
    }

    void ChangePlayerIconPos(int playerNum)
    {
        switch (playerNum)
        {
            case 0:
                switch (optionSelectedByPlayer[playerNum])
                {
                    case 0:

                        P1_icons[0].SetActive(true);
                        P1_icons[1].SetActive(false);
                        P1_icons[2].SetActive(false);
                        P1_icons[3].SetActive(false);


                        break;
                    case 1:
                        P1_icons[0].SetActive(false);
                        P1_icons[1].SetActive(true);
                        P1_icons[2].SetActive(false);
                        P1_icons[3].SetActive(false);

                        break;

                    case 2:
                        P1_icons[0].SetActive(false);
                        P1_icons[1].SetActive(false);
                        P1_icons[2].SetActive(true);
                        P1_icons[3].SetActive(false);

                        break;

                    case 3:
                        P1_icons[0].SetActive(false);
                        P1_icons[1].SetActive(false);
                        P1_icons[2].SetActive(false);
                        P1_icons[3].SetActive(true);

                        break;
                }
                break;
            case 1:
                switch (optionSelectedByPlayer[playerNum])
                {
                    case 0:

                        P2_icons[0].SetActive(true);
                        P2_icons[1].SetActive(false);
                        P2_icons[2].SetActive(false);
                        P2_icons[3].SetActive(false);


                        break;
                    case 1:
                        P2_icons[0].SetActive(false);
                        P2_icons[1].SetActive(true);
                        P2_icons[2].SetActive(false);
                        P2_icons[3].SetActive(false);

                        break;

                    case 2:
                        P2_icons[0].SetActive(false);
                        P2_icons[1].SetActive(false);
                        P2_icons[2].SetActive(true);
                        P2_icons[3].SetActive(false);

                        break;

                    case 3:
                        P2_icons[0].SetActive(false);
                        P2_icons[1].SetActive(false);
                        P2_icons[2].SetActive(false);
                        P2_icons[3].SetActive(true);

                        break;
                }
                break;
            
            case 2:
                switch (optionSelectedByPlayer[playerNum])
                {
                    case 0:

                        P3_icons[0].SetActive(true);
                        P3_icons[1].SetActive(false);
                        P3_icons[2].SetActive(false);
                        P3_icons[3].SetActive(false);


                        break;
                    case 1:
                        P3_icons[0].SetActive(false);
                        P3_icons[1].SetActive(true);
                        P3_icons[2].SetActive(false);
                        P3_icons[3].SetActive(false);

                        break;

                    case 2:
                        P3_icons[0].SetActive(false);
                        P3_icons[1].SetActive(false);
                        P3_icons[2].SetActive(true);
                        P3_icons[3].SetActive(false);

                        break;

                    case 3:
                        P3_icons[0].SetActive(false);
                        P3_icons[1].SetActive(false);
                        P3_icons[2].SetActive(false);
                        P3_icons[3].SetActive(true);

                        break;
                }
                break;
            case 3:
                switch (optionSelectedByPlayer[playerNum])
                {
                    case 0:

                        P4_icons[0].SetActive(true);
                        P4_icons[1].SetActive(false);
                        P4_icons[2].SetActive(false);
                        P4_icons[3].SetActive(false);


                        break;
                    case 1:
                        P4_icons[0].SetActive(false);
                        P4_icons[1].SetActive(true);
                        P4_icons[2].SetActive(false);
                        P4_icons[3].SetActive(false);

                        break;

                    case 2:
                        P4_icons[0].SetActive(false);
                        P4_icons[1].SetActive(false);
                        P4_icons[2].SetActive(true);
                        P4_icons[3].SetActive(false);

                        break;

                    case 3:
                        P4_icons[0].SetActive(false);
                        P4_icons[1].SetActive(false);
                        P4_icons[2].SetActive(false);
                        P4_icons[3].SetActive(true);

                        break;
                }
                break;
        }

        

    }

    public void ConfirmPlayerOptionChoice(int playerNum)
    {
        if(!hasPlayerVoted[playerNum])
        {
            //add vote to player
            hasPlayerVoted[playerNum] = true;

            //change icon to show they have selected option
            switch (playerNum)
            {
                case 0:
                    P1_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.yellow;
                    break;
                case 1:
                    P2_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.yellow;
                    break;
                case 2:
                    P3_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.yellow;
                    break;
                case 3:
                    P4_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.yellow;
                    break;
            }

            //add vote to tally
            if (optionVoteCount[optionSelectedByPlayer[playerNum]] < 4)
                optionVoteCount[optionSelectedByPlayer[playerNum]]++;

            print("Option " + optionSelectedByPlayer[playerNum] + " votes = " + optionVoteCount[optionSelectedByPlayer[playerNum]]);

            CheckIfAllPlayersHaveVoted();
        }
        
    }

    public void RevokePlayerOptionChoice(int playerNum)
    {
        if(hasPlayerVoted[playerNum])
        {
            //change icon to show they have selected option
            switch (playerNum)
            {
                case 0:
                    P1_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.white;
                    break;
                case 1:
                    P2_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.white;
                    break;
                case 2:
                    P3_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.white;
                    break;
                case 3:
                    P4_icons[optionSelectedByPlayer[playerNum]].GetComponent<Image>().color = Color.white;
                    break;
            }

            //add vote to tally
            if(optionVoteCount[optionSelectedByPlayer[playerNum]] != 0) optionVoteCount[optionSelectedByPlayer[playerNum]]--;
            print("Option " + optionSelectedByPlayer[playerNum] + " votes = " + optionVoteCount[optionSelectedByPlayer[playerNum]]);

            hasPlayerVoted[playerNum] = false;
        }
    }

    public void SetOptions(GameObject origin)
    {
        //get connections from origin
        var connections = origin.GetComponent<EventSpot>().connections;
        eventSpotOptions = connections;
        optionVoteCount = new int[connections.Count];

        optionCountMax = connections.Count;
        print("Option count max = " + optionCountMax);


        for (int i = 0; i < connections.Count; i++)
        {
            optionUIGO[i].SetActive(true);

            var eventSpotData = connections[i].GetComponent<EventSpot>();
            string sceneName = eventSpotData.levelName;

            optionUIGO[i].GetComponentInChildren<TMP_Text>().text = sceneName;

            var image = optionUIGO[i].transform.Find("EventSpotTypeImage").GetComponent<Image>();
            //get event type
            if (sceneName.Contains("Combat")) image.sprite = combatImage;
            else if (sceneName.Contains("Story")) image.sprite = storyImage;
            else if (sceneName.Contains("Rest")) image.sprite = restImage;
            else if (sceneName.Contains("Shop")) image.sprite = shopImage;
            else if (sceneName.Contains("MiniBoss")) image.sprite = miniBossImage;
            else if (sceneName.Contains("BigBoss")) image.sprite = bigBossImage;
        }
    }
  
    void CheckIfAllPlayersHaveVoted()
    {

        int votedPlayerCount = 0;

        foreach (var boolean in hasPlayerVoted)
        {
            if (boolean == true) votedPlayerCount++;
        }

        List<int> highestVoteOptions = new List<int>();

        //all players have voted
        if (votedPlayerCount == _GM.playerGameObjList.Count)
        {
            int highestVote = -1;

            for (int i = 0; i < optionVoteCount.Length; i++)
            {
                if (optionVoteCount[i] > highestVote)
                {
                    //clear tied options
                    highestVoteOptions.Clear();

                    highestVote = optionVoteCount[i];
                    highestVoteOptions.Add(i);
                }
                else if (optionVoteCount[i] == highestVote)
                {
                    highestVoteOptions.Add(i);

                }
            }


            //one winner
            if (highestVoteOptions.Count == 1)
            {
                selectedOptionGO = eventSpotOptions[highestVoteOptions[0]];
            }
            else
            {
                selectedOptionGO = eventSpotOptions[Random.Range(0,highestVoteOptions.Count)];
            }
            print("Winner is " + selectedOptionGO.name);

            string sceneName = selectedOptionGO.GetComponent<EventSpot>().sceneName;

            //change current party location on map
            overworldMapManager.currentPartyLocation = selectedOptionGO;
            overworldMapManager.SaveMap();

            StartCoroutine(LoadSceneAdditvly(sceneName));
            

        }
        else print("Not evreyone has voted");



    }

    IEnumerator LoadSceneAdditvly(string scene)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        while (!asyncOperation.isDone)
        {
            yield return null;

        }
        if(asyncOperation.isDone)
        {
            print("change active scene");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
            overworldMapManager.levelRoot.SetActive(false);
        }

        //var currentScene = SceneManager.GetActiveScene();

        //SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        //SceneManager.SetActiveScene()

    }

}
