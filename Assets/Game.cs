using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckState { None, Check, Checkmate };

public class Game : MonoBehaviour
{
    public static Cell selectedCell;
    static Cell[] validMoves;
    public static ChessColor PlayerColor;
    public Color emptyColor;
    public Color occupiedColor;

    public static System.Action<ChessColor, ChessPiece> OnTurnFinish;

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

    /// <summary>
    /// Clearing the Board of the Sprites that Display Potential Moves
    /// </summary>
    /// <param name="resetCell"></param>
    public static void ClearDisplay(bool resetCell = false)
    {
        // Debug.Log("Clearing Display Hard Reset: " + resetCell);
        if (resetCell)
        {
            validMoves = null;
            selectedCell = null;
        }

        foreach (var cell in Board.board)
        {
            cell.self.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    void DisplayMoves(Cell cell)
    {
        ClearDisplay();
        validMoves = cell.GetMoves();

        // Foreach valid move for that Cell, display the Sprite, If its empty use green else red
        foreach (var c in validMoves)
        {
            var display = c.self.transform.GetChild(1).GetComponent<SpriteRenderer>();
            if (c.piece != ChessPiece.None)
                display.color = occupiedColor;
            else
                display.color = emptyColor;

            c.self.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Move a Piece Between Cells
    /// </summary>
    /// <param name="cA">Cell to Move From</param>
    /// <param name="cB">Cell to Move To</param>
    void MovePiece(Cell cA, Cell cB)
    {
        foreach (var cell in validMoves)
        {
            if (cell == cB)
            {
                Cell[,] OldBoard = Board.board.Clone() as Cell[,];

                
                cB.piece = cA.piece;
                cB.color = cA.color;
                cA.piece = ChessPiece.None;


                if(SpecialMoves.IsInCheck(cB.color)){
                    Board.board = OldBoard;
                    Debug.Log("hello");
                }
                ClearDisplay(true);
                Board.UpdateCell();

                OnFinishTurn(cB.color, cB.piece);
                return;
            }
        }

        ClearDisplay(true);
    }

    void OnFinishTurn(ChessColor color, ChessPiece piece)
    {
        OnTurnFinish?.Invoke(color, piece);
        ChessColor nextColor = color == ChessColor.White ? ChessColor.Black : ChessColor.White;

        if (piece == ChessPiece.King || piece == ChessPiece.Rook)
            SpecialMoves.CheckCastle(color);

        foreach (var cell in Board.board)
        {
            cell.clickable = true;
        }

        PlayerColor = nextColor;
        OnTurnStart(nextColor);
    }

    void OnTurnStart(ChessColor color)
    {
        // if (SpecialMoves.IsInCheckmate(color))
        //     GameOver();

        Debug.Log("Color: " + color + ", Check: " + SpecialMoves.IsInCheck(color));

        if (SpecialMoves.IsInCheck(color))
        {
            var King = color == ChessColor.White ? Board.WhiteKing : Board.BlackKing;

            selectedCell = King;
            DisplayMoves(King);

            foreach (var cell in Board.board)
            {
                // if (cell != King)
                //     cell.clickable = false;
            }
        }
    }

    // TODO: Implement GameOver
    // TODO: Regenerating the Game Board
    void GameOver()
    {

    }
}