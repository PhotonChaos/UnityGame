using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
    // Player High Score
    private static string HSPath = Application.persistentDataPath + "/player.savefile";
    
    public static void SaveData() {
        Debug.Log("Saving Data...");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(HSPath, FileMode.Create);
        PlayerData score = new PlayerData();

        bf.Serialize(stream, score);
        stream.Close();
    }

    public static PlayerData LoadData(int n = 0) {
        Debug.Log("Loading data...");

        if(File.Exists(HSPath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(HSPath, FileMode.Open);

            PlayerData data = bf.Deserialize(stream) as PlayerData;
            stream.Close();

            PlayerData.BeatGame = data._BeatGame;
            PlayerData.BeatGameExtra = data._BeatExtra;
            PlayerController.HighScore = data.HighScore;

            Debug.Log("Data loaded.");

            return data;
        } else {
            Debug.LogError($"Player Save Data not found in path {HSPath}");
            SaveData();

            if (n > 2) {
                Debug.LogError("UH OH STINKY");
                return null;  // don't infinitely screw things up
            }

            return LoadData();
        }
    }
}

