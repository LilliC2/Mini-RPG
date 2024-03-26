using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    List<GameObject> listOfRooms;
    [SerializeField] int dungeonContraints;

    [SerializeField]
    GameObject tempRoom;

    [SerializeField] int roomSize;
    
    public class DelaunayTriangle 
    {


    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        listOfRooms = new List<GameObject>();

      //  GenerateRooms(3);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) GenerateRooms(10);

    }

    void BowyerWatson(List<GameObject> pointList)
    {
        //treat each room as a point/vertex

        //add super triangle, must contain all the points in the list


    }

    void GenerateRooms(int numOfRooms)
    {
        for (int i = 0; i < numOfRooms; i++)
        {
            Vector3 randomPos = GetValidSpawnLocation(tempRoom);
            GameObject room = Instantiate(tempRoom, randomPos, Quaternion.identity);
            listOfRooms.Add(room);

        }

    }

    Vector3 GetValidSpawnLocation(GameObject newRoom)
    {
        Collider objCollider = newRoom.GetComponent<Collider>();
        Vector3 newPos = Vector3.zero;
        bool validPos = false;

        do
        {
            newPos = GeneratePosition();

            //get corners of new room
            Vector3 min = newPos - objCollider.bounds.extents;
            Vector3 max = newPos + objCollider.bounds.extents;

            //must be square!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Collider[] overlapObjs = Physics.OverlapSphere(newPos, 
                Mathf.Sqrt(Mathf.Pow(newRoom.transform.localScale.x,2) + Mathf.Pow(newRoom.transform.localScale.z, 2))/2);

            if (overlapObjs.Length == 0) validPos = true;
        }
        while (!validPos);

        return newPos;
    }


    Vector3 GeneratePosition()
    {
        Vector3 position = new Vector3(Random.Range(-dungeonContraints, dungeonContraints), 0, Random.Range(-dungeonContraints, dungeonContraints));
        return position;
    }
}
