using UnityEngine;

public class Shootable : MonoBehaviour
{
    public float StartingHealth;
    public GameObject Explosion;
    public bool Bombable;

    [ReadOnly]
    public float Health;

    float d_time = 0;

    private void Start() {
        Health = StartingHealth;
    }

    public void Update() {
        if (d_time > 0) {
            d_time -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "PlayerBullet") {
            Destroy(other.gameObject);  // eliminate the bullet

            Damage(other.gameObject.GetComponent<PlayerBullet>().GetPower()); // Damage the enemy
        } else if(other.tag == "Bomb") {
            Damage(other.gameObject.GetComponent<BombProjectile>().Power);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!Bombable) return;

        if (other.gameObject.CompareTag("Bomb")) {
            if (CompareTag("EnemyBullet")) {
                // delete bullet, add points
            } else if (CompareTag("Enemy")) {
                Damage(other.GetComponent<BombProjectile>().Power, true);
            } else if (CompareTag("Boss")) {
                gameObject.GetComponent<Boss>().Damage(other.GetComponent<BombProjectile>().Power, true);
            }
        }
    }

    public void Damage(float dmg, bool bomb = false) {
        if(bomb) {
            if (d_time > 0) return;
            d_time = 0.4f;
        }

        Health -= dmg;
        if (Health <= 0) Die();
    }

    public float GetHealth() {
        return Health;
    }

    public void Die(bool playerKilled = true) {
        if (playerKilled) {
            // Player Killed Enemy
            StageController.Player.GetComponent<PlayerController>().AddPoints(100);  // give points
            GameObject expl = Instantiate(Explosion, transform.position, transform.rotation);
            expl.GetComponent<ParticleSystem>().Play(); // explode
            Destroy(expl, expl.GetComponent<ParticleSystem>().main.duration);
        }

        StageController.AudioManager.PlaySFX("enemydeath", 0.2f);

        StageController.Stage.BeatEnemy();

        Destroy(gameObject);
    }
}
