using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public BrainClicker brainClicker;
    
    // List to track all upgrades in the game
    private List<Upgrade> allUpgrades = new List<Upgrade>();
    
    void Start()
    {
        // Find all upgrade components and register them
        Upgrade[] foundUpgrades = FindObjectsOfType<Upgrade>();
        allUpgrades.AddRange(foundUpgrades);
        
        // Initialize all upgrades
        foreach (Upgrade upgrade in allUpgrades)
        {
            upgrade.Initialize(brainClicker);
            upgrade.gameObject.SetActive(false); // Hide initially
        }
    }
    
    void Update()
    {
        // Update visibility for all upgrades based on affordability
        foreach (Upgrade upgrade in allUpgrades)
        {
            if (upgrade != null)
            {
                upgrade.UpdateButtonVisibility();
            }
        }
    }
}