using UnityEngine;

public class CellDisplay : MonoBehaviour
{
    public Cell cell;
    public static System.Action<Cell, Cell> OnCellClick;
    public static System.Action<Cell> OnCellSelect;

    /// <summary>
    /// If the Mouse has been Clicked on this Cell, then select it
    /// </summary>
    private void OnMouseDown()
    {

        if (Game.isPaused)
            return;

        // Deselecting the Current Cell if Clicked on Twice
        if (Game.selectedCell != null)
        {
            if (Game.selectedCell == cell)
            {
                Game.selectedCell = null;
                Game.ClearDisplay();
                return;
            }
        }

        // Selecting a Piece
        if (cell.color == Game.PlayerColor && cell.piece != ChessPiece.None)
        {
            if (!cell.clickable)
                return;

            // Debug.Log("Selected Own Piece");
            Game.selectedCell = cell;
            // Debug.Log("Selected Cell: " + Game.selectedCell.position);
            OnCellSelect?.Invoke(cell);
        }
        else if (Game.selectedCell != null)
        {
            // Selecting a Move to an Empty Cell
            // Debug.Log("Selecting Move Piece");
            if (cell.piece == ChessPiece.None)
            {
                // Debug.Log("Attempting Move: " + Game.selectedCell.position + " -> " + cell.position);
                OnCellClick(Game.selectedCell, cell);
            }
            else
            {
                // Changing Selection to Different Piece
                if (cell.color == Game.PlayerColor)
                {
                    if (!cell.clickable)
                        return;

                    // Debug.Log("Selected Own Piece");
                    Game.selectedCell = cell;
                    OnCellSelect?.Invoke(cell);
                }
                // Selecting a Move to an Enemy Cell
                else
                {
                    // Debug.Log("Selected Enemy Piece");
                    // Debug.Log("Attempting Move: " + Game.selectedCell.position + " -> " + cell.position);
                    OnCellClick(Game.selectedCell, cell);
                }
            }
        }
    }
}
