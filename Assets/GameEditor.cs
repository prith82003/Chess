#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Game))]
public class GameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var game = target as Game;
        if (GUILayout.Button("Game Over"))
            game.StartCoroutine(game.GameOver());
    }
}
#endif