using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script initialises the board and pieces for the chess game

public class Board : MonoBehaviour
{
    public Vector2 firstPosition;
    public const int BOARD_SIZE = 8;
    static float cameraSize
    {
        get => Camera.main.orthographicSize * 2;
    }
    static float cellSize
    {
        get => cameraSize / BOARD_SIZE;
    }
    public static Vector2 spawnPosition
    {
        get
        {
            var pos = cellSize * (BOARD_SIZE / 2) - (cellSize / 2);
            return Vector2.one * pos;
        }
    }

    // Bottom left is 0, 0
    [SerializeField] GameObject cell;
    public static Cell[,] board;

    // Chess Pieces
    public List<Sprite> dispPieces;
    public static List<Sprite> pieces;

    public static System.Action UpdateCell;

    private void OnValidate()
    {
        pieces = dispPieces;
        Debug.Log("Pieces: " + pieces.Count);
    }

    /// <summary>
    /// Transform index to position
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="y">y position</param>
    /// <returns></returns>
    public static Vector2 GetCellPosition(int x, int y)
    {
        return new Vector2(x, y) * cellSize - spawnPosition;
    }


    /// <summary>
    /// Returns the chess piece that should be placed 
    /// at the given position at start of game
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    ChessPiece CalculateChessPiece(int x, int y)
    {
        if (y == 1 || y == 6)
        {
            Debug.Log("Pawn at (" + x + ", " + y + ")");
            return ChessPiece.Pawn;
        }

        if (y == 0 || y == 7)
        {
            if (x == 0 || x == 7)
                return ChessPiece.Rook;

            if (x == 1 || x == 6)
                return ChessPiece.Horse;

            if (x == 2 || x == 5)
                return ChessPiece.Bishop;

            if (x == 3)
                return ChessPiece.Queen;

            if (x == 4)
                return ChessPiece.King;
        }
        return ChessPiece.None;
    }

    void DestroyChildren()
    {
        if (transform.childCount == 0)
            return;
        DestroyImmediate(transform.GetChild(0).gameObject);
        DestroyChildren();
    }

    public Color BlackColor;
    public Color WhiteColor;

    /// <summary>
    /// Initialise the board
    /// Spawns cells and sets their color and piece
    /// </summary>

    public void GenerateBoard()
    {
        DestroyChildren();
        board = new Cell[BOARD_SIZE, BOARD_SIZE];

        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                var cellObj = Instantiate(cell, transform);
                cellObj.transform.position = GetCellPosition(x, y);
                ChessColor color = (y < 3) ? ChessColor.White : ChessColor.Black;
                ChessPiece piece = CalculateChessPiece(x, y);
                var newCell = new Cell(color, piece, new Vector2Int(x, y), cellObj);
                cellObj.GetComponent<CellDisplay>().cell = newCell;
                board[x, y] = newCell;

                //Alternate color of cell white to black
                var i = y * BOARD_SIZE + x;
                if (y % 2 == 0)
                    i++;

                var col = (i % 2 == 0) ? WhiteColor : BlackColor;
                cellObj.GetComponent<SpriteRenderer>().color = col;
                newCell.boardColor = col;
            }
        }
        UpdateCell();
    }

    void Start() => GenerateBoard();
}
