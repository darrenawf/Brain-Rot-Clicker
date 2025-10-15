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
    private int passiveBPS = 0;
    private float passiveTimer = 0f;

    void Start()
    {
        UpdateCounters();
    }

    void Update()
    {
        // Remove clicks older than 1 second
        float currentTime = Time.time;
        clickTimes.RemoveAll(time => currentTime - time > 1f);

        // Handle passive brain rot generation
        passiveTimer += Time.deltaTime;
        if (passiveTimer >= 1f && passiveBPS > 0)
        {
            brainRotCount += passiveBPS;
            lifetimeBrainRot += passiveBPS;
            passiveTimer = 0f;
            UpdateCounters();
        }

        // Update BPS display every 0.5 seconds
        updateTimer += Time.deltaTime;

        if (!hasStartedClicking && (clickTimes.Count > 0 || passiveBPS > 0))
        {
            displayedBPS = clickTimes.Count + passiveBPS;
            UpdateCounters();
            hasStartedClicking = true;
            updateTimer = 0f;
        }
        else if (updateTimer >= 0.5f)
        {
            displayedBPS = clickTimes.Count + passiveBPS;
            UpdateCounters();
            updateTimer = 0f;

            if (clickTimes.Count == 0 && passiveBPS == 0)
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

    // Public method to add passive BPS
    public void AddPassiveBPS(int amount)
    {
        passiveBPS += amount;
        UpdateCounters();
    }

    // Public method to remove passive BPS (if needed)
    public void RemovePassiveBPS(int amount)
    {
        passiveBPS -= amount;
        if (passiveBPS < 0) passiveBPS = 0;
        UpdateCounters();
    }

    // Make this public so upgrades can call it
    public void UpdateCounters()
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

    // Public method for passive upgrades to add brain rot directly
    public void AddBrainRot(int amount)
    {
        brainRotCount += amount;
        lifetimeBrainRot += amount;
        UpdateCounters();
    }
}