using UnityEngine;
using UnityEngine.Tilemaps;

// Attach to the Tilemap you’re panning
[RequireComponent(typeof(Tilemap), typeof(Renderer))]
public class PanLoopTilemap : MonoBehaviour
{
    public Vector2 speed = new Vector2(3f, 0f);     // world units per second
    public Vector2 loopSizeOverride = Vector2.zero; // set if your tilemap repeats every W x H

    private Vector3 startPos;
    private Vector2 loop;
    private Renderer rend;
    private Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        rend    = GetComponent<Renderer>();
        startPos = transform.position;

        // Compute the repeating chunk size in world units.
        if (loopSizeOverride != Vector2.zero)
        {
            loop = loopSizeOverride;
        }
        else
        {
            // Use tilemap cell bounds * grid cell size (more reliable than renderer bounds)
            var grid = tilemap.layoutGrid;
            var cellSize = grid.cellSize;                  // world units per cell
            var cells = tilemap.cellBounds.size;           // number of cells (x,y)
            loop = new Vector2(cells.x * cellSize.x, cells.y * cellSize.y);
            // Fallback if empty:
            if (loop.x <= 0f || loop.y <= 0f)
                loop = (Vector2)rend.bounds.size;
        }
    }

    void Update()
    {
        // Move
        Vector3 pos = transform.position + (Vector3)(speed * Time.deltaTime);

        // Wrap in X (modulo handles very high speeds)
        if (loop.x > 0.0001f)
        {
            float dx = pos.x - startPos.x;
            float wraps = Mathf.Floor(dx / loop.x);        // can be many widths in one frame
            pos.x -= wraps * loop.x;
        }

        // Wrap in Y (optional)
        if (loop.y > 0.0001f)
        {
            float dy = pos.y - startPos.y;
            float wraps = Mathf.Floor(dy / loop.y);
            pos.y -= wraps * loop.y;
        }

        transform.position = pos;
    }
}
