using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private float defaultDuration = 4f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (dialogText != null)
            dialogText.gameObject.SetActive(false);
    }

    public void ShowDialog(string message, float duration = -1f)
    {
        if (dialogText == null) return;

        dialogText.text = message;
        dialogText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideDialog));

        if (duration <= 0f)
            duration = defaultDuration;

        Invoke(nameof(HideDialog), duration);
    }

    public void HideDialog()
    {
        if (dialogText != null)
            dialogText.gameObject.SetActive(false);
    }
}
