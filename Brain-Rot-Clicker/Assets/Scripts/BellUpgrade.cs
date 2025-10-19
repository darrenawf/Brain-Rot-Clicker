using UnityEngine;

public class BellUpgrade : Upgrade
{
    [Header("Bell Settings")]
    public AudioSource bellSound; // Drag an AudioSource for the bell sound here
    
    private int bellAmount = 40;
    
    void Start()
    {
        upgradeName = "Bell Upgrade";
        cost = 500;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
        
        // Set up bell sound if assigned
        if (bellSound != null)
        {
            bellSound.playOnAwake = false;
        }
    }
    
    protected override void ApplyUpgrade()
    {
        Debug.Log("Bell Upgrade purchased! +40 brain rot every 10 seconds with bell sound");
        
        // Add +20 brain rot every 10 seconds
        if (brainClicker != null)
        {
            brainClicker.AddPassiveEvery10Seconds(bellAmount);
            
            // Register this upgrade to play bell sound every 10 seconds
            brainClicker.RegisterBellUpgrade(this);
        }
    }
    
    // Public method that BrainClicker will call every 10 seconds
    public void PlayBellSound()
    {
        if (bellSound != null && bellSound.clip != null)
        {
            bellSound.PlayOneShot(bellSound.clip);
        }
    }
}