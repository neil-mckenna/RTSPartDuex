using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

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


    #endregion

    /// <summary>
    /// 
    /// </summary>

    #region Client



    #endregion
}
