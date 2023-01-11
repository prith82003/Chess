using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Cell selectedCell;
    static Cell[] validMoves;
    public static ChessColor PlayerColor;

    void Awake()
    {
        CellDisplay.OnCellSelect += DisplayMoves;
        CellDisplay.OnCellClick += MovePiece;
    }

    void Start()
    {
        PlayerColor = ChessColor.White;
        selectedCell = null;
        validMoves = null;
    }

    public static void ClearDisplay(bool resetCell = false)
    {
        Debug.Log("Clearing Display Hard Reset: " + resetCell);
        if (resetCell)
        {
            validMoves = null;
            selectedCell = null;
        }

        var cells = FindObjectsOfType<CellDisplay>();

        foreach (var c in cells)
        {
            c.GetComponent<SpriteRenderer>().color = c.cell.boardColor;
        }
    }

    void DisplayMoves(Cell cell)
    {
        ClearDisplay();
        validMoves = cell.GetMoves();
        foreach (var c in validMoves)
        {
            c.self.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    void MovePiece(Cell cA, Cell cB)
    {
        foreach (var cell in validMoves)
        {
            if (cell == cB)
            {
                cB.piece = cA.piece;
                cA.piece = ChessPiece.None;
                cB.color = cA.color;
                ClearDisplay(true);
                Board.UpdateCell();
                return;
            }
        }

        ClearDisplay(true);
    }
}
