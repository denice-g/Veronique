using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {

    public static PuzzleManager Instance { get; private set; }

    private Dictionary<string, bool> puzzles = new Dictionary<string, bool>();

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        puzzles["Room1"] = false;
    }

    public bool IsPuzzleComplete(string puzzleName) {
        return puzzles.ContainsKey(puzzleName) && puzzles[puzzleName];
    }

    public void SetPuzzleComplete(string puzzleName) {
        if(puzzles.ContainsKey(puzzleName)) {
            puzzles[puzzleName] = true;
            Debug.Log($"Puzzle '{puzzleName}' marked complete!");
        }
        else {
            Debug.LogWarning($"Puzzle '{puzzleName}' not foudn in PuzzleManager!");
        }
    }

    public void RegisterPuzzle(string puzzleName) {
        if(!puzzles.ContainsKey(puzzleName)) {
            puzzles[puzzleName] = false;
        }
    }
}