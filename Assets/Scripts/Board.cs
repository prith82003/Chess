using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public static System.Action UpdateCell;
    public string FENString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w";

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

    Dictionary<char, ChessPiece> FENPiece = new Dictionary<char, ChessPiece>()
    {
        ['k'] = ChessPiece.King,
        ['q'] = ChessPiece.Queen,
        ['r'] = ChessPiece.Rook,
        ['b'] = ChessPiece.Bishop,
        ['n'] = ChessPiece.Horse,
        ['p'] = ChessPiece.Pawn
    };

    /// <summary>
    /// Loads a FEN Sequence onto Board
    /// More Information: https://www.chess.com/terms/fen-chess
    /// </summary>
    public void LoadFENString()
    {
        ClearBoard();

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

            // TODO: Add Castling, En Passant, Half Move Clock, Full Move Clock
        }
    }

    /// <summary>
    /// Outputs the current Board Configuration as FEN Sequence
    /// </summary>
    public void WriteFENString()
    {
        string FENString = "";

        for (int y = BOARD_SIZE - 1; y >= 0; y--)
        {
            int emptyCount = 1;
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                var cell = board[x, y];

                if (cell.piece == ChessPiece.None)
                {
                    if (x == BOARD_SIZE - 1 && y != 0)
                        FENString += emptyCount + "/";
                    else
                        emptyCount++;
                    continue;
                }
                else
                {
                    if (emptyCount > 1)
                    {
                        FENString += emptyCount;
                        emptyCount = 1;
                    }
                }

                var piece = FENPiece.FirstOrDefault(p => p.Value == cell.piece).Key;
                if (cell.color == ChessColor.White)
                    piece = char.ToUpper(piece);

                FENString += piece;
                if (x == BOARD_SIZE - 1 && y != 0)
                    FENString += "/";
            }
        }

        // TODO: Add Castling, En Passant, Half Move Clock, Full Move Clock

        FENString += " " + ((Game.PlayerColor == ChessColor.White) ? "w" : "b") + " ---- - 0 1";
        Debug.Log("FEN String: " + FENString);
    }

    public static Cell GetRandomPiece(ChessColor color)
    {
        // Get a random cell with a piece of color on board
        var cells = board.Cast<Cell>().Where(c => c.color == color && c.piece != ChessPiece.None).ToList();
        return cells[Random.Range(0, cells.Count)];
    }

    public static List<Cell> GetPieces(ChessColor color)
    {
        return board.Cast<Cell>().Where(c => c.color == color && c.piece != ChessPiece.None).ToList();
    }
}
