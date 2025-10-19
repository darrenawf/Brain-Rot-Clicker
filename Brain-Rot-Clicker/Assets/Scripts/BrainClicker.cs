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

    // Updated variables for 10-second intervals
    private int passiveEvery10Seconds = 0;
    private float tenSecondTimer = 0f;
    private bool justAddedTenSecondBonus = false;
    private int tenSecondBonusAmount = 0;

    // Track when to show the spike
    private float spikeDisplayTime = 0f;
    private bool showingSpike = false;

    // Bell upgrade system
    private List<BellUpgrade> bellUpgrades = new List<BellUpgrade>();

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

    // Class to track fixed amount chances with sound
    [System.Serializable]
    public class FixedClickChance
    {
        public int fixedAmount;
        public float chance;
        public AudioSource bonusSound;

        public FixedClickChance(int amount, float chancePercent, AudioSource sound)
        {
            fixedAmount = amount;
            chance = chancePercent;
            bonusSound = sound;
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

        // Handle 10-second interval brain rot generation
        tenSecondTimer += Time.deltaTime;
        if (tenSecondTimer >= 10f && passiveEvery10Seconds > 0)
        {
            brainRotCount += passiveEvery10Seconds;
            lifetimeBrainRot += passiveEvery10Seconds;
            tenSecondTimer = 0f;

            justAddedTenSecondBonus = true;
            tenSecondBonusAmount = passiveEvery10Seconds;

            // Start showing the spike for 1 second
            showingSpike = true;
            spikeDisplayTime = Time.time;

            // Play bell sounds for all purchased bell upgrades
            PlayBellSounds();

            UpdateCounters();
        }

        // Check if we should stop showing the spike
        if (showingSpike && Time.time - spikeDisplayTime >= 1f)
        {
            showingSpike = false;
            justAddedTenSecondBonus = false;
        }

        // Update BPS display every 0.5 seconds
        updateTimer += Time.deltaTime;

        if (!hasStartedClicking && (clickTimes.Count > 0 || passiveBPS > 0 || passiveEvery10Seconds > 0))
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

            if (clickTimes.Count == 0 && passiveBPS == 0 && passiveEvery10Seconds == 0)
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
        AudioSource bonusSoundToPlay = null;

        foreach (FixedClickChance fixedChance in fixedChances)
        {
            if (Random.Range(0f, 1f) <= fixedChance.chance)
            {
                fixedAmountTriggered = true;
                fixedAmount = fixedChance.fixedAmount;
                bonusSoundToPlay = fixedChance.bonusSound;
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

        // Play bonus sound if fixed amount was triggered (using PlayOneShot for overlapping)
        if (fixedAmountTriggered && bonusSoundToPlay != null && bonusSoundToPlay.clip != null)
        {
            bonusSoundToPlay.PlayOneShot(bonusSoundToPlay.clip);
        }

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

    // Method to add fixed amount chance with sound
    public void AddChanceFixedClick(int amount, float chance, AudioSource bonusSound)
    {
        fixedChances.Add(new FixedClickChance(amount, chance, bonusSound));
        Debug.Log("Added fixed chance: " + amount + " with " + (chance * 100) + "% chance");
    }

    // Method to register bell upgrade
    public void RegisterBellUpgrade(BellUpgrade bellUpgrade)
    {
        if (!bellUpgrades.Contains(bellUpgrade))
        {
            bellUpgrades.Add(bellUpgrade);
        }
    }

    // Method to play all bell sounds
    private void PlayBellSounds()
    {
        foreach (BellUpgrade bellUpgrade in bellUpgrades)
        {
            if (bellUpgrade != null && bellUpgrade.IsPurchased())
            {
                bellUpgrade.PlayBellSound();
            }
        }
    }

    // Calculate BPS - show spike only when bonus triggers
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

        // Base BPS without 10-second bonus
        int baseBPS = adjustedBaseBPS + averageFixedBPS + passiveBPS;

        // Only show the spike for 1 second after the bonus triggers
        if (showingSpike)
        {
            // Show base BPS + the full bonus amount
            return baseBPS + tenSecondBonusAmount;
        }
        else
        {
            // Normal display: just the continuous BPS
            return baseBPS;
        }
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

    // Method to add passive every 10 seconds
    public void AddPassiveEvery10Seconds(int amount)
    {
        passiveEvery10Seconds += amount;
        UpdateCounters();
    }

    // Method to remove passive every 10 seconds
    public void RemovePassiveEvery10Seconds(int amount)
    {
        passiveEvery10Seconds -= amount;
        if (passiveEvery10Seconds < 0) passiveEvery10Seconds = 0;
        UpdateCounters();
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