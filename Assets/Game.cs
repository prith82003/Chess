using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player { White, Black };

public class Game : MonoBehaviour
{
    public static Cell selectedCell;

    Cell SelectCell()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<CellDisplay>() != null)
            {
                var cell = hit.collider.gameObject.GetComponent<CellDisplay>().cell;
                return cell;
            }
        }

        return null;
    }

    void ClearDisplay()
    {
        var cells = FindObjectsOfType<CellDisplay>();
        foreach (var c in cells)
        {
            c.GetComponent<SpriteRenderer>().color = c.cell.boardColor;
        }
    }

    void DisplayMoves(Cell cell)
    {
        var moves = cell.GetMoves();
        foreach (var c in moves)
        {
            c.self.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClearDisplay();
            var cell = SelectCell();
            if (cell != null)
            {
                Debug.Log("Selected Cell: (" + cell.position.x + ", " + cell.position.y + ")");
                DisplayMoves(cell);
            }
        }
    }
}
