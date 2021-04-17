using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandManager : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Check to see if RMB was pressed
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Checked if we clicked out of bounds
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            return;

        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        // Go over each selected unit and tell them to move
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            unit.GetUnitMovement().CmdMove(point);
    }
}
