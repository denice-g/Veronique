// SceneSpawn.cs
using UnityEngine;

public class SceneSpawn : MonoBehaviour
{
    [Tooltip("Fallback spawn name if SpawnPoint.Next wasn't set.")]
    public string defaultSpawnName = "Spawn_Default";

    void Start()
    {
        var nameToUse = string.IsNullOrEmpty(SpawnPoint.Next) ? defaultSpawnName : SpawnPoint.Next;
        var spawn = GameObject.Find(nameToUse);
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player && spawn)
        {
            player.transform.position = spawn.transform.position;
        }

        // Reset for safety (optional):
        SpawnPoint.Next = defaultSpawnName;
    }
}
