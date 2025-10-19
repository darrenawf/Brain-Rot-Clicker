using UnityEngine;

public class ReactionUpgrade : Upgrade
{
    private int clickBonus = 2;
    
    void Start()
    {
        upgradeName = "Reaction Upgrade";
        cost = 250;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
    }
    
    protected override void ApplyUpgrade()
    {
        // Increase the click multiplier in BrainClicker
        if (brainClicker != null)
        {
            brainClicker.IncreaseClickMultiplier(clickBonus);
            Debug.Log("Reaction Upgrade purchased! +" + clickBonus + " brain rot per click! Total: +" + brainClicker.GetClickMultiplier() + " per click");
        }
    }
}