using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
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

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collision is with something that has a network identity?
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdenity))
        {
            if (networkIdenity.connectionToClient == connectionToClient)
                return;

            // If our projectile collides with a target that is not our own, deal damage
            if (other.TryGetComponent<Health>(out Health health))
                health.DealDamage(damageToDeal);

            // Destroy the projectile
            DestroySelf();
        }
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
