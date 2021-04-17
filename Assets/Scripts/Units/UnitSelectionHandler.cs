using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private RectTransform unitSelectionArea = null;

    private Vector2 startPosition;
    private RTSPlayer player;
    private Camera mainCamera;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;
        //player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }

    private void Update()
    {
        // Temp fix until lobby
        if (player == null && NetworkClient.connection != null)
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        // Drag and select a group of units
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        // We are done selecting our units. Get rid of the box!
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                return;

            // If the target is NOT a unit, get out
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
                return;

            // If it hit a unit but its not ours... get out
            if (!unit.hasAuthority)
                return;

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
                selectedUnit.Select();

            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in player.GetPlayerUnits())
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x > min.x &&
                screenPosition.x < max.x && 
                screenPosition.y > min.y && 
                screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }


    private void StartSelectionArea()
    {
        // Start selection area
        foreach (Unit selectedUnit in SelectedUnits)
            selectedUnit.Deselect();

        SelectedUnits.Clear();

        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        // Get the absolute value of the areaWidth and areaHeight
        unitSelectionArea.sizeDelta = new Vector2(Math.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);

    }
}
