using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public event Action ServerOnDie;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        // stops taking damage when dead
        if(currentHealth == 0) { return; }

        // return highest number between damaged health and and 0, this stop health below 0
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if(currentHealth != 0) { return; }

        // raise dead event
        ServerOnDie?.Invoke();

        //
        Debug.LogWarning("I died");

    }

    #endregion

    ///


    #region Client

    #endregion

}
