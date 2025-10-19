using UnityEngine;

public class SoundUpgrade : Upgrade
{
    [Header("Sound Settings")]
    public AudioSource clickSound; // Drag an AudioSource component here that has the click sound
    
    void Start()
    {
        upgradeName = "Sound Upgrade";
        cost = 15;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
        
        // If no AudioSource is assigned, try to get one from this GameObject
        if (clickSound == null)
        {
            clickSound = GetComponent<AudioSource>();
        }
        
        // If we have an AudioSource, set it up
        if (clickSound != null)
        {
            clickSound.playOnAwake = false;
        }
        else
        {
            Debug.LogWarning("SoundUpgrade: No AudioSource component found or assigned.");
        }
    }
    
    protected override void ApplyUpgrade()
    {
        Debug.Log("Sound Upgrade purchased! New click sound enabled.");
        
        // Register this upgrade with BrainClicker
        if (brainClicker != null)
        {
            // Make sure BrainClicker has the sound upgrades list
            if (!BrainClicker.soundUpgrades.Contains(this))
            {
                BrainClicker.soundUpgrades.Add(this);
            }
        }
    }
    
    // Public method that BrainClicker will call
    public void PlayClickSound()
    {
        if (clickSound != null && clickSound.clip != null)
        {
            // No cooldown - play sound for every click
            clickSound.PlayOneShot(clickSound.clip);
        }
    }
    
    // Clean up when destroyed
    void OnDestroy()
    {
        if (BrainClicker.soundUpgrades.Contains(this))
        {
            BrainClicker.soundUpgrades.Remove(this);
        }
    }
}