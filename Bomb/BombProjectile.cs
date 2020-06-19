using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public float Power;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyBullet")) {
            ObjectPooler.Disable(other.gameObject);
            StageController.Player.GetComponent<PlayerController>().AddPoints(100);
        }
    }
}
