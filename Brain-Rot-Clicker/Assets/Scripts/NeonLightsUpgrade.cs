using UnityEngine;

public class NeonLightsUpgrade : Upgrade
{
    [Header("Neon Lights Settings")]
    public GameObject neonLightsObject; // Reference to your neon lights GameObject
    public int passiveBPSBonus = 3;
    
    void Start()
    {
        upgradeName = "Neon Lights";
        cost = 100;
        
        // Hide neon lights initially if not purchased
        if (neonLightsObject != null && !isPurchased)
        {
            neonLightsObject.SetActive(false);
        }
    }
    
    protected override void ApplyUpgrade()
    {
        // Show the neon lights
        if (neonLightsObject != null)
        {
            neonLightsObject.SetActive(true);
        }
        
        // Add passive BPS bonus
        if (brainClicker != null)
        {
            brainClicker.AddPassiveBPS(passiveBPSBonus);
        }
        
        Debug.Log("Neon Lights unlocked! +" + passiveBPSBonus + " passive BPS");
    }
}