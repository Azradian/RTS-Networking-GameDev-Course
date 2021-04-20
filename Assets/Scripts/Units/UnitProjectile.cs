using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float force = 10f;

    private void Start()
    {
        rb.velocity = transform.forward * force;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), lifetime);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
