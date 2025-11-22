using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PipePuzzleGenerator : MonoBehaviour
{
    public static PipePuzzleGenerator Instance;

    [Header("---------- Grid Size ----------")]
    public int columns = 16;
    public int rows = 9;

    [Header("---------- References ----------")]
    public PipeTile tilePrefab; // assign prefab (UI Image + PipeTile)
    public Transform gridParent; // GridLayoutGroup parent

    [Header("---------- Pipe Sprites ----------")]
    public Sprite blankSprite;
    public Sprite straightSprite;
    public Sprite cornerSprite;
    public Sprite tSprite;

    [Header("---------- MenuUIs ----------")]
    [SerializeField] private GameObject puzzle1UI;
    public GameObject Fire1;

    private PipeTile[,] tiles;
    private List<Vector2Int> solutionPath;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        GeneratePuzzle();
    }

    public void GeneratePuzzle()
    {
        // enforce size if you want 16x9 specifically
        if (columns * rows != 144)
        {
            // optional: adjust but we'll keep user-provided values
        }

        tiles = new PipeTile[columns, rows];

        // Clear old children
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParent.GetChild(i).gameObject);
        }

        // Instantiate blank tiles (no visuals yet)
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var t = Instantiate(tilePrefab, gridParent);
                t.blankSprite = blankSprite;
                t.straightSprite = straightSprite;
                t.cornerSprite = cornerSprite;
                t.tSprite = tSprite;
                t.manager = this;

                t.asset = PipeTile.Asset.Blank;
                t.rotationStep = 0;
                t.correctRotation = 0;
                t.locked = false;

                tiles[x, y] = t;

                var btn = t.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(t.OnClickRotate);
                }
            }
        }

        BuildPathAndFill();

        // Fix start/end visuals and lock them
        var startTile = tiles[0, 0];
        startTile.locked = true;
        startTile.rotationStep = startTile.correctRotation;
        startTile.ApplyVisual();
        startTile.UpdateConnections();

        var endTile = tiles[columns - 1, rows - 1];
        endTile.locked = true;
        endTile.rotationStep = endTile.correctRotation;
        endTile.ApplyVisual();
        endTile.UpdateConnections();

        // Randomize visuals for everything except locked tiles
        RandomizeTileRotation();
    }

    void BuildPathAndFill()
    {
        // Clear all tiles
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                var t = tiles[x, y];
                t.asset = PipeTile.Asset.Blank;
                t.rotationStep = 0;
                t.correctRotation = 0;
                t.locked = false;
                t.ApplyVisual();
                t.UpdateConnections();
            }
        }

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(columns - 1, rows - 1);
        solutionPath = MakeRandomSimplePath(start, end);

        // Assign correct tile types and rotations for path
        for (int i = 0; i < solutionPath.Count; i++)
        {
            Vector2Int cell = solutionPath[i];
            Vector2Int prev = i > 0 ? solutionPath[i - 1] : new Vector2Int(-999, -999);
            Vector2Int next = i < solutionPath.Count - 1 ? solutionPath[i + 1] : new Vector2Int(-999, -999);

            bool upC = false, rightC = false, downC = false, leftC = false;

            if (prev.x != -999)
            {
                if (prev.x == cell.x && prev.y == cell.y + 1) upC = true;
                if (prev.x == cell.x && prev.y == cell.y - 1) downC = true;
                if (prev.y == cell.y && prev.x == cell.x + 1) leftC = true;
                if (prev.y == cell.y && prev.x == cell.x - 1) rightC = true;
            }

            if (next.x != -999)
            {
                if (next.x == cell.x && next.y == cell.y + 1) upC = true;
                if (next.x == cell.x && next.y == cell.y - 1) downC = true;
                if (next.y == cell.y && next.x == cell.x + 1) leftC = true;
                if (next.y == cell.y && next.x == cell.x - 1) rightC = true;
            }

            var tile = tiles[cell.x, cell.y];

            int connections = (upC ? 1 : 0) + (rightC ? 1 : 0) + (downC ? 1 : 0) + (leftC ? 1 : 0);

            if (connections == 2)
            {
                if ((upC && downC) || (leftC && rightC))
                    tile.asset = PipeTile.Asset.Straight;
                else
                    tile.asset = PipeTile.Asset.Corner;
            }
            else if (connections == 3 || connections == 4)
            {
                tile.asset = PipeTile.Asset.TJunction;
            }
            else
            {
                tile.asset = PipeTile.Asset.Blank;
            }

            tile.SetCorrectRotationForConnections(upC, rightC, downC, leftC);
            tile.UpdateConnections();
        }

        // Lock start and end tiles and set correct rotation
        var startTile = tiles[0, 0];
        startTile.asset = PipeTile.Asset.TJunction;
        startTile.locked = true;
        startTile.rotationStep = startTile.correctRotation;
        startTile.ApplyVisual();
        startTile.UpdateConnections();

        var endTile = tiles[columns - 1, rows - 1];
        endTile.asset = PipeTile.Asset.TJunction;
        endTile.locked = true;
        endTile.rotationStep = endTile.correctRotation;
        endTile.ApplyVisual();
        endTile.UpdateConnections();

        // Randomize only the **interior path tiles** (exclude start/end)
        foreach (var cell in solutionPath)
        {
            if ((cell.x == 0 && cell.y == 0) || (cell.x == columns - 1 && cell.y == rows - 1))
            {
                continue;
            }

            var tile = tiles[cell.x, cell.y];
            int max = (tile.asset == PipeTile.Asset.Straight) ? 2 : 4;
            tile.rotationStep = Random.Range(0, max);
            tile.ApplyVisual();
            tile.UpdateConnections();
        }
    }

    List<Vector2Int> MakeRandomSimplePath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int> { start };
        HashSet<Vector2Int> visited = new HashSet<Vector2Int> { start };
        System.Random rng = new System.Random();

        while (true)
        {
            Vector2Int current = path[path.Count - 1];
            if (current == end) break;

            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] dir = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

            foreach (var d in dir)
            {
                Vector2Int npos = current + d;
                if (npos.x < 0 || npos.x >= columns || npos.y < 0 || npos.y >= rows) continue;
                if (visited.Contains(npos)) continue;
                neighbors.Add(npos);
            }

            if (neighbors.Count == 0)
            {
                path.RemoveAt(path.Count - 1);
                if (path.Count == 0) return FallbackStraightPath(start, end);
                continue;
            }

            // Shuffle neighbors randomly
            for (int i = 0; i < neighbors.Count; i++)
            {
                int j = rng.Next(i, neighbors.Count);
                var temp = neighbors[i];
                neighbors[i] = neighbors[j];
                neighbors[j] = temp;
            }

            var chosen = neighbors[0];
            path.Add(chosen);
            visited.Add(chosen);
        }

        return path;
    }

    List<Vector2Int> FallbackStraightPath(Vector2Int start, Vector2Int end)
    {
        var p = new List<Vector2Int>();
        for (int x = start.x; x <= end.x; x++) p.Add(new Vector2Int(x, start.y));
        for (int y = start.y + 1; y <= end.y; y++) p.Add(new Vector2Int(end.x, y)); // assume end.y >= start.y
        return p;
    }

    // Called by tiles after they rotate
    public void OnTileChanged(PipeTile tile)
    {
        // tile.UpdateConnections(); // tile already updated itself
        if (IsPuzzleComplete())
        {
            //Debug.Log("Puzzle Complete!");
            OnPuzzleCompleted();
        }
    }

    // Follow connections from start (0,0) to end (columns-1, rows-1)
    public bool IsPuzzleComplete()
    {
        foreach (var tile in tiles)
        {
            if (tile.asset == PipeTile.Asset.Blank) continue; // ignore blanks
            if (tile.rotationStep != tile.correctRotation) return false; // not correctly rotated
        }
        return true;
    }

    private bool FollowPipe(int x, int y, bool[,] visited)
    {
        if (x < 0 || x >= columns || y < 0 || y >= rows) return false;
        if (visited[x, y]) return false;
        var t = tiles[x, y];
        if (t == null) return false;

        visited[x, y] = true;

        // check if reached end
        if (x == columns - 1 && y == rows - 1)
        {
            // ensure this end tile has a connection back to previous tile (optional)
            return true;
        }

        // UP
        if (t.up && y + 1 < rows)
        {
            var nb = tiles[x, y + 1];
            if (nb != null && nb.down && !visited[x, y + 1])
                if (FollowPipe(x, y + 1, visited)) return true;
        }

        // RIGHT
        if (t.right && x + 1 < columns)
        {
            var nb = tiles[x + 1, y];
            if (nb != null && nb.left && !visited[x + 1, y])
                if (FollowPipe(x + 1, y, visited)) return true;
        }

        // DOWN
        if (t.down && y - 1 >= 0)
        {
            var nb = tiles[x, y - 1];
            if (nb != null && nb.up && !visited[x, y - 1])
                if (FollowPipe(x, y - 1, visited)) return true;
        }

        // LEFT
        if (t.left && x - 1 >= 0)
        {
            var nb = tiles[x - 1, y];
            if (nb != null && nb.right && !visited[x - 1, y])
                if (FollowPipe(x - 1, y, visited)) return true;
        }

        return false;
    }

    private void OnPuzzleCompleted()
    {
        //Debug.Log("PUZZLE COMPLETED - PLACE WIN LOGIC HERE");

        if (Fire1 != null)
            Fire1.SetActive(false);

        puzzle1UI.SetActive(false);
    }

    private void RandomizeTileRotation()
    {
        for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
            {
                var tile = tiles[x, y];
                if (tile == null) continue;
                if (tile.locked) continue;

                int max = (tile.asset == PipeTile.Asset.Straight) ? 2 : 4;
                tile.rotationStep = Random.Range(0, max);
                tile.ApplyVisual();
                tile.UpdateConnections();
            }
    }
}