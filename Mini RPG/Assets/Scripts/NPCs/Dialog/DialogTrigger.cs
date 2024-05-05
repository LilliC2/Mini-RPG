using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogTrigger : GameBehaviour
{
    [SerializeField]
    GameObject visualCue;
    bool playerInRange = false;

    [SerializeField] TextAsset inkJSON;

    // Start is called before the first frame update
    void Start()
    {
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if(playerInRange)
        {
            var collidersInRange = Physics.OverlapSphere(gameObject.transform.position, 4, _GM.playerMask);
            foreach (var player in collidersInRange)
            {
                var input = player.gameObject.GetComponent<PlayerController>().controls;
                if(input.Gameplay.Interact.IsPressed())
                {
                    print("PRESSED");
                }
            }
        }
        visualCue.SetActive(playerInRange);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
