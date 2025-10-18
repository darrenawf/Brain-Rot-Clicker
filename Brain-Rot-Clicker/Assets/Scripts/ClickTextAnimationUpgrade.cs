using UnityEngine;

public class ClickTextAnimationUpgrade : Upgrade
{
    [Header("Click Animation Settings")]
    public GameObject clickTextPrefab; // Reference to the ClickTextAnimation prefab
    
    void Start()
    {
        upgradeName = "Visual Feedback";
        cost = 25;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
        
        // Disable the click animation in BrainClicker initially
        if (brainClicker != null && clickTextPrefab != null)
        {
            brainClicker.clickTextPrefab = null; // Start with no animation
        }
    }
    
    protected override void ApplyUpgrade()
    {
        // Enable the click animation in BrainClicker
        if (brainClicker != null && clickTextPrefab != null)
        {
            brainClicker.clickTextPrefab = clickTextPrefab;
        }
        
        Debug.Log("Visual Feedback upgrade purchased! Click animations are now enabled.");
    }
}