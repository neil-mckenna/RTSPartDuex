using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 25;
    [SerializeField] private float projectileLifetime = 5f;
    [SerializeField] private float launchForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * launchForce;
        
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), projectileLifetime);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        // dont trigger friendly fire
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            
            if(networkIdentity.connectionToClient == connectionToClient) { return; }

        }

        // Damage enemies
        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }

        // destroy the projectile
        DestroySelf();
        
    }


    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
