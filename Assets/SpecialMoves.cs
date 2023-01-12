using UnityEngine;

public class SpecialMoves
{

    /// <summary>
    /// Returns if the color is in check, checkmate, or none
    /// </summary>
    /// <param name="color">Color to check Check for</param>
    /// <returns></returns>
    public static bool IsInCheck(ChessColor color, Cell kingCell = null)
    {
        ChessColor enemyColor = color == ChessColor.White ? ChessColor.Black : ChessColor.White;

        // If the king cell is not specified, find it
        if (kingCell == null)
            kingCell = color == ChessColor.White ? Board.WhiteKing : Board.BlackKing;

        Debug.Log("King Cell: " + kingCell.position);

        // Check each of enemy's potential moves

        // Loop through each tile on the board
        foreach (var c in Board.board)
        {

            // If the tile is occupied by an enemy piece
            if (c.color == enemyColor && c.piece != ChessPiece.None && c.piece != ChessPiece.King)
            {
                // Debug.Log();
                Cell[] moves = c.GetMoves(true);

                // Loop through each move the enemy piece can make
                foreach (var move in moves)
                {

                    // If the move is a king of the same color as the color being checked
                    if (move == kingCell)
                        return true;
                }

                moves = null;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks If the King is in Checkmate
    /// </summary>
    /// <param name="color">The Color to Check Checkmate for</param>
    /// <returns></returns>
    public static bool IsInCheckmate(ChessColor color)
    {
        Cell King = color == ChessColor.White ? Board.WhiteKing : Board.BlackKing;
        var KingMoves = King.GetMoves();

        if (KingMoves.Length == 0)
        {
            var c = IsInCheck(color);
            if (c)
            {
                Debug.LogError("Checkmate: " + color);
                Debug.Break();
                return c;
            }
            return false;
        }

        foreach (var move in KingMoves)
        {
            if (!IsInCheck(color, move))
            {
                Debug.LogWarning("Not In Check at: " + move.position);
                return false;
            }
        }

        Debug.LogError("Checkmate: " + color);
        Debug.Break();
        return true;
    }

    // TODO: Implement Castling
    public static void CheckCastle(ChessColor color)
    {

    }
}