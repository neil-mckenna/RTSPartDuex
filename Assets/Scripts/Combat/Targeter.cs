using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if(!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        target = newTarget;

    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    // server method

    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    #endregion

}
