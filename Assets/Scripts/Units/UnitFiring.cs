using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [Header("Unit Firing Fundamentals")]
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    
    [Header("Unit Firing Attributes")]
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f * 4;

    // fire timer
    private float lastFireTime;

    #region Server

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();


        // if we dont have a target
        if(target == null) { return; }

        // stop if not targetable
        if (!CanFireAtTarget()) { return; }

        // make a rotation variable to the target
        Quaternion targetRotation =
            Quaternion.LookRotation(target.transform.position - transform.position);

        // rotate every frame to the target
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // we can now shoot
        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            // a variable for reverse rotation to setup aim rotation for object of diffent sizes 
            Quaternion projectileRotation =
                Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

            // instance of the projectile
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            // spawn objects on server
            NetworkServer.Spawn(projectileInstance, connectionToClient);


            // reset fire timer
            lastFireTime = Time.time;
        }

    }

    [Server]
    private bool CanFireAtTarget()
    {
        // is in range of target
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude 
            <= fireRange * fireRange;
    } 


    #endregion

}
