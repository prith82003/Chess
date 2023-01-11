using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var board = target as Board;
        if (GUILayout.Button("Generate Board"))
            board.GenerateBoard();
    }
}
