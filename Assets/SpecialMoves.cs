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

        // Debug.Log("King Cell: " + kingCell.position);

        // Check each of enemy's potential moves

        // Loop through each tile on the board
        foreach (var c in Board.board)
        {

            // If the tile is occupied by an enemy piece
            if (c.color == enemyColor && c.piece != ChessPiece.None && c.piece != ChessPiece.King)
            {
                // Debug.Log();
                Cell[] moves = c.GetMoves(true, false);

                // Loop through each move the enemy piece can make
                foreach (var move in moves)
                {

                    // If the move is a king of the same color as the color being checked
                    if (move == kingCell)
                    {
                        Debug.LogError("Check: " + color);
                        Debug.Log("King Cell: " + kingCell.position);
                        Debug.Log("Enemy Color: " + enemyColor);
                        Debug.Log("Check Cell: " + move.position);

                        return true;
                    }
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
        // If the color is not in check, return false
        if (!IsInCheck(color))
            return false;

        // Loop through each tile on the board
        foreach (var cell in Board.board)
        {
            if (cell.color == color && cell.piece != ChessPiece.None && cell.piece != ChessPiece.King)
            {
                // If Legal Moves Still Exist For Any Piece, Must not Be Checkmate
                Cell[] moves = cell.GetMoves(true);

                if (moves.Length > 0)
                    return false;
            }
        }

        return true;
    }

    // TODO: Implement Castling
    public static void CheckCastle(Cell cell)
    {
        var posLeft = new Vector2Int(cell.position.x - 1, cell.position.y);
        var posRight = new Vector2Int(cell.position.x + 1, cell.position.y);


        if (Board.CheckIfBounds(posLeft))
        {
            var cellLeft = Board.board[posLeft.x, posLeft.y];
            if ((cellLeft.piece == ChessPiece.Rook || cellLeft.piece == ChessPiece.King) && cellLeft.color == cell.color)
            {
                Debug.Log("Swap Left. Cell: " + cell.piece + ", Left: " + cellLeft.piece);
                GameObject.FindObjectOfType<Game>().Swap(cellLeft, cell);
                return;
            }
        }

        if (Board.CheckIfBounds(posRight))
        {
            var cellRight = Board.board[posRight.x, posRight.y];
            if ((cellRight.piece == ChessPiece.Rook || cellRight.piece == ChessPiece.King) && cellRight.color == cell.color)
            {
                Debug.Log("Swap Right. Cell: " + cell.piece + ", Right: " + cellRight.piece);
                GameObject.FindObjectOfType<Game>().Swap(cellRight, cell);
            }
        }


    }
}