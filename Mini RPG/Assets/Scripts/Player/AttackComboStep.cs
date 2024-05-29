using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AttackComboStep : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController controller;
    float stepAmount;
    private void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        stepAmount = GetComponentInParent<PlayerController>().stepAmount;
    }

    public void Step()
    {
        controller.Move(transform.forward * stepAmount * Time.deltaTime); //step forward

    }
}
