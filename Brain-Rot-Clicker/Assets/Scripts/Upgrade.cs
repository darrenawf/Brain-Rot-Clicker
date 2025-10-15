using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Upgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    public string upgradeName;
    public int cost;    
    protected BrainClicker brainClicker;
    protected Button button;
    protected bool isPurchased = false;
    
    public void Initialize(BrainClicker brainClickerRef)
    {
        brainClicker = brainClickerRef;
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnUpgradePurchased);
        }
    }
    
    public void UpdateButtonVisibility()
    {
        if (brainClicker != null && !isPurchased)
        {
            gameObject.SetActive(IsAffordable() && IsAvailable());
        }
    }
    
    protected virtual bool IsAffordable()
    {
        // Only check lifetime brain rot for visibility (unlock requirement)
        return brainClicker.lifetimeBrainRot >= cost;
    }
    
    protected virtual bool IsAvailable()
    {
        return !isPurchased;
    }
    
    // Public method to check if upgrade is purchased
    public bool IsPurchased()
    {
        return isPurchased;
    }
    
    void OnUpgradePurchased()
    {
        // Check current brain rot at purchase time to prevent going negative
        if (!isPurchased && brainClicker.lifetimeBrainRot >= cost && brainClicker.brainRotCount >= cost)
        {
            brainClicker.brainRotCount -= cost;
            isPurchased = true;
            ApplyUpgrade();
            OnPurchaseComplete();
        }
    }
    
    protected virtual void OnPurchaseComplete()
    {
        gameObject.SetActive(false);
    }
    
    protected abstract void ApplyUpgrade();
}