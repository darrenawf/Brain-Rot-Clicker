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
    
    // New variables for 7-second intervals
    private int passiveEvery7Seconds = 0;
    private float sevenSecondTimer = 0f;
    private bool justAddedSevenSecondBonus = false;
    private int sevenSecondBonusAmount = 0;

    // Animation variables
    private Vector3 originalScale;
    private Vector3 hoverScale;
    private bool isAnimating = false;
    private bool isHovering = false;

    // Animation settings (you can adjust these in the Inspector)
    public float clickScaleFactor = 0.8f;
    public float hoverScaleFactor = 1.1f;
    public float clickAnimationDuration = 0.1f;
    public float hoverAnimationDuration = 0.2f;

    void Start()
    {
        // Store original scale and calculate hover scale
        originalScale = transform.localScale;
        hoverScale = originalScale * hoverScaleFactor;
        UpdateCounters();
    }

    void Update()
    {
        // Remove clicks older than 1 second
        float currentTime = Time.time;
        clickTimes.RemoveAll(time => currentTime - time > 1f);

        // Handle passive brain rot generation (every second)
        passiveTimer += Time.deltaTime;
        if (passiveTimer >= 1f && passiveBPS > 0)
        {
            brainRotCount += passiveBPS;
            lifetimeBrainRot += passiveBPS;
            passiveTimer = 0f;
            UpdateCounters();
        }

        // Handle 7-second interval brain rot generation
        sevenSecondTimer += Time.deltaTime;
        if (sevenSecondTimer >= 7f && passiveEvery7Seconds > 0)
        {
            brainRotCount += passiveEvery7Seconds;
            lifetimeBrainRot += passiveEvery7Seconds;
            sevenSecondTimer = 0f;
            
            // Set flag to show the spike in BPS display
            justAddedSevenSecondBonus = true;
            sevenSecondBonusAmount = passiveEvery7Seconds;
            
            UpdateCounters();
        }

        // Update BPS display every 0.5 seconds
        updateTimer += Time.deltaTime;

        if (!hasStartedClicking && (clickTimes.Count > 0 || passiveBPS > 0 || passiveEvery7Seconds > 0))
        {
            // Calculate BPS - if we just added the 7-second bonus, show the spike
            if (justAddedSevenSecondBonus)
            {
                displayedBPS = clickTimes.Count + passiveBPS + sevenSecondBonusAmount;
                // Reset the flag after showing it once
                justAddedSevenSecondBonus = false;
            }
            else
            {
                displayedBPS = clickTimes.Count + passiveBPS + (passiveEvery7Seconds / 7);
            }
            
            UpdateCounters();
            hasStartedClicking = true;
            updateTimer = 0f;
        }
        else if (updateTimer >= 0.5f)
        {
            // Calculate BPS - if we just added the 7-second bonus, show the spike
            if (justAddedSevenSecondBonus)
            {
                displayedBPS = clickTimes.Count + passiveBPS + sevenSecondBonusAmount;
                // Reset the flag after showing it once
                justAddedSevenSecondBonus = false;
            }
            else
            {
                displayedBPS = clickTimes.Count + passiveBPS + (passiveEvery7Seconds / 7);
            }
            
            UpdateCounters();
            updateTimer = 0f;

            if (clickTimes.Count == 0 && passiveBPS == 0 && passiveEvery7Seconds == 0)
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

        // Start click animation
        StartCoroutine(ClickAnimation());

        UpdateCounters();
    }

    void OnMouseEnter()
    {
        if (!isAnimating)
        {
            isHovering = true;
            StartCoroutine(HoverAnimation(true));
        }
    }

    void OnMouseExit()
    {
        if (isHovering)
        {
            isHovering = false;
            StartCoroutine(HoverAnimation(false));
        }
    }

    // Click animation coroutine
    private System.Collections.IEnumerator ClickAnimation()
    {
        isAnimating = true;
        
        Vector3 targetScale = originalScale * clickScaleFactor;
        float elapsedTime = 0f;

        // Scale down
        while (elapsedTime < clickAnimationDuration / 2f)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (clickAnimationDuration / 2f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        elapsedTime = 0f;

        // Scale back up
        Vector3 endScale = isHovering ? hoverScale : originalScale;
        while (elapsedTime < clickAnimationDuration / 2f)
        {
            transform.localScale = Vector3.Lerp(targetScale, endScale, elapsedTime / (clickAnimationDuration / 2f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
        isAnimating = false;
    }

    // Hover animation coroutine
    private System.Collections.IEnumerator HoverAnimation(bool hoverIn)
    {
        isAnimating = true;
        
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = hoverIn ? hoverScale : originalScale;
        float elapsedTime = 0f;

        while (elapsedTime < hoverAnimationDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / hoverAnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        isAnimating = false;
    }

    // Public method to add passive BPS (every second)
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

    // NEW: Public method to add passive brain rot every 7 seconds
    public void AddPassiveEvery7Seconds(int amount)
    {
        passiveEvery7Seconds += amount;
        UpdateCounters();
    }

    // NEW: Public method to remove passive 7-second brain rot (if needed)
    public void RemovePassiveEvery7Seconds(int amount)
    {
        passiveEvery7Seconds -= amount;
        if (passiveEvery7Seconds < 0) passiveEvery7Seconds = 0;
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