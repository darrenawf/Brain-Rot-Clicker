using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BrainClicker : MonoBehaviour
{
    public int brainRotCount = 0;
    public int lifetimeBrainRot = 0;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI bpsText;
    public GameObject clickTextPrefab;
    public ReactionFace reactionFace;

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

    // Click multiplier variables
    private int clickMultiplier = 1;

    // Fixed amount chance variables
    private List<FixedClickChance> fixedChances = new List<FixedClickChance>();

    // Sound system
    public static List<SoundUpgrade> soundUpgrades = new List<SoundUpgrade>();

    // Animation variables
    private Vector3 originalScale;
    private Vector3 hoverScale;
    private bool isAnimating = false;
    private bool isHovering = false;

    // Animation settings
    public float clickScaleFactor = 0.8f;
    public float hoverScaleFactor = 1.1f;
    public float clickAnimationDuration = 0.1f;
    public float hoverAnimationDuration = 0.2f;

    // Class to track fixed amount chances
    [System.Serializable]
    public class FixedClickChance
    {
        public int fixedAmount;
        public float chance;

        public FixedClickChance(int amount, float chancePercent)
        {
            fixedAmount = amount;
            chance = chancePercent;
        }
    }

    void Start()
    {
        originalScale = transform.localScale;
        hoverScale = originalScale * hoverScaleFactor;

        // Initialize sound upgrades list
        soundUpgrades = new List<SoundUpgrade>();

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

            justAddedSevenSecondBonus = true;
            sevenSecondBonusAmount = passiveEvery7Seconds;

            UpdateCounters();
        }

        // Update BPS display every 0.5 seconds
        updateTimer += Time.deltaTime;

        if (!hasStartedClicking && (clickTimes.Count > 0 || passiveBPS > 0 || passiveEvery7Seconds > 0))
        {
            displayedBPS = CalculateCurrentBPS();
            UpdateCounters();
            hasStartedClicking = true;
            updateTimer = 0f;
        }
        else if (updateTimer >= 0.5f)
        {
            displayedBPS = CalculateCurrentBPS();
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
        int clickAmount = clickMultiplier;

        // Check for fixed amount chances (replace the click amount if triggered)
        bool fixedAmountTriggered = false;
        int fixedAmount = 0;

        foreach (FixedClickChance fixedChance in fixedChances)
        {
            if (Random.Range(0f, 1f) <= fixedChance.chance)
            {
                fixedAmountTriggered = true;
                fixedAmount = fixedChance.fixedAmount;
                break; // Only one fixed amount can trigger per click
            }
        }

        // Use fixed amount if triggered, otherwise use normal click amount
        int finalAmount = fixedAmountTriggered ? fixedAmount : clickAmount;

        brainRotCount += finalAmount;
        lifetimeBrainRot += finalAmount;

        // Record the time of this click
        clickTimes.Add(Time.time);

        // Spawn animation with the final amount (red if fixed amount triggered)
        SpawnClickAnimation(finalAmount, fixedAmountTriggered);

        // Play click sound
        PlayClickSound();

        // Show random reaction face ONLY if it's active and enabled
        if (reactionFace != null && reactionFace.gameObject.activeInHierarchy)
        {
            reactionFace.ShowRandomFace();
        }

        // Start click animation
        StartCoroutine(ClickAnimation());

        UpdateCounters();
    }

    void SpawnClickAnimation(int amount, bool isFixedAmount)
    {
        if (clickTextPrefab != null)
        {
            GameObject animationInstance = Instantiate(clickTextPrefab, Vector3.zero, Quaternion.identity);
            animationInstance.SetActive(true);

            TextMeshProUGUI textComponent = animationInstance.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = "+" + amount;
                textComponent.fontSize = 40;
                textComponent.fontStyle = FontStyles.Bold;

                // Make fixed amount clicks red
                if (isFixedAmount)
                {
                    textComponent.color = Color.red;
                }
            }

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                animationInstance.transform.SetParent(canvas.transform, false);

                RectTransform rectTransform = animationInstance.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    float randomX = Random.Range(-250f, 250f);
                    float randomY = Random.Range(250f, 350f);
                    rectTransform.anchoredPosition = new Vector2(randomX, randomY);
                }
            }
        }
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

    // Method to add fixed amount chance
    public void AddChanceFixedClick(int amount, float chance)
    {
        fixedChances.Add(new FixedClickChance(amount, chance));
        Debug.Log("Added fixed chance: " + amount + " with " + (chance * 100) + "% chance");
    }

    // Calculate BPS including fixed chance averages
    private int CalculateCurrentBPS()
    {
        int baseClickBPS = clickTimes.Count * clickMultiplier;

        // Calculate average BPS from fixed chances
        float fixedChanceBPS = 0f;
        foreach (FixedClickChance fixedChance in fixedChances)
        {
            fixedChanceBPS += fixedChance.fixedAmount * fixedChance.chance;
        }
        int averageFixedBPS = Mathf.RoundToInt(clickTimes.Count * fixedChanceBPS);

        // Calculate what portion of clicks use fixed amounts vs normal amounts
        float totalFixedChance = 0f;
        foreach (FixedClickChance fixedChance in fixedChances)
        {
            totalFixedChance += fixedChance.chance;
        }
        totalFixedChance = Mathf.Clamp01(totalFixedChance); // Cap at 100%

        // Adjust base BPS for the chance that clicks are replaced by fixed amounts
        int adjustedBaseBPS = Mathf.RoundToInt(clickTimes.Count * clickMultiplier * (1f - totalFixedChance));

        int totalBPS = adjustedBaseBPS + averageFixedBPS + passiveBPS + (passiveEvery7Seconds / 7);

        if (justAddedSevenSecondBonus)
        {
            totalBPS += sevenSecondBonusAmount;
            justAddedSevenSecondBonus = false;
        }

        return totalBPS;
    }

    // Method to play click sound
    private void PlayClickSound()
    {
        // Play sound upgrades
        foreach (SoundUpgrade soundUpgrade in soundUpgrades)
        {
            if (soundUpgrade != null && soundUpgrade.IsPurchased())
            {
                soundUpgrade.PlayClickSound();
                return; // Only play one sound upgrade
            }
        }
        
        // No default sound - only play if there are purchased sound upgrades
    }

    // Method to increase click multiplier
    public void IncreaseClickMultiplier(int amount)
    {
        clickMultiplier += amount;
        Debug.Log("Click multiplier increased to: " + clickMultiplier);
    }

    // Method to get current click multiplier
    public int GetClickMultiplier()
    {
        return clickMultiplier;
    }

    // Click animation coroutine
    private System.Collections.IEnumerator ClickAnimation()
    {
        isAnimating = true;

        Vector3 targetScale = originalScale * clickScaleFactor;
        float elapsedTime = 0f;

        while (elapsedTime < clickAnimationDuration / 2f)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (clickAnimationDuration / 2f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        elapsedTime = 0f;

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

    public void AddPassiveBPS(int amount)
    {
        passiveBPS += amount;
        UpdateCounters();
    }

    public void RemovePassiveBPS(int amount)
    {
        passiveBPS -= amount;
        if (passiveBPS < 0) passiveBPS = 0;
        UpdateCounters();
    }

    public void AddPassiveEvery7Seconds(int amount)
    {
        passiveEvery7Seconds += amount;
        UpdateCounters();
    }

    public void RemovePassiveEvery7Seconds(int amount)
    {
        passiveEvery7Seconds -= amount;
        if (passiveEvery7Seconds < 0) passiveEvery7Seconds = 0;
        UpdateCounters();
    }

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

    public void AddBrainRot(int amount)
    {
        brainRotCount += amount;
        lifetimeBrainRot += amount;
        UpdateCounters();
    }
}