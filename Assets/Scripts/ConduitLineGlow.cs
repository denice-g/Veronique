using UnityEngine;

[ExecuteAlways]
public class ConduitLineGlow : MonoBehaviour
{
    [Header("Identity")]
    public string id = "ORANGE";   // must match a lever's id
    
    [Header("Rank (0 = brightest / first)")]
    public int rank = 0;

    [Header("Refs")]
    public ConduitHintManager manager;

    private SpriteRenderer[] renderers;

    [Header("Layer Alpha Multipliers (Core / Halo1 / Halo2...)")]
    public float[] layerAlphaMul = { 1.00f, 0.32f, 0.12f };

    [Header("Per-line brightness")]
    [Range(0f, 2f)]
    public float brightnessMultiplier = 1f;   // <-- NEW

    [Header("Pulse")]
    public float pulseAmount = 0.08f;
    public float pulseSpeed  = 3f;

    void OnEnable()  { CacheChildren(); }
    void OnValidate(){ CacheChildren(); }

    void CacheChildren()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
    }

    void Update()
    {
        if (manager == null) return;
        if (renderers == null || renderers.Length == 0) CacheChildren();

        // 0..1 brightness from rank
        float t = manager.GetRankLerp01(rank);

        // baseA from manager.min/max, then scaled by our per-line multiplier
        float baseA = Mathf.Lerp(manager.minAlpha, manager.maxAlpha, t);
        baseA *= brightnessMultiplier;                          // <-- NEW
        baseA = Mathf.Clamp01(baseA);                           // <-- NEW

        float prox  = manager.GetProximityFactor(transform.position);
        float a     = Mathf.Clamp01(baseA * prox);

        if (pulseAmount > 0f)
        {
            a *= 1f + pulseAmount * Mathf.Sin(Time.time * pulseSpeed);
            a = Mathf.Clamp01(a);
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            float mul = (i < layerAlphaMul.Length) ? layerAlphaMul[i] : 1f;
            var c = r.color;
            c.a = Mathf.Clamp01(a * mul);
            r.color = c;
        }
    }
}
