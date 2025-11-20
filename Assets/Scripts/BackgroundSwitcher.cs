using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour
{
    [Header("Assign your BG SpriteRenderer here")]
    public SpriteRenderer bgRenderer;

    [Header("Assign the normal and red-alert sprites")]
    public Sprite normalSprite;
    public Sprite redAlertSprite;

    public void SetRedAlert(bool enabled)
    {
        if (bgRenderer == null) return;
        bgRenderer.sprite = enabled ? redAlertSprite : normalSprite;
    }
}

