using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : GameBehaviour
{
    public enum CameraState { Combat, Dialog}
    CameraState cameraState;

    [SerializeField] float zAdjustments;
    [SerializeField] float xAdjustments;
    [SerializeField] float yAdjustments;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(cameraState)
        {
            case CameraState.Combat:

                Vector3 position = new();

                foreach (var item in _GM.playerGameObjList)
                {
                    position += item.transform.position;
                }

                position /= _GM.playerGameObjList.Count;

                gameObject.transform.position = new Vector3(position.x+ xAdjustments, yAdjustments, position.z+ zAdjustments);

                //check if players can be seen by camera


                break;
        }
    }
}
