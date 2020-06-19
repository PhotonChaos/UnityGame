using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelLoader
{
    private List<string> LevelCode;
    private int InstructionNumber = 0; // Instruction Pointer

    public LevelLoader(TextAsset asset) {
        LevelCode = new List<string>();

        LevelCode.AddRange(asset.text.Split('\n'));
    }

    public void ProcessLevel() {
        string title = NextInstruction()[1];
        string subtitle = NextInstruction()[1];

        StageController.Stage.LevelTitle = title.Replace("_", " ");
        StageController.Stage.LevelSubtitle = subtitle.Replace("_", " ");
    }

    public string[] NextInstruction(int n = 0) {
        if(n > 100) {
            Debug.LogError("TOO MANY WHITESPACE LINES AND COMMENTS!!!!");
            return null;
        }

        if (InstructionNumber >= LevelCode.Count) {
//            Debug.Log($"FLAG_A: {InstructionNumber}");
            return null;
        } else if (string.IsNullOrWhiteSpace(LevelCode[InstructionNumber]) || LevelCode[InstructionNumber].StartsWith("#")) {
//            Debug.Log($"FLAG_B: {InstructionNumber}");
            InstructionNumber++;
            return NextInstruction(n+1);
        }

        return LevelCode[InstructionNumber++].Trim().Split(' ');
    }
}
