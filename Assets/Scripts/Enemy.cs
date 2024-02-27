using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Action OnVictory;

    Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Projectile"))
            return;

        //TODO victory

        Victory();
    }


    private void FixedUpdate()
    {
        if (body.IsSleeping())
            body.WakeUp();
    }

    void Victory()
    {
        OnVictory?.Invoke();
    }
}
