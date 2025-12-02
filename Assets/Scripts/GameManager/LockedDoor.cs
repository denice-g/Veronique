// SceneEdgeLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockedDoor : MonoBehaviour
{
    [Tooltip("Exact scene name (must be added in Build Settings).")]
    public string targetSceneName;

    [Tooltip("Spawn point name to use in target scene (e.g., Spawn_Left, Spawn_Right).")]
    public string targetSpawnPoint = "Spawn_Default";

    [Tooltip("Optional small delay for fade or audio.")]
    public float loadDelay = 0.1f;

    bool loading;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (loading) return;
        if (!other.CompareTag("Player")) return;

        if (!GameManager.Instance.AreAllMainPuzzlesComplete())
        {
            Debug.Log("Door is locked!");
            return;
        }

        loading = true;
        SpawnPoint.Next = targetSpawnPoint;
        Invoke(nameof(LoadTargetScene), loadDelay);
    }

    void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }
}

/*public static class SpawnStartPoint
{
    public static string Next = "Spawn_Default";
}*/
