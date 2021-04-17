using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will be used similar to a tag for objects that are tergetable

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
