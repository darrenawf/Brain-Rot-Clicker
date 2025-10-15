using UnityEngine;

public class SixSevenUpgrade : Upgrade
{
    private bool isActive = false;
    
    void Start()
    {
        upgradeName = "Six Seven";
        cost = 67;
    }
    
    void Update()
    {
        if (isActive && brainClicker != null)
        {
            // 1/67 chance each frame to trigger the effect
            if (Random.Range(1, 68) == 67)
            {
                int brainRotGained = Random.Range(6, 8); // Randomly 6 or 7
                brainClicker.brainRotCount += brainRotGained;
                brainClicker.lifetimeBrainRot += brainRotGained;
                Debug.Log($"SIX SEVEN! Gained {brainRotGained} brain rot!");
            }
        }
    }
    
    protected override void ApplyUpgrade()
    {
        isActive = true;
        Debug.Log("Six Seven upgrade purchased! 1/67 chance to gain 6-7 brain rot");
    }
}