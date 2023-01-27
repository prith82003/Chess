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

        if (GUILayout.Button("Random Test"))
        {
            RandomTester.Initialise();
            RandomTester.MakeMoves(game.testIterations);
        }

        if (GUILayout.Button("Graph"))
        {
            RandomTester.AnalyseData(game.dataFile);
        }
    }
}
#endif