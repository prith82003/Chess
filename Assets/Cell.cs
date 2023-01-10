using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public enum ChessPiece { None, Pawn, Rook, Knight, Bishop, Queen, King };
    public enum ChessColor { White, Black };
    public ChessColor color;
    public ChessPiece piece;

    public Cell(ChessColor color, ChessPiece piece)
    {
        this.color = color;
        this.piece = piece;
    }

    public void SetPiece(ChessPiece piece) => this.piece = piece;
    public void SetColor(ChessColor color) => this.color = color;
}
