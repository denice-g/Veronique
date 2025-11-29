using UnityEngine;
using UnityEngine.UI;

public class PipeTile : MonoBehaviour
{
    public enum Asset { Blank, Straight, Corner, TJunction }

    [Header("Runtime")]
    public Asset asset = Asset.Blank;
    public int rotationStep = 0; // 0..3 (but Straight uses 0..1)

    [Header("Sprites (assign in inspector)")]
    public Sprite blankSprite;
    public Sprite straightSprite; // baseline: horizontal (left-right)
    public Sprite cornerSprite;   // baseline: left -> down
    public Sprite tSprite;        // baseline: up, left, right (T pointing up)

    [Header("Runtime puzzle fields")]
    [HideInInspector] public int correctRotation = 0; // rotation that makes tile correct
    [HideInInspector] public bool locked = false;      // start/end tiles

    [HideInInspector] public bool isStart = false; // set true for start tiles
    [HideInInspector] public bool isEnd = false; // set true for end tiles
    [HideInInspector] public int rotationForStart = 0; // rotation that counts as correct for start
    [HideInInspector] public int rotationForEnd = 2; // rotation that counts as correct for end

    [HideInInspector] public bool up, right, down, left; // current connections (after UpdateConnections)

    [HideInInspector] public PipePuzzleGenerator manager; // set by generator

    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ApplyVisual()
    {
        if (image == null) image = GetComponent<Image>();

        switch (asset)
        {
            case Asset.Blank: image.sprite = blankSprite; break;
            case Asset.Straight: image.sprite = straightSprite; break;
            case Asset.Corner: image.sprite = cornerSprite; break;
            case Asset.TJunction: image.sprite = tSprite; break;
        }

        transform.localRotation = Quaternion.Euler(0, 0, -90 * rotationStep);
    }

    // Update up/right/down/left based on asset baseline and rotationStep
    public void UpdateConnections()
    {
        bool up0 = false, right0 = false, down0 = false, left0 = false;

        // baseline orientations (based on your sprite descriptions)
        switch (asset)
        {
            case Asset.Blank:
                up0 = right0 = down0 = left0 = false;
                break;

            case Asset.Straight:
                // baseline: horizontal (left <-> right)
                left0 = true;
                right0 = true;
                break;

            case Asset.Corner:
                // baseline: left -> down
                left0 = true;
                down0 = true;
                break;

            case Asset.TJunction:
                // baseline: T pointing up (up, left, right)
                up0 = true;
                left0 = true;
                right0 = true;
                break;
        }

        // rotate clockwise rotationStep times (90° each)
        bool u = up0, r = right0, d = down0, l = left0;
        for (int i = 0; i < rotationStep; i++)
        {
            bool nu = l;
            bool nr = u;
            bool nd = r;
            bool nl = d;
            u = nu; r = nr; d = nd; l = nl;
        }

        up = u; right = r; down = d; left = l;
    }

    // Attempt to find and store the canonical rotation that yields exactly the requested connectors.
    // Returns true if found and sets correctRotation. Does not change final visual rotation (it restores rotationStep).
    public void SetCorrectRotationForConnections(bool upC, bool rightC, bool downC, bool leftC)
    {
        // store correct rotation for reference
        up = upC;
        right = rightC;
        down = downC;
        left = leftC;

        // figure rotationStep for sprite orientation based on your sprites:
        if (asset == Asset.Straight)
        {
            correctRotation = (upC && downC) ? 1 : 0; // vertical = 1, horizontal = 0
        }
        else if (asset == Asset.Corner)
        {
            if (upC && rightC) correctRotation = 0;
            else if (rightC && downC) correctRotation = 1;
            else if (downC && leftC) correctRotation = 2;
            else correctRotation = 3;
        }
        else if (asset == Asset.TJunction)
        {
            if (!downC) correctRotation = 0;
            else if (!leftC) correctRotation = 1;
            else if (!upC) correctRotation = 2;
            else correctRotation = 3;
        }
        else correctRotation = 0;
    }

    public bool IsCorrect()
    {
        if (isStart)
        {
            // For start/end tiles, check against the required rotation, not correctRotation
            return rotationStep == rotationForStart;
        }
        else if(isEnd)
        {
            return rotationStep == rotationForEnd;
        }

        return rotationStep == correctRotation;
    }

    public void OnClickRotate()
    {
        if (locked) return;

        // advance rotation step but respect tile rotation limits
        rotationStep++;
        if (asset == Asset.Straight)
            rotationStep %= 2; // straight has only 2 states
        else if (asset == Asset.Blank)
            rotationStep = 0;   // blanks don't rotate
        else
            rotationStep %= 4; // corner and T have 4 states

        ApplyVisual();
        UpdateConnections();

        /*if (IsCorrect())
            Debug.Log($"Tile {name} is CORRECT (rot {rotationStep})");
        else
            Debug.Log($"Tile {name} is WRONG (rot {rotationStep})");*/

        if (manager != null)
            manager.OnTileChanged(this);
    }
}