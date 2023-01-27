using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class RandomTester
{
    static string FilePath = "./Assets/DataFiles/Test";
    static string FilePathSfx = ".bytes";
    static StreamWriter writer;

    static List<Cell> Pieces = new List<Cell>();

    public static void Initialise()
    {
        // Create Save File
        string pathName = FilePath + FilePathSfx;
        int diffNum = 0;

        while (File.Exists(pathName))
        {
            pathName = FilePath + diffNum + FilePathSfx;
            diffNum++;
        }

        writer = File.CreateText(pathName);
    }

    public static void MakeMoves(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            if (MakeMove(ChessColor.White, true) == 1)
            {
                UnityEngine.Debug.Log("No Moves Left");
                break;
            }
            if (MakeMove(ChessColor.Black, true) == 1)
            {
                UnityEngine.Debug.Log("No Moves Left");
                break;
            }
        }

        writer.Close();
    }

    public static int MakeMove(ChessColor color, bool reset = false)
    {
        if (reset)
        {
            Pieces.Clear();
            Pieces = Board.GetPieces(color);
        }

        if (Pieces.Count == 0)
            return 1;

        // Get Random Piece of color
        var piece = Pieces[Random.Range(0, Pieces.Count)];

        // Get Random Move
        var moves = piece.GetMoves();

        if (moves.Length == 0)
        {
            Pieces.Remove(piece);
            return MakeMove(color);
        }

        var rand = Random.Range(0, moves.Length);
        UnityEngine.Debug.Log("Moves Length: " + moves.Length + " Rand: " + rand);

        var move = moves[rand];
        GameObject.FindObjectOfType<Game>().MovePiece(piece, move, true);
        return 0;
    }

    public static void AnalyseData(TextAsset dataFile)
    {
        if (dataFile == null)
            return;

        StreamReader reader = new StreamReader(new MemoryStream(dataFile.bytes));
        List<Data> data = new List<Data>();

        while (!reader.EndOfStream)
        {
            var num = reader.ReadLine().Split(',');
            var time = long.Parse(num[0]);
            var deepCheck = bool.Parse(num[1]);

            data.Add(new Data(time, deepCheck));
        }

        data.Sort((a, b) => a.time.CompareTo(b.time));
        var medianTime = data[data.Count / 2];
        var shortestTime = data[0];
        var longestTime = data[data.Count - 1];
        var sLongestTime = data[data.Count - 2];
        var tLongestTime = data[data.Count - 3];
        var fLongestTime = data[data.Count - 4];

        var avgTime = 0L;

        foreach (var time in data)
            avgTime += time.time;

        avgTime /= data.Count;

        UnityEngine.Debug.Log("Median Time: " + medianTime.time + ", Deep Check: " + medianTime.deepCheck);
        UnityEngine.Debug.Log("Shortest Time: " + shortestTime.time + ", Deep Check: " + shortestTime.deepCheck);
        UnityEngine.Debug.Log("Longest Time: " + longestTime.time + ", Deep Check: " + longestTime.deepCheck);
        UnityEngine.Debug.Log("2nd Longest Time: " + sLongestTime.time + ", Deep Check: " + sLongestTime.deepCheck);
        UnityEngine.Debug.Log("3rd Longest Time: " + tLongestTime.time + ", Deep Check: " + tLongestTime.deepCheck);
        UnityEngine.Debug.Log("4th Longest Time: " + fLongestTime.time + ", Deep Check: " + fLongestTime.deepCheck);
        UnityEngine.Debug.Log("Average Time: " + avgTime);

        reader.Close();
    }

    public static void WriteLine(long time, bool deepCheck)
    {
        writer.WriteLine(time + "," + deepCheck);
    }
}

public struct Data
{
    public long time;
    public bool deepCheck;

    public Data(long time, bool deepCheck)
    {
        this.time = time;
        this.deepCheck = deepCheck;
    }
}