using UnityEngine;

public class SixSevenUpgrade : Upgrade
{
    [Header("Visual Object")]
    public GameObject objectToShow; // Drag the object you want to make visible here
    
    [Header("Sound Settings")]
    public AudioSource bonusSound; // Drag an AudioSource for the +67 bonus sound here
    
    private int fixedAmount = 67;
    private float chance = 0.02f; // 2% chance
    
    void Start()
    {
        upgradeName = "Six Seven";
        cost = 67;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
        
        // Set up bonus sound if assigned
        if (bonusSound != null)
        {
            bonusSound.playOnAwake = false;
        }
    }
    
    protected override void ApplyUpgrade()
    {
        if (brainClicker != null)
        {
            // Add the chance-based fixed click amount to BrainClicker
            brainClicker.AddChanceFixedClick(fixedAmount, chance, bonusSound);
            Debug.Log("Six Seven upgrade purchased! 2% chance for fixed +67 brain rot per click");
            
            // Make the object visible when upgrade is purchased
            if (objectToShow != null)
            {
                objectToShow.SetActive(true);
            }
            
            // Play the bonus sound when the upgrade is purchased
            if (bonusSound != null && bonusSound.clip != null)
            {
                bonusSound.PlayOneShot(bonusSound.clip);
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