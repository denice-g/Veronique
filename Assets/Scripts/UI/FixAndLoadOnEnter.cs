using UnityEngine;
using UnityEngine.SceneManagement;

public class FixAndLoadOnEnter : MonoBehaviour
{
    public string playerTag = "Player";
    public string completionSceneName = "End_Screen";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        ShipCrisisController.Instance?.FixShip();
        Time.timeScale = 1f;
        SceneManager.LoadScene(completionSceneName);
    }
}

