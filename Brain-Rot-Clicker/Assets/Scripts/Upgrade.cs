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
            // Play upgrade purchase sound from any purchased SoundUpgrade
            PlayUpgradePurchaseSound();
            
            brainClicker.brainRotCount -= cost;
            isPurchased = true;
            ApplyUpgrade();
            OnPurchaseComplete();
        }
    }
    
    // New method to play sound when purchasing upgrades
    private void PlayUpgradePurchaseSound()
    {
        // Use the same sound system as brain clicks
        foreach (SoundUpgrade soundUpgrade in BrainClicker.soundUpgrades)
        {
            if (soundUpgrade != null && soundUpgrade.IsPurchased())
            {
                soundUpgrade.PlayClickSound();
                return; // Only play one sound
            }
        }
    }
    
    protected virtual void OnPurchaseComplete()
    {
        gameObject.SetActive(false);
    }
    
    protected abstract void ApplyUpgrade();
}