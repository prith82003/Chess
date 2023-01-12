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

        // Check each of enemy's potential moves

        // Loop through each tile on the board
        foreach (var c in Board.board)
        {

            // If the tile is occupied by an enemy piece
            if (c.color == enemyColor && c.piece != ChessPiece.None)
            {
                var moves = c.GetMoves();

                // Loop through each move the enemy piece can make
                foreach (var move in moves)
                {

                    // If the move is a king of the same color as the color being checked
                    if (move == kingCell)
                    {
                        return true;
                    }
                }
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

        foreach (var move in King.GetMoves())
        {
            if (!IsInCheck(color, move))
                return false;
        }

        return true;
    }

    public static void CheckCastle(ChessColor color)
    {

    }
}