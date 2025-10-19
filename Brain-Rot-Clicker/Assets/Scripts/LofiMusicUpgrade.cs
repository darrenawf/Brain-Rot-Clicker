using UnityEngine;

public class LofiMusicUpgrade : Upgrade
{
    [Header("Lofi Music Settings")]
    public AudioSource lofiMusic; // Drag an AudioSource component with lofi music here
    
    void Start()
    {
        upgradeName = "Lofi Music";
        cost = 400;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
        
        // If no AudioSource is assigned, try to get one from this GameObject
        if (lofiMusic == null)
        {
            lofiMusic = GetComponent<AudioSource>();
        }
        
        // If we have an AudioSource, set it up
        if (lofiMusic != null)
        {
            lofiMusic.playOnAwake = false;
            lofiMusic.loop = true;
            lofiMusic.Stop();
            
            // Preload the audio data to prevent lag when first played
            if (lofiMusic.clip != null && !lofiMusic.clip.preloadAudioData)
            {
                lofiMusic.clip.LoadAudioData();
            }
        }
        else
        {
            Debug.LogWarning("LofiMusicUpgrade: No AudioSource component found or assigned.");
        }
    }
    
    protected override void ApplyUpgrade()
    {
        Debug.Log("Lofi Music upgrade purchased! Chill beats activated. +3 passive BPS");
        
        // Add +3 passive brain rot per second
        if (brainClicker != null)
        {
            brainClicker.AddPassiveBPS(3);
        }
        
        // Start playing the lofi music
        if (lofiMusic != null && lofiMusic.clip != null)
        {
            lofiMusic.Play();
        }
        else
        {
            Debug.LogWarning("LofiMusicUpgrade: No audio clip assigned to play.");
        }
    }
}