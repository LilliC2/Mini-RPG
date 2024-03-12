using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneData
{
    public string levelName;
    public string eventType;
    public PhysicsScene scene;
}

public class SceneDatabase : GameBehaviour
{
    [Header("Forest")]
    public SceneData[] forest_levels;
    

}
