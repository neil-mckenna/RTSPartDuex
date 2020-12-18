using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
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

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
