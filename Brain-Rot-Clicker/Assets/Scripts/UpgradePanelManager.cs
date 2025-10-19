using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradePanelManager : MonoBehaviour
{
    [Header("Panel Settings")]
    public Transform upgradesPanel;
    public Vector2 firstButtonPosition = new Vector2(0, 0);
    public Vector2 buttonSize = new Vector2(150, 150);
    public float buttonSpacing = 200f;
    public int maxVisibleUpgrades = 5;
    
    private List<Upgrade> allUpgrades = new List<Upgrade>();
    private List<GameObject> activeUpgradeButtons = new List<GameObject>();
    private BrainClicker brainClicker;
    
    void Start()
    {
        brainClicker = FindObjectOfType<BrainClicker>();
        FindAllUpgrades();
        
        // Start with all upgrades hidden
        foreach (Upgrade upgrade in allUpgrades)
        {
            upgrade.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        UpdateUpgradeDisplay();
    }
    
    void FindAllUpgrades()
    {
        Upgrade[] foundUpgrades = FindObjectsOfType<Upgrade>(true); // Include inactive
        allUpgrades.AddRange(foundUpgrades);
        
        foreach (Upgrade upgrade in allUpgrades)
        {
            upgrade.Initialize(brainClicker);
        }
    }
    
    void UpdateUpgradeDisplay()
    {
        // Clear current active buttons
        foreach (GameObject button in activeUpgradeButtons)
        {
            if (button != null)
                button.SetActive(false);
        }
        activeUpgradeButtons.Clear();
        
        // Get affordable upgrades that haven't been purchased yet
        List<Upgrade> affordableUpgrades = GetAffordableUpgrades();
        
        // Sort upgrades by cost (cheapest first)
        affordableUpgrades = affordableUpgrades.OrderBy(upgrade => upgrade.cost).ToList();
        
        // Show only up to maxVisibleUpgrades
        int upgradesToShow = Mathf.Min(affordableUpgrades.Count, maxVisibleUpgrades);
        
        for (int i = 0; i < upgradesToShow; i++)
        {
            Upgrade upgrade = affordableUpgrades[i];
            if (!upgrade.IsPurchased())
            {
                upgrade.gameObject.SetActive(true);
                PositionUpgradeButton(upgrade.gameObject, i, upgradesToShow);
                activeUpgradeButtons.Add(upgrade.gameObject);
            }
        }
    }
    
    List<Upgrade> GetAffordableUpgrades()
    {
        List<Upgrade> affordable = new List<Upgrade>();
        
        foreach (Upgrade upgrade in allUpgrades)
        {
            if (brainClicker.lifetimeBrainRot >= upgrade.cost && !upgrade.IsPurchased())
            {
                affordable.Add(upgrade);
            }
        }
        
        return affordable;
    }
    
    void PositionUpgradeButton(GameObject button, int index, int totalUpgrades)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Calculate the total height of all buttons
            float totalHeight = (totalUpgrades - 1) * buttonSpacing;
            
            // Calculate the starting Y position (top of the screen)
            float startY = totalHeight / 2f;
            
            // Calculate the Y position for this button (cheapest on top, going downward)
            float yPos = startY - (buttonSpacing * index);
            
            // Position on the right side of the screen
            Vector2 position = new Vector2(firstButtonPosition.x, yPos);
            
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = buttonSize;
        }
    }
}