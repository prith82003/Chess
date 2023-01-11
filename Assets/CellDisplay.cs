using UnityEngine;

public class CellDisplay : MonoBehaviour
{
    public Cell cell;
    public static System.Action<Cell, Cell> OnCellClick;
    public static System.Action<Cell> OnCellSelect;

    private void OnMouseDown()
    {
        if (Game.selectedCell != null)
        {
            Debug.Log("Selected Cell: " + Game.selectedCell.position);
            Debug.Log(".Clicked: " + cell.position);
        }
        else
            Debug.Log("Selected Cell: null");


        if (cell.color == Game.PlayerColor && cell.piece != ChessPiece.None)
        {
            Debug.Log("Selected Own Piece");
            Game.selectedCell = cell;
            Debug.Log("Selected Cell: " + Game.selectedCell.position);
            OnCellSelect?.Invoke(cell);
        }
        else if (Game.selectedCell != null)
        {
            Debug.Log("Selecting Move Piece");
            if (cell.piece == ChessPiece.None)
            {
                Debug.Log("Attempting Move: " + Game.selectedCell.position + " -> " + cell.position);
                if (Game.selectedCell == null)
                    Debug.Log("Selected Cell: null");
                if (cell == null)
                    Debug.Log("Clicked Cell: null");
                OnCellClick(Game.selectedCell, cell);
            }
            else
            {
                if (cell.color == Game.PlayerColor)
                {
                    Debug.Log("Selected Own Piece");
                    Game.selectedCell = cell;
                    OnCellSelect?.Invoke(cell);
                }
                else
                {
                    Debug.Log("Selected Enemy Piece");
                    Debug.Log("Attempting Move: " + Game.selectedCell.position + " -> " + cell.position);
                    OnCellClick(Game.selectedCell, cell);
                }
            }
        }
    }
}
