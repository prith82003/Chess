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

    // Board
    // Bottom left is 0, 0
    public static Cell[,] board;
    [SerializeField] GameObject cell;
    public Color BlackColor;
    public Color WhiteColor;
    public static Cell WhiteKing;
    public static Cell BlackKing;

    // Chess Pieces
    public List<Sprite> pieces;
    public string FENString;

    public static System.Action UpdateCell;

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

    /// <summary>
    /// Commit Genocide on all Children of Game Controller Object (Board Tiles)
    /// </summary>
    void DestroyChildren()
    {
        if (transform.childCount == 0)
            return;
        DestroyImmediate(transform.GetChild(0).gameObject);
        DestroyChildren();
    }

    public void ClearBoard()
    {
        // Reset Board
        DestroyChildren();

        if (board != null)
        {
            foreach (var cell in board)
                cell.Remove();
        }
        board = new Cell[BOARD_SIZE, BOARD_SIZE];

        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                var cellObj = Instantiate(cell, transform);
                var newCell = new Cell(ChessColor.White, ChessPiece.None, new Vector2Int(x, y), cellObj);
                cellObj.transform.position = GetCellPosition(x, y);
                cellObj.GetComponent<CellDisplay>().cell = newCell;
                board[x, y] = newCell;

                // Alternate Colors, Each Row Starts with Different Color
                var i = y + x;
                var col = (i % 2 != 0) ? WhiteColor : BlackColor;
                cellObj.GetComponent<SpriteRenderer>().color = col;
                newCell.boardColor = col;
            }
        }
    }

    /// <summary>
    /// Initialise the board
    /// Spawns cells and sets their color and piece
    /// </summary>
    public void GenerateBoard()
    {
        ClearBoard();

        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                // Spawns Object for Board Tile, Sets Position, Sets Color, Sets Piece
                ChessColor color = (y < 3) ? ChessColor.White : ChessColor.Black;
                ChessPiece piece = CalculateChessPiece(x, y);

                // Creates Cell Object to Represent Information
                board[x, y].color = color;
                board[x, y].piece = piece;
            }
        }
        UpdateCell();
    }

    /// <summary>
    /// Check if the Given Position is within the bounds of the board
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool CheckIfBounds(Vector2Int pos)
    {
        if (pos.x >= BOARD_SIZE || pos.x < 0 || pos.y >= BOARD_SIZE || pos.y < 0)
            return false;
        return true;
    }

    /// <summary>
    /// Loads a FEN String onto Board
    /// </summary>
    public void LoadFENString()
    {
        ClearBoard();

        var FENPiece = new Dictionary<char, ChessPiece>()
        {
            ['k'] = ChessPiece.King,
            ['q'] = ChessPiece.Queen,
            ['r'] = ChessPiece.Rook,
            ['b'] = ChessPiece.Bishop,
            ['n'] = ChessPiece.Horse,
            ['p'] = ChessPiece.Pawn
        };

        int boardIndex = 0;
        string FENBoard = FENString.Split(' ')[0];

        foreach (var c in FENBoard)
        {
            if (char.IsDigit(c))
            {
                boardIndex += int.Parse(c.ToString());
                continue;
            }

            int Rawrow = boardIndex / BOARD_SIZE;
            int col = boardIndex % BOARD_SIZE;

            int row = BOARD_SIZE - Rawrow - 1;

            // Go to next row if c is '/'
            if (c == '/')
                continue;

            ChessColor color = char.IsUpper(c) ? ChessColor.White : ChessColor.Black;
            ChessPiece piece = FENPiece[char.ToLower(c)];

            try
            {
                Debug.Log("Piece at (" + col + ", " + row + "): " + piece + ", Color: " + color + ", FEN char: " + c);

                board[col, row].color = color;
                board[col, row].piece = piece;
                boardIndex++;
            }
            catch
            {
                Debug.Log("Error at (" + col + ", " + row + ")");
            }

            Game.PlayerColor = (FENString.Split(' ')[1] == "w") ? ChessColor.White : ChessColor.Black;


            UpdateCell();
        }
    }
}
