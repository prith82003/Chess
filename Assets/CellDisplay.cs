using UnityEngine;

public class CellDisplay : MonoBehaviour
{
    public Cell cell;
    public static System.Action<Cell> OnCellClick;
    public static System.Action<Cell> OnCellSelect;

    private void OnMouseDown()
    {
        Debug.Log(".Clicked: " + cell.position);
        if (Game.selectedCell == null)
        {
            Game.selectedCell = cell;
            OnCellSelect?.Invoke(cell);
        }
        else
        {
            OnCellClick?.Invoke(cell);
            Game.selectedCell = null;
        }
    }
}
