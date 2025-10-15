using UnityEngine;

public class SixSevenUpgrade : Upgrade
{
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
        }
    }
}