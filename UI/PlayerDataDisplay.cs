using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataDisplay : MonoBehaviour
{
    public Text HealthText;
    public PlayerController Player;
    public TextMeshProUGUI Power;

    private void Start() {
        Player = StageController.Player.GetComponent<PlayerController>();
    }

    private void Update() {
        if(Player != null) {
            HealthText.text = $"High Score: {PlayerController.HighScore}\nScore: {Player.Score}\nPlayer: {Player.GetHealth()}\nBomb: {Player.GetBombs()}\nPower: {Player.GetPowerDisplay()}";
        }
    }
}
