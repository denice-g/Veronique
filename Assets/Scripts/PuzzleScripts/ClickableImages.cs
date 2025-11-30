using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableImage : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum ImageType { Calendar, Key }
    public ImageType imageType;

    [Header("Visual Feedback")]
    public bool enableHoverEffect = true;
    public Color hoverColor = new Color(1f, 1f, 0.7f, 1f); // Slight yellow tint
    public float hoverScale = 1.1f;

    [Header("Optional - Popup on Click")]
    public GameObject popupPanel; // Optional: assign if you want a popup to appear

    private StoragePuzzleManager puzzleManager;
    private Image image;
    private Color originalColor;
    private Vector3 originalScale;
    private bool hasBeenClicked = false;

    void Start()
    {
        // Find puzzle manager
        puzzleManager = FindObjectOfType<StoragePuzzleManager>();
        
        if (puzzleManager == null)
        {
            Debug.LogError($"[ClickableImage] StoragePuzzleManager not found! {gameObject.name} won't work.");
        }

        // Get image component
        image = GetComponent<Image>();
        
        if (image != null)
        {
            originalColor = image.color;
            
            // CRITICAL: Enable raycast target so image can be clicked
            image.raycastTarget = true;
        }
        else
        {
            Debug.LogError($"[ClickableImage] No Image component found on {gameObject.name}!");
        }

        // Store original scale
        originalScale = transform.localScale;

        // Hide popup initially if assigned
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (puzzleManager == null) return;

        Debug.Log($"[ClickableImage] {imageType} image clicked!");

        // Mark as clicked
        hasBeenClicked = true;

        // Show popup if assigned
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }

        // Notify puzzle manager
        switch (imageType)
        {
            case ImageType.Calendar:
                puzzleManager.OnCalendarClicked();
                break;
            
            case ImageType.Key:
                puzzleManager.OnKeyClicked();
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!enableHoverEffect) return;

        // Change color
        if (image != null)
        {
            image.color = hoverColor;
        }

        // Scale up slightly
        transform.localScale = originalScale * hoverScale;

        // Change cursor (optional - requires cursor management)
        // Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!enableHoverEffect) return;

        // Restore color
        if (image != null)
        {
            image.color = originalColor;
        }

        // Restore scale
        transform.localScale = originalScale;
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    public bool HasBeenClicked()
    {
        return hasBeenClicked;
    }

    public void ResetClickState()
    {
        hasBeenClicked = false;
    }
}