using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

// Manage Unit movement for setting destination, clearing path, and chasing targeted enemy units.

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 3f;

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        // Stop the units from moving after the game is over
        agent.ResetPath();
    }

    // Use ServerCallback to prevent clients from accessing the function
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();

        // Check if we have a target
        if (target != null)
        {
            // If we need to get in range of target
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }

            return;
        }

        // This will prevent units we didn't tell to move from blocking
        if (!agent.hasPath)
            return;

        // If the units remaining distance is close enough, stop the movement.
        if (agent.remainingDistance > agent.stoppingDistance)
            return;

        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();

        // Is the position valid? If not, don't do anything!
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            return;

        // ???
        agent.SetDestination(hit.position);
    }

    #endregion


}
