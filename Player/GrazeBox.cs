using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeBox : MonoBehaviour
{
    public GameObject Player;
    private PlayerController Pcont;

    private void Start() {
        Pcont = Player.GetComponent<PlayerController>();
    }

    private void FixedUpdate() {
        gameObject.transform.position = Player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "EnemyBullet") {
            StageController.AudioManager.PlaySFX("graze", 0.3f);
            Pcont.AddPoints(100);
        }
    }
}
