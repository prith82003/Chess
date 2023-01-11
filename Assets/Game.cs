using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Cell selectedCell;
    public static ChessColor PlayerColor;

    void Awake()
    {
        CellDisplay.OnCellSelect += DisplayMoves;
    }

    void Start()
    {
        PlayerColor = ChessColor.White;
    }

    void ClearDisplay()
    {
        var cells = FindObjectsOfType<CellDisplay>();
        foreach (var c in cells)
        {
            c.GetComponent<SpriteRenderer>().color = c.cell.boardColor;
        }
    }

    void DisplayMoves(Cell cell)
    {
        ClearDisplay();
        var moves = cell.GetMoves();
        foreach (var c in moves)
        {
            c.self.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    void MovePiece(Cell cA, Cell cB)
    {

    }

    private void Update()
    {

    }
}
