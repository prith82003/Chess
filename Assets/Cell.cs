using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ChessPiece { None = 9, Pawn = 0, Rook = 1, Horse = 2, Bishop = 3, Queen = 4, King = 5 };
public enum ChessColor { White, Black };

[System.Serializable]
public class Cell
{
    public ChessColor color;
    public ChessPiece piece;
    public Vector2Int position;
    public Color boardColor;

    public GameObject self;
    public bool display;
    public bool clickable;

    // Initialises the Cell
    public Cell(ChessColor color, ChessPiece piece, Vector2Int position, GameObject self)
    {
        this.color = color;
        this.piece = piece;
        this.position = position;
        this.self = self;
        display = false;
        clickable = true;

        Board.UpdateCell += UpdateCell;
    }

    public void Remove()
    {
        Board.UpdateCell -= UpdateCell;
    }

    /// <summary>
    /// Updates the cell's sprite
    /// </summary>
    void UpdateCell()
    {
        if (piece == ChessPiece.None)
        {
            self.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            return;
        }

        int pieceIndex = (int)piece;
        if (color == ChessColor.Black)
            pieceIndex += 6;
        self.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Board.pieces[pieceIndex];

        // Tracks King Positions
        if (piece == ChessPiece.King && color == ChessColor.White)
            Board.WhiteKing = this;
        else if (piece == ChessPiece.King && color == ChessColor.Black)
            Board.BlackKing = this;
    }

    /// <summary>
    /// Returns an Array of Cells that the Piece can Move To
    /// </summary>
    /// <returns>All Valid Cells the Piece can Move To</returns>
    public Cell[] GetMoves(bool checkExtra = false)
    {

        // Debug.Log("Cell: (" + position.x + ", " + position.y + ") Piece: " + piece + " Color: " + color);

        // Retrieves the Types of Moves Each Piece Can Make
        List<Moves> moves = new List<Moves>();
        switch (piece)
        {
            case ChessPiece.Pawn:
                moves.Add(new PawnMoves());
                if (checkExtra)
                    moves.Add(new DiagonalMoves());
                break;
            case ChessPiece.Rook:
                moves.Add(new BaseMoves());
                break;
            case ChessPiece.Horse:
                moves.Add(new HorseMoves());
                break;
            case ChessPiece.Bishop:
                moves.Add(new DiagonalMoves());
                break;
            case ChessPiece.Queen:
                moves.Add(new BaseMoves());
                moves.Add(new DiagonalMoves());
                break;
            case ChessPiece.King:
                moves.Add(new BaseMoves());
                moves.Add(new DiagonalMoves());
                break;
        }

        List<Cell> cells = new List<Cell>();

        // Check if piece exists diagonally to eliminate
        if (piece == ChessPiece.Pawn)
        {
            Vector2Int pos = new Vector2Int(position.x + 1, position.y + 1);
            if (Board.CheckIfBounds(pos))
            {
                var cell = Board.board[pos.x, pos.y];
                if (cell.piece != ChessPiece.None && color != cell.color)
                    cells.Add(cell);
            }
            pos = new Vector2Int(position.x - 1, position.y + 1);
            if (Board.CheckIfBounds(pos))
            {
                var cell = Board.board[pos.x, pos.y];
                if (cell.piece != ChessPiece.None && color != cell.color)
                    cells.Add(cell);
            }

        }

        foreach (var move in moves)
        {
            foreach (var b in move.GetMoves())
            {
                var m = b;
                // Inverts the Y Axis if the Piece is Black
                if (color == ChessColor.Black)
                    m.y *= -1;

                // Handles the Pieces That Can Move Only Once
                if (piece == ChessPiece.Pawn || piece == ChessPiece.Horse || piece == ChessPiece.King)
                {
                    Vector2Int pos = new Vector2Int(position.x + m.x, position.y + m.y);
                    if (!Board.CheckIfBounds(pos))
                        continue;

                    // Check's If a Piece is Blocking the Move
                    var cell = Board.board[pos.x, pos.y];
                    if (cell.piece != ChessPiece.None && (cell.color == Game.PlayerColor || piece == ChessPiece.Pawn))
                        continue;

                    if (piece == ChessPiece.King && SpecialMoves.IsInCheck(color, cell))
                    {
                        Debug.Log("Skipped: (" + cell.position.x + ", " + cell.position.y + ")");
                        continue;
                    }

                    if (cell != null)
                        cells.Add(cell);

                    continue;
                }

                // Handles the Pieces That Can Move Multiple Times
                // Loops Since Piece Can Move Multiple Steps
                for (int i = 1; i < 8; i++)
                {
                    // Multiplied By i, Imagine as Vector with Direction that is Multiplied by i 
                    // to Increase Magnitude Step by Step
                    Vector2Int pos = new Vector2Int(position.x + m.x * i, position.y + m.y * i);
                    if (!Board.CheckIfBounds(pos))
                        break;
                    var cell = Board.board[pos.x, pos.y];

                    // Check if Friendly Piece is Blocking the Move
                    if (cell.piece != ChessPiece.None && cell.color == Game.PlayerColor)
                        break;
                    cells.Add(cell);

                    // Stops the Loop if Enemy Piece is Blocking the Move
                    if (cell.piece != ChessPiece.None)
                        break;
                }
            }
        }

        return cells.ToArray();
    }
}

// Each class Represents the Type of Move that can be Made
// PawnMoves = Moves that a Pawn can Make
// BaseMoves = Horizontal and Vertical Moves
// DiagonalMoves = Diagonal Moves
// HorseMoves = Moves that a Horse can Make
public abstract class Moves
{
    internal abstract Vector2Int[] GetMoves();
}

public class PawnMoves : Moves
{
    internal override Vector2Int[] GetMoves()
    {
        return new Vector2Int[] { new Vector2Int(0, 1) };
    }
}

public class BaseMoves : Moves
{
    internal override Vector2Int[] GetMoves()
    {
        return new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
    }
}

public class DiagonalMoves : Moves
{
    internal override Vector2Int[] GetMoves()
    {
        return new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1) };
    }
}

public class HorseMoves : Moves
{
    internal override Vector2Int[] GetMoves()
    {
        return new Vector2Int[] { new Vector2Int(-2, 1), new Vector2Int(-1, 2), new Vector2Int(1, 2), new Vector2Int(2, 1), new Vector2Int(2, -1), new Vector2Int(1, -2), new Vector2Int(-1, -2), new Vector2Int(-2, -1) };
    }
}

