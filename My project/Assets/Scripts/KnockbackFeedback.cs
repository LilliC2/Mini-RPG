using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class KnockbackFeedback : MonoBehaviour
{

    [SerializeField]
    private float strength = 16, delay = 0.15f;

    public UnityEvent OnBegin, OnDone;

    [SerializeField]
    Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PlayFeedback(GameObject sender)
    {

        print(sender.name);
        StopAllCoroutines();
        OnBegin?.Invoke();

        Vector3 direction = (transform.position - sender.transform.position).normalized;

        rb.AddForce(direction * strength, ForceMode.Impulse);

        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        OnDone?.Invoke();
    }

}
