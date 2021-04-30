using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    // We created the health bar, start listening for health updates
    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    // We destroyed the health bar, stop listening for health updates
    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    // Show the health bar when hovering over the object
    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }

    // Hide the health bar when hovering over the object
    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }

    // Perform health bar updates
    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = ((float)currentHealth / maxHealth);
    }
}
