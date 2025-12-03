using UnityEngine;
using UnityEngine.UI;

public class VictoryArrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [Tooltip("The arrow GameObject (usually the same GameObject this script is on)")]
    public GameObject arrowObject;
    
    [Tooltip("Arrow image component (optional - for color customization)")]
    public Image arrowImage;
    
    [Header("Colors")]
    [Tooltip("Color of the arrow")]
    public Color arrowColor = new Color(0.2f, 0.5f, 1f, 1f); // Blue
    
    void Start()
    {
        // If arrowObject not assigned, use this GameObject
        if (arrowObject == null)
        {
            arrowObject = gameObject;
        }
        
        // Auto-find image component if not assigned
        if (arrowImage == null)
        {
            arrowImage = GetComponent<Image>();
        }
        
        // Set arrow color
        if (arrowImage != null)
        {
            arrowImage.color = arrowColor;
        }
        
        // Hide arrow initially
        if (arrowObject != null)
        {
            arrowObject.SetActive(false);
        }
    }

    public void ShowVictoryArrow()
    {
        if (arrowObject != null)
        {
            arrowObject.SetActive(true);
            Debug.Log("[VictoryArrow] Arrow shown");
        }
    }

    public void HideVictoryArrow()
    {
        if (arrowObject != null)
        {
            arrowObject.SetActive(false);
            Debug.Log("[VictoryArrow] Arrow hidden");
        }
    }
}