using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
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
    public bool doubleMove;
    public Vector2Int FENPos;

    // Initialises the Cell
    public Cell(ChessColor color, ChessPiece piece, Vector2Int position, GameObject self)
    {
        this.color = color;
        this.piece = piece;
        this.position = position;
        this.self = self;
        display = false;
        clickable = true;
        doubleMove = true;

        Board.UpdateCell += UpdateCell;
    }

    public void Remove() => Board.UpdateCell -= UpdateCell;

    /// <summary>
    /// Updates the cell's sprite
    /// </summary>
    void UpdateCell()
    {
        if (piece == ChessPiece.Pawn && (position.y == 0 || position.y == 7))
            piece = ChessPiece.Queen;

        if (piece == ChessPiece.None)
        {
            self.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            return;
        }

        var board = self.transform.parent.GetComponent<Board>();

        int pieceIndex = (int)piece;
        if (color == ChessColor.Black)
            pieceIndex += 6;
        self.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = board.pieces[pieceIndex];

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
    public Cell[] GetMoves(bool checkExtra = false, bool checkCheck = true)
    {
        // Debug.Log("Cell: (" + position.x + ", " + position.y + ") Piece: " + piece + " Color: " + color);
        // Retrieves the Types of Moves Each Piece Can Make

        Stopwatch sw = Stopwatch.StartNew();

        List<Moves> moves = new List<Moves>();
        switch (piece)
        {
            case ChessPiece.Pawn:
                moves.Add(new PawnMoves());
                if (checkExtra)
                    moves.Add(new DiagonalMoves());
                if (doubleMove)
                    moves.Add(new DoubleMove());
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
            int yOffset = color == ChessColor.White ? 1 : -1;
            Vector2Int pos = new Vector2Int(position.x + 1, position.y + yOffset);
            if (Board.CheckIfBounds(pos))
            {
                var cell = Board.board[pos.x, pos.y];
                if (cell.piece != ChessPiece.None && color != cell.color)
                    cells.Add(cell);
            }
            pos = new Vector2Int(position.x - 1, position.y + yOffset);
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
                        continue;

                    if (checkCheck && !IsLegalMove(cell))
                        continue;

                    if (move.GetType() == (new DoubleMove()).GetType())
                    {
                        int yOffset = color == ChessColor.White ? 1 : -1;
                        // Check if there is a piece in the way
                        if (Board.board[position.x, position.y + yOffset].piece != ChessPiece.None)
                            continue;
                    }

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
                    if (cell.piece != ChessPiece.None && cell.color == color)
                        break;

                    if (checkCheck && !IsLegalMove(cell))
                        continue;

                    cells.Add(cell);

                    // Stops the Loop if Enemy Piece is Blocking the Move
                    if (cell.piece != ChessPiece.None)
                        break;
                }
            }
        }

        sw.Stop();
        RandomTester.WriteLine(sw.ElapsedTicks, checkCheck);
        return cells.ToArray();
    }

    bool IsLegalMove(Cell cell)
    {
        // Check if After this cell moves to the cell given in the argument, if there is a check
        // If there is a check, then the move is illegal
        // If there is no check, then the move is legal

        // Save the Piece that is in the Cell
        ChessPiece Movepiece = cell.piece;
        ChessColor Movecolor = cell.color;

        // Move the Piece to the Cell
        cell.piece = this.piece;
        cell.color = this.color;

        // Remove the Piece from the Current Cell
        this.piece = ChessPiece.None;

        // Check if the King is in Check
        bool check = SpecialMoves.IsInCheck(color);

        // Move the Piece Back to the Current Cell
        this.piece = cell.piece;
        this.color = cell.color;

        // Move the Piece to the Cell
        cell.piece = Movepiece;
        cell.color = Movecolor;

        if (check)
        {
            UnityEngine.Debug.Log("This Move Would Cause A Check On: " + color);
            UnityEngine.Debug.Log("Current Position: " + position);
            UnityEngine.Debug.Log("Future Position: " + cell.position);
            UnityEngine.Debug.Log("Piece: " + this.piece);
            UnityEngine.Debug.Log("Piece Color: " + this.color);
        }

        return !check;
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

public class DoubleMove : Moves
{
    internal override Vector2Int[] GetMoves()
    {
        return new Vector2Int[] { new Vector2Int(0, 2) };
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

