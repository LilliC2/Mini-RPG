using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : GameBehaviour
{
    MapData mapData;

    // Start is called before the first frame update
    void Start()
    {
        mapData = FindAnyObjectByType<MapData>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            foreach (var item in mapData.mapLocations)
            {
                print(item.name);
            }
            //_UI.LoadScene("Map");
        }
    }
}
