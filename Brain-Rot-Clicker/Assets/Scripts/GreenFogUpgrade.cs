using UnityEngine;

public class GreenFogUpgrade : Upgrade
{
    public ParticleSystem greenFogParticles;
    //private bool isActive = false;
    
    void Start()
    {
        upgradeName = "Green Fog";
        cost = 5;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
    }
    
    protected override void ApplyUpgrade()
    {
        //isActive = true;
        
        // Add 1 to passive BPS - this will make BrainClicker add 1 brain rot every second
        if (brainClicker != null)
        {
            brainClicker.AddPassiveBPS(1);
        }
        
        if (greenFogParticles != null)
        {
            greenFogParticles.Play();
        }
        
        Debug.Log("Green Fog upgrade purchased! Producing 1 brain rot per second...");
    }
}