﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;


    // server actions
    public event Action ServerOnDie;


    // client actions
    public event Action<int, int> ClientOnHealthUpdated; 



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

        // Kill stuff
        Debug.LogWarning("I died");

    }

    #endregion

    ///


    #region Client

    private void HandleHealthUpdated(int oldhealth, int newhealth)
    {
        ClientOnHealthUpdated?.Invoke(newhealth, maxHealth);
    }


    #endregion

}