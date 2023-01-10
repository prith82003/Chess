using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script initialises the board and pieces for the chess game

public class Board : MonoBehaviour
{
    public Vector2 firstPosition;
    public const int BOARD_SIZE = 8;
    static float cameraSize = Camera.main.orthographicSize;
    static float cellSize = cameraSize / BOARD_SIZE;

    [SerializeField] GameObject cell;
    public static Vector2 spawnPosition
    {
        get
        {
            var pos = cellSize * (BOARD_SIZE / 2) - (cellSize / 2);
            return Vector2.one * pos;
        }
    }

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

    void CreateBoard()
    {
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                var cellObj = Instantiate(cell, transform);
                cellObj.transform.position = GetCellPosition(x, y);
            }
        }
    }
}
