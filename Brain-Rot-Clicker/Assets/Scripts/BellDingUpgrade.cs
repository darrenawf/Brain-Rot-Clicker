using UnityEngine;

public class BellDingUpgrade : Upgrade
{
    public GameObject bellSoundObject; // Drag a GameObject with AudioSource from hierarchy
    private AudioSource audioSource;
    private float bellTimer = 0f;
    private bool isActive = false;
    
    void Start()
    {
        upgradeName = "Bell Ding";
        cost = 100;
        
        // Get AudioSource from the dragged GameObject
        if (bellSoundObject != null)
        {
            audioSource = bellSoundObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("Bell Sound Object doesn't have an AudioSource component!");
            }
        }
    }
    
    void Update()
    {
        // Only run the timer if the upgrade is active
        if (isActive && brainClicker != null)
        {
            bellTimer += Time.deltaTime;
            
            if (bellTimer >= 10f)
            {
                PlayBellDing();
                bellTimer = 0f;
            }
        }
    }
    
    void PlayBellDing()
    {
        // Play the bell sound
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        
        // Add 10 brain rot
        brainClicker.brainRotCount += 10;
        brainClicker.lifetimeBrainRot += 10;
        
        Debug.Log("Bell Ding! +10 brain rot");
    }
    
    protected override void ApplyUpgrade()
    {
        isActive = true;
        Debug.Log("Bell Ding upgrade purchased! +10 brain rot every 10 seconds");
    }
}