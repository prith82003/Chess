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

    public Cell(ChessColor color, ChessPiece piece, Vector2Int position, GameObject self)
    {
        this.color = color;
        this.piece = piece;
        this.position = position;
        this.self = self;

        Board.UpdateCell += UpdateCell;
    }

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
    }

    public Cell[] GetMoves()
    {
        List<Moves> moves = new List<Moves>();
        switch (piece)
        {
            case ChessPiece.Pawn:
                moves.Add(new PawnMoves());
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

        if (piece == ChessPiece.Pawn) // Check if piece exists diagonally to eliminate
        {
            var cell = Board.board[position.x + 1, position.y + 1];
            if (cell != null && cell.piece != ChessPiece.None)
                cells.Add(cell);
            cell = Board.board[position.x - 1, position.y + 1];
            if (cell != null && cell.piece != ChessPiece.None)
                cells.Add(cell);
        }

        foreach (var move in moves)
        {
            foreach (var m in move.GetMoves())
            {
                if (piece == ChessPiece.Pawn || piece == ChessPiece.Horse || piece == ChessPiece.King) //Only one move
                {
                    var cell = Board.board[position.x + m.x, position.y + m.y];
                    if (cell != null)
                        cells.Add(cell);
                    break;
                }

                for (int i = 1; i < 8; i++)
                {
                    var cell = Board.board[position.x + m.x * i, position.y + m.y * i];
                    if (cell == null)
                        break;
                    cells.Add(cell);
                    if (cell.piece != ChessPiece.None)
                        break;
                }
            }
        }

        return cells.ToArray();
    }
}

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

