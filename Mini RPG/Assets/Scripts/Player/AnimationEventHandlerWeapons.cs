using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AnimationEventHandlerWeapons : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController controller;
    PlayerController player;
    Animator animator;
    float stepAmount;
    Rigidbody rb;
    [SerializeField]
    private float strength = 16, delay = 0.15f;

    private void OnEnable()
    {
        controller = GetComponentInParent<CharacterController>();
        rb = GetComponentInParent<Rigidbody>();
        animator = GetComponent<Animator>();
        stepAmount = GetComponentInParent<PlayerController>().stepAmount;
        player = GetComponentInParent<PlayerController>();
    }

    public void AttackMovement()
    {

        StopAllCoroutines();

        Vector3 direction = (transform.position - transform.forward).normalized;

        rb.AddForce(direction * strength, ForceMode.Impulse);

        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


    public void ComboIncease()
    {
        player.currentAttackCount++;

        if (player.currentAttackCount > 2) player.currentAttackCount = 0;
        animator.SetInteger("AttackCounter", player.currentAttackCount);

    }
}
