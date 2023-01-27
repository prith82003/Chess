using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CheckState { None, Check, Checkmate };

public class Game : MonoBehaviour
{
    public static Cell selectedCell;
    static Cell[] validMoves;
    public static ChessColor PlayerColor;
    public Color emptyColor;
    public Color occupiedColor;
    public static bool isPaused;
    public int testIterations;
    public TextAsset dataFile;

    public static System.Action<ChessColor, ChessPiece> OnTurnFinish;

    void Awake()
    {
        CellDisplay.OnCellSelect += DisplayMoves;
        CellDisplay.OnCellClick += MovePiece;
    }

    void Start()
    {
        RestartGame();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            Resume();
        }
    }

    /// <summary>
    /// Clearing the Board of the Sprites that Display Potential Moves
    /// </summary>
    /// <param name="resetCell"></param>
    public static void ClearDisplay(bool resetCell = false)
    {
        Debug.Log("Clearing Display Hard Reset: " + resetCell);
        if (resetCell)
        {
            validMoves = null;
            selectedCell = null;
        }

        foreach (var cell in Board.board)
        {
            cell.self.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    void DisplayMoves(Cell cell)
    {
        ClearDisplay();
        validMoves = cell.GetMoves();

        // Foreach valid move for that Cell, display the Sprite, If its empty use green else red
        foreach (var c in validMoves)
        {
            var display = c.self.transform.GetChild(1).GetComponent<SpriteRenderer>();
            if (c.piece != ChessPiece.None)
                display.color = occupiedColor;
            else
                display.color = emptyColor;

            c.self.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Move a Piece Between Cells
    /// </summary>
    /// <param name="cA">Cell to Move From</param>
    /// <param name="cB">Cell to Move To</param>
    public void MovePiece(Cell cA, Cell cB, bool force = false)
    {
        if (force)
        {
            cB.piece = cA.piece;
            cB.color = cA.color;
            cA.piece = ChessPiece.None;
            cA.doubleMove = false;
            cB.doubleMove = false;
            ClearDisplay(true);
            Board.UpdateCell();

            OnFinishTurn(cB.color, cB);
            return;
        }

        foreach (var cell in validMoves)
        {
            if (cell == cB)
            {
                cB.piece = cA.piece;
                cB.color = cA.color;
                cA.piece = ChessPiece.None;
                cA.doubleMove = false;
                cB.doubleMove = false;
                ClearDisplay(true);
                Board.UpdateCell();

                OnFinishTurn(cB.color, cB);
                return;
            }
        }

        ClearDisplay(true);
    }

    public void Swap(Cell cA, Cell cB)
    {
        var tempPiece = cA.piece;
        var tempColor = cA.color;
        cA.piece = cB.piece;
        cA.color = cB.color;
        cB.piece = tempPiece;
        cB.color = tempColor;
        ClearDisplay(true);
        Board.UpdateCell();
    }

    public void OnFinishTurn(ChessColor color, Cell cell)
    {
        OnTurnFinish?.Invoke(color, cell.piece);
        ChessColor nextColor = color == ChessColor.White ? ChessColor.Black : ChessColor.White;

        if (cell.piece == ChessPiece.King || cell.piece == ChessPiece.Rook)
            SpecialMoves.CheckCastle(cell);

        PlayerColor = nextColor;
        OnTurnStart(nextColor);
    }

    public void OnTurnStart(ChessColor color)
    {
        if (SpecialMoves.IsInCheckmate(color))
        {
            StartCoroutine(GameOver());
            return;
        }

        Debug.Log("Color: " + color + ", Check: " + SpecialMoves.IsInCheck(color));
    }

    // UI

    public GameObject GameOverPanel;
    public GameObject Screen;
    public const float PanelAlpha = 220f;
    public float PanelFadeSpeed = 0.5f;

    public IEnumerator GameOver()
    {
        Debug.Break();
        GameOverPanel.SetActive(true);
        var panel = GameOverPanel.GetComponent<Image>();
        var color = panel.color;
        color.a = 0;
        panel.color = color;

        while (color.a < ((PanelAlpha + PanelFadeSpeed * Time.deltaTime) / 255f))
        {
            Debug.Log("Color A: " + color.a);
            color.a += PanelFadeSpeed * Time.deltaTime;
            panel.color = color;
            yield return null;
        }

        Screen.SetActive(true);
    }

    public void Pause()
    {
        GameOverPanel.SetActive(true);
        Screen.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        GameOverPanel.SetActive(false);
        Screen.SetActive(false);
        isPaused = false;
    }

    public void RestartGame()
    {
        isPaused = false;
        PlayerColor = ChessColor.White;
        selectedCell = null;
        validMoves = null;
        GameOverPanel.SetActive(false);
        Screen.SetActive(false);
        FindObjectOfType<Board>().GenerateBoard();
        // OnTurnStart(PlayerColor);
    }

    public static void Quit() => Application.Quit();
}