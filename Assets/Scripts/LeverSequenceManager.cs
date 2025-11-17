using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeverSequenceManager : MonoBehaviour
{
    [Header("Sources")]
    public ConduitHintManager hintManager;     // drag Puzzle_ConduitHints here
    public List<Lever> levers = new List<Lever>();

    [Header("Room Lighting (dark overlay fade)")]
    public SpriteRenderer darknessOverlay;     // black overlay SpriteRenderer
    [Range(0f,1f)] public float overlayTargetAlpha = 0f; // 0 = fully bright after solve
    public float fadeDuration = 1.6f;

    [Header("Feedback (optional)")]
    public AudioSource sfx;
    public AudioClip solvedJingle;
    public AudioClip wrongSequenceJingle;      // optional “bzzz” for wrong 5-lever combo

    // internal state
    private List<string> expectedOrder = new List<string>();  // correct sequence of IDs
    private List<string> enteredOrder  = new List<string>();  // what player has flipped this attempt
    public bool solved = false;

    void Awake()
    {
        BuildExpectedOrderFromHints();
        WireLevers();
    }

    void BuildExpectedOrderFromHints()
    {
        expectedOrder.Clear();

        if (hintManager == null || hintManager.lines == null || hintManager.lines.Count == 0)
        {
            Debug.LogWarning("[LeverSequence] No hintManager or lines; set order manually.");
            return;
        }

        // sort by rank (0 = brightest / first in order)
        var sorted = hintManager.lines
            .Where(l => l != null)
            .OrderBy(l => l.rank)
            .ToList();

        foreach (var line in sorted)
            expectedOrder.Add(line.id.Trim().ToUpper());

        Debug.Log("[LeverSequence] Expected order: " + string.Join(" -> ", expectedOrder));
    }

    void WireLevers()
    {
        foreach (var lever in levers)
        {
            if (!lever) continue;
            lever.manager = this;
            lever.SetOff(false);
        }

        solved = false;
        enteredOrder.Clear();
    }

    // Called by Lever when player presses E
    public void AttemptFlip(Lever lever)
    {
        if (solved || lever == null) return;

        // ignore flipping the same lever again within the same attempt
        if (lever.IsOn) return;

        // turn it on visually
        lever.SetOn(true);

        // record this choice
        string id = lever.id.Trim().ToUpper();
        enteredOrder.Add(id);

        // if we haven't flipped 5 levers yet, just wait for more
        if (enteredOrder.Count < expectedOrder.Count)
            return;

        // we have 5 levers flipped – check the whole sequence
        bool match = true;
        for (int i = 0; i < expectedOrder.Count; i++)
        {
            if (enteredOrder[i] != expectedOrder[i])
            {
                match = false;
                break;
            }
        }

        if (match)
        {
            // Puzzle solved!
            solved = true;
            if (solvedJingle && sfx) sfx.PlayOneShot(solvedJingle);
            StartCoroutine(PowerOnRoutine());
        }
        else
        {
            // Whole 5-lever sequence was wrong → reset all levers
            if (wrongSequenceJingle && sfx) sfx.PlayOneShot(wrongSequenceJingle);
            StartCoroutine(ResetAllCo());
        }
    }

    IEnumerator ResetAllCo()
    {
        // tiny delay so player can "see" the last lever flip before everything drops
        yield return new WaitForSeconds(0.1f);

        enteredOrder.Clear();
        foreach (var l in levers)
            l?.SetOff(false);
    }

    IEnumerator PowerOnRoutine()
    {
        if (!darknessOverlay) yield break;

        float t = 0f;
        float startA = darknessOverlay.color.a;
        var c = darknessOverlay.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.SmoothStep(0f, 1f, t / fadeDuration);

            c.a = Mathf.Lerp(startA, overlayTargetAlpha, u);
            darknessOverlay.color = c;

            yield return null;
        }
        c.a = overlayTargetAlpha;
        darknessOverlay.color = c;
    }
}
