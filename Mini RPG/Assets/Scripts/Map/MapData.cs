using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public bool hasMapBeenGenerated;

    public static MapData Instance;

    public GameObject[,] mapLocations;

    public GameObject currentPartyLocation;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
