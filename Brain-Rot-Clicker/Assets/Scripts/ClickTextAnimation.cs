using UnityEngine;
using TMPro;

public class ClickTextAnimation : MonoBehaviour
{
    public float floatSpeed = 50f;
    public float fadeSpeed = 2f;
    private TextMeshProUGUI text;
    private RectTransform rectTransform;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        
        // Ensure the text starts fully visible
        if (text != null)
        {
            Color color = text.color;
            color.a = 1f;
            text.color = color;
            
            // Set font size to 40
            text.fontSize = 60;
            text.fontStyle = FontStyles.Bold;
        }
        
        // Destroy after 1 second
        if (gameObject.scene.IsValid())
        {
            Destroy(gameObject, 1f);
        }
    }

    void Update()
    {
        // Only animate if we have the components
        if (text != null && rectTransform != null)
        {
            // Float upward
            rectTransform.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
            
            // Fade out
            Color color = text.color;
            color.a -= fadeSpeed * Time.deltaTime;
            text.color = color;
        }
    }
}