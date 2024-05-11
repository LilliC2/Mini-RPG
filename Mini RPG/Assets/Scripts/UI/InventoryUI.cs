using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : GameBehaviour
{
    [SerializeField] GameObject inventoryGO;

    public void OpenInventory()
    {
        //find which player is opening UI
        inventoryGO.SetActive(true);



    }
}
