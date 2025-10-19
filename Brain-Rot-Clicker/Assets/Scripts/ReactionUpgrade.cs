using UnityEngine;

public class ReactionUpgrade : Upgrade
{
    [Header("Visual Object")]
    public GameObject objectToShow; // This should be the ReactionFace object
    
    private int clickBonus = 2;
    private ReactionFace reactionFace;
    
    void Start()
    {
        upgradeName = "Reaction Upgrade";
        cost = 250;
        
        // Auto-find the BrainClicker if not set
        if (brainClicker == null)
        {
            brainClicker = FindObjectOfType<BrainClicker>();
        }
        
        // If objectToShow is not set, use the object this script is attached to
        if (objectToShow == null)
        {
            objectToShow = gameObject;
        }
        
        // Get the ReactionFace component from the objectToShow
        reactionFace = objectToShow.GetComponent<ReactionFace>();
        
        // Hide the object at start and show a face
        if (objectToShow != null)
        {
            objectToShow.SetActive(false);
        }
    }
    
    protected override void ApplyUpgrade()
    {
        // Increase the click multiplier in BrainClicker
        if (brainClicker != null)
        {
            brainClicker.IncreaseClickMultiplier(clickBonus);
            Debug.Log("Reaction Upgrade purchased! +" + clickBonus + " brain rot per click! Total: +" + brainClicker.GetClickMultiplier() + " per click");
            
            // Make the object visible when upgrade is purchased
            if (objectToShow != null)
            {
                objectToShow.SetActive(true);
                
                // Show a reaction face
                if (reactionFace != null)
                {
                    reactionFace.ShowRandomFace();
                }
            }
        }
    }
}