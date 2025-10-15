using UnityEngine;
using TMPro;

public class BPSVisibilityUpgrade : Upgrade
{
    public TextMeshProUGUI bpsText;
    
    void Start()
    {
        upgradeName = "Show BPS";
        cost = 10;
    }
    
    protected override void ApplyUpgrade()
    {
        if (bpsText != null)
        {
            bpsText.gameObject.SetActive(true);
        }
        Debug.Log("BPS counter is now visible!");
    }
}