using UnityEngine;

public class SixSevenUpgrade : Upgrade
{
    [Header("Visual Object")]
    public GameObject objectToShow; // Drag the object you want to make visible here
    
    private int fixedAmount = 67;
    private float chance = 0.03f; // 1.5% chance
    
    void Start()
    {
        upgradeName = "Six Seven";
        cost = 67;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
    }
    
    protected override void ApplyUpgrade()
    {
        if (brainClicker != null)
        {
            // Add the chance-based fixed click amount to BrainClicker
            brainClicker.AddChanceFixedClick(fixedAmount, chance);
            Debug.Log("Six Seven upgrade purchased! 3% chance for fixed +67 brain rot per click");
            
            // Make the object visible when upgrade is purchased
            if (objectToShow != null)
            {
                objectToShow.SetActive(true);
            }
        }
    }
    
    // Optional: Hide the object at start if it shouldn't be visible initially
    void OnEnable()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(false);
        }
    }
}