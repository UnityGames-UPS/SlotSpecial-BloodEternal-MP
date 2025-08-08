using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{

    internal static List<string> FlattenSymbolsToEmit(List<List<string>> symbolsToEmit)
    {
        List<string> flattenedList = new List<string>();

        // Flatten the list
        foreach (var innerList in symbolsToEmit)
        {
            flattenedList.AddRange(innerList);
        }

        return flattenedList;
    }

    internal static List<List<int>> ConvertStringListsToInt(List<List<string>> input)
    {
        List<List<int>> output = new List<List<int>>();

        foreach (List<string> subList in input)
        {
            List<int> intSubList = new List<int>();
            foreach (string str in subList)
            {
                if (int.TryParse(str, out int value))
                {
                    intSubList.Add(value);
                }
                else
                {
                    // Handle invalid string (optional: skip, add 0, throw error, etc.)
                    Debug.LogWarning($"Invalid number: {str}");
                    intSubList.Add(0); // or choose to skip or handle differently
                }
            }
            output.Add(intSubList);
        }

        return output;
    }

    internal static List<List<int>> GetSymbolToEmit(Payload payload, GameData gameData)
    {
        List<List<int>> symbols = new List<List<int>>();

        for (int i = 0; i < payload.lineWins.Count; i++)
        {
            int lineIndex = payload.lineWins[i].lineIndex;
            List<int> positions = payload.lineWins[i].positions;

            for (int j = 0; j < positions.Count; j++)
            {
                int x = positions[j];
                int y = gameData.lines[lineIndex][positions[j]];

                // Create a new list for each coordinate pair
                symbols.Add(new List<int> { x, y });
            }
        }


        return symbols;
    }
    internal static List<string> getAllVal(Substitutions substitutions)
    {
        List<string> res = new List<string>();
        res.Clear();
        foreach (var obj in substitutions.bloodSplash)
        {
            res.Add(obj.index);
        }
        return res;
    }
    internal static List<string> GetSingleString(List<List<string>> data)
    {
        List<string> res = new List<string>();

        foreach (var innerList in data)
        {

            res.Add(innerList[0]);
            res.Add(innerList[1]);

        }
        // Testx(res);
        return res;
    }
    internal static void Testx(List<string> VHPos)
    {
        for (int i = 0; i < VHPos.Count; i++)
        {
            Debug.Log($"Testx [{i}]: {VHPos[i]}");
        }
    }
    internal static List<List<string>> GetListOfSymbolToEmit(Payload payload, GameData gameData)
    {
        List<List<string>> symbols = new List<List<string>>();
        for (int i = 0; i < payload.lineWins.Count; i++)
        {
            int lineIndex = payload.lineWins[i].lineIndex;
            List<int> positions = payload.lineWins[i].positions;
            List<string> dummy = new List<string>();

            for (int j = 0; j < positions.Count; j++)
            {
                string x = positions[j].ToString();
                string y = gameData.lines[lineIndex][positions[j]].ToString();
                string z = x + "," + y;
                // Create a new list for each coordinate pair
                dummy.Add(z);

            }
            symbols.Add(dummy);
            // Testx(symbols[i]);
        }

        return symbols;
    }
}
