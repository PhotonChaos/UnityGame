using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public static bool BeatGame = false;
    public static bool BeatGameExtra = false;

    public long HighScore;
    
    public bool _BeatGame;
    public bool _BeatExtra;

    public PlayerData() {
        HighScore = PlayerController.HighScore;
        _BeatGame = BeatGame;
        _BeatExtra = BeatGameExtra;
    }
}
 