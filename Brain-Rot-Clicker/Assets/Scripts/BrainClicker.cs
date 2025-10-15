using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BrainClicker : MonoBehaviour
{
    public int brainRotCount = 0;
    public int lifetimeBrainRot = 0;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI bpsText;
    
    private List<float> clickTimes = new List<float>();
    private float updateTimer = 0f;
    private int displayedBPS = 0;
    private bool hasStartedClicking = false;
    
    void Start()
    {
        UpdateCounters();
    }
    
    void Update()
    {
        // Remove clicks older than 1 second
        float currentTime = Time.time;
        clickTimes.RemoveAll(time => currentTime - time > 1f);
        
        // Update BPS display
        updateTimer += Time.deltaTime;
        
        if (!hasStartedClicking && clickTimes.Count > 0)
        {
            displayedBPS = clickTimes.Count;
            UpdateCounters();
            hasStartedClicking = true;
            updateTimer = 0f;
        }
        else if (updateTimer >= 1f)
        {
            displayedBPS = clickTimes.Count;
            UpdateCounters();
            updateTimer = 0f;
            
            if (clickTimes.Count == 0)
            {
                hasStartedClicking = false;
            }
        }
    }
    
    void OnMouseDown()
    {
        brainRotCount += 1;
        lifetimeBrainRot += 1;
        
        // Record the time of this click
        clickTimes.Add(Time.time);
        
        UpdateCounters();
    }
    
    void UpdateCounters()
    {
        if (counterText != null)
        {
            counterText.text = brainRotCount + " brain rot";
        }
        if (bpsText != null)
        {
            bpsText.text = displayedBPS + " brain rot per second";
        }
    }
}