using UnityEngine;

public class NeonLightsAnimated : MonoBehaviour
{
    public enum ScrollDirection { Left, Right }
    
    public float scrollSpeed = 2f;
    public ScrollDirection direction = ScrollDirection.Left;
    
    private float backgroundWidth;
    private Vector3 startPosition;
    private Transform duplicateBackground;

    void Start()
    {
        // Calculate background width based on sprite renderer or assume 1920*2 = 3840 units
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            backgroundWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
            // If no sprite renderer, use the explicit dimensions you provided
            backgroundWidth = 1920 * 2f; // Adjust scale factor if needed
        }
        
        startPosition = transform.position;
        
        // Determine spawn position based on direction
        Vector3 spawnOffset = direction == ScrollDirection.Left ? 
            Vector3.right * backgroundWidth : 
            Vector3.left * backgroundWidth;
        
        // Create duplicate background
        GameObject duplicate = Instantiate(gameObject, transform.parent);
        duplicate.transform.position = transform.position + spawnOffset;
        
        // Remove this script from duplicate to avoid multiple instances running
        Destroy(duplicate.GetComponent<NeonLightsAnimated>());
        
        duplicateBackground = duplicate.transform;
    }

    void Update()
    {
        // Determine movement direction
        Vector3 moveDirection = direction == ScrollDirection.Left ? Vector3.left : Vector3.right;
        
        // Move both backgrounds
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, backgroundWidth);
        transform.position = startPosition + moveDirection * newPosition;
        
        // Reset position based on direction
        if (direction == ScrollDirection.Left)
        {
            if (transform.position.x <= startPosition.x - backgroundWidth)
            {
                transform.position = startPosition;
            }
        }
        else // Right direction
        {
            if (transform.position.x >= startPosition.x + backgroundWidth)
            {
                transform.position = startPosition;
            }
        }
        
        // Position the duplicate background
        Vector3 duplicateOffset = direction == ScrollDirection.Left ? 
            Vector3.right * backgroundWidth : 
            Vector3.left * backgroundWidth;
            
        duplicateBackground.position = transform.position + duplicateOffset;
    }
}