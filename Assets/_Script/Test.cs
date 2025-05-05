using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class Test 
{
    private static string savePath = Application.persistentDataPath + "/test.csv";
    public static List<int> killCounts = new List<int>();
    public static List<int> amountSkillsBought = new List<int>();
    public static List<int> amountWaves = new List<int>();
    public static List<string> death = new List<string>();
    public static List<int> replays = new List<int>();
    public static List<int> reset = new List<int>();
    public static List<int> amountDash = new List<int>();
    public static List<string> whatSkillsBought = new List<string>();


    public static void SaveToCSV()
    {
        using (StreamWriter writer = new StreamWriter(savePath, false))
        {
            writer.WriteLine("Replays," + string.Join(",", replays));
            writer.WriteLine("KillCounts," + string.Join(",", killCounts));
            writer.WriteLine("amountSkillsBought," + string.Join(",", amountSkillsBought));
            writer.WriteLine("AmountWaves," + string.Join(",", amountWaves));
            writer.WriteLine("Death," + string.Join(",", death));
            writer.WriteLine("Reset," + string.Join(",", reset));
            writer.WriteLine("AmountDash," + string.Join(",", amountDash));
            writer.WriteLine("WhatClassBought," + string.Join(",", whatSkillsBought));
        }
    }
}
