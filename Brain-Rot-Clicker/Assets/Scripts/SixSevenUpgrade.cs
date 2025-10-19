using UnityEngine;

public class SixSevenUpgrade : Upgrade
{
    [Header("Visual Object")]
    public GameObject objectToShow; // Drag the object you want to make visible here
    
    void Start()
    {
        upgradeName = "Six Seven";
        cost = 67;
    }
    
    protected override void ApplyUpgrade()
    {
        if (brainClicker != null)
        {
            brainClicker.AddPassiveEvery7Seconds(6);
            Debug.Log("Six Seven upgrade purchased! Adds 6 brain rot every 7 seconds");
            
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