using UnityEngine;
using System.Collections.Generic;

public enum ProximityMode { Off, HorizontalOnly, Radial2D }

public class ConduitHintManager : MonoBehaviour
{
    [Header("Lines (drag overlays here)")]
    public List<ConduitLineGlow> lines = new List<ConduitLineGlow>();

    [Header("Player (for proximity)")]
    public Transform player;

    [Header("Alpha by Rank")]
    [Range(0f,1f)] public float minAlpha = 0.20f;
    [Range(0f,1f)] public float maxAlpha = 1.00f;

    [Header("Proximity Reveal")]
    public ProximityMode proximityMode = ProximityMode.HorizontalOnly;
    public float fullBrightDistance = 1.5f;
    public float fadeStartDistance  = 6.0f;

    void Awake()
    {
        foreach (var l in lines) if (l) l.manager = this;
    }

    public float GetProximityFactor(Vector3 worldPos)
    {
        if (proximityMode == ProximityMode.Off || player == null) return 1f;

        if (proximityMode == ProximityMode.HorizontalOnly)
        {
            float dx = Mathf.Abs(player.position.x - worldPos.x);
            return Mathf.Clamp01(Mathf.InverseLerp(fadeStartDistance, fullBrightDistance, dx));
        }
        else // Radial2D
        {
            float d = Vector2.Distance(player.position, worldPos);
            return Mathf.Clamp01(Mathf.InverseLerp(fadeStartDistance, fullBrightDistance, d));
        }
    }

    // 1 for rank 0, 0 for worst rank
    public float GetRankLerp01(int rank)
    {
        int n = Mathf.Max(1, lines.Count);
        float t = (n == 1) ? 1f : 1f - (float)rank / (n - 1);
        return Mathf.Clamp01(t);
    }
}
