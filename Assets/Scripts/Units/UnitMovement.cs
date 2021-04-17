using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    #region Server

    [Command]
    public void CmdMove(Vector3 position)
    {
        // Is the position valid? If not, don't do anything!
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            return;

        // ???
        agent.SetDestination(hit.position);
    }

    #endregion


}
