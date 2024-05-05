using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogTrigger : GameBehaviour
{
    [SerializeField]
    GameObject visualCue;
    bool playerInRange = false;

    List<GameObject> playersInRange;

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
            foreach (var player in playersInRange)
            {
                var input = player.GetComponent<PlayerController>().controls;
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
            playersInRange.Add(other.gameObject);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Remove(other.gameObject);

            playerInRange = false;
        }
    }
}
