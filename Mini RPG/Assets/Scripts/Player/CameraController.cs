using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CameraController : GameBehaviour
{
    public enum CameraState { Combat, Dialog, WorldMap}
    CameraState cameraState;
    [SerializeField]
    Ease overworldCamEase;
    bool overworldCameraEase = false;

    [SerializeField] float zAdjustments;
    [SerializeField] float xAdjustments;
    [SerializeField] float yAdjustments;

    // Start is called before the first frame update
    void Start()
    {
        ChangeCameraState();
       // if (cameraState == CameraState.WorldMap) //gameObject.transform.localRotation = new Quaternion(30, 0, 0,0);
    }

    void ChangeCameraState()
    {
        var currentScene = SceneManager.GetActiveScene().name;

        if (currentScene.Contains("Map")) cameraState = CameraState.WorldMap;
        if (currentScene.Contains("Combat")) cameraState = CameraState.Combat;
        if (currentScene.Contains("Story")) cameraState = CameraState.Dialog;

    }

    public void CameraPan_OverworldMap(Vector3 bossLocation)
    {
        Vector3 startPos = transform.position;

        transform.position = new Vector3(0, 3, bossLocation.z-1);
        transform.DOMove(startPos, 5).SetEase(overworldCamEase);
        ExecuteAfterSeconds(5, () => overworldCameraEase = true);
    }

    // Update is called once per frame
    void Update()
    {
        switch(cameraState)
        {
            case CameraState.Combat:

                if(_GM.playerGameObjList.Count != 0)
                {
                    Vector3 position = new();

                    foreach (var item in _GM.playerGameObjList)
                    {
                        position += item.transform.position;
                    }

                    position /= _GM.playerGameObjList.Count;

                    gameObject.transform.position = new Vector3(position.x + xAdjustments, yAdjustments, position.z + zAdjustments);

                    //check if players can be seen by camera
                }



                break;
            case CameraState.WorldMap:

                if(overworldCameraEase)
                {

                }

                break;
        }
    }
}
