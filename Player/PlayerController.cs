using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static float Speed = 14.0f;
    public static int MaxPower = 100;

    public PlayerName Name;

    public int StartingPower = 50;

    public Transform ShotSpawn;

    public GameObject DeathExplosion;

    // Other Classes
    public PlayerBounds Bounds;
    public ShotTemplates Shots;
    public GameObject BombA;
    public GameObject BombB;

    public GameObject HitboxMarker;

    public GameObject CarbonLeft;
    public GameObject CarbonRight;
    public GameObject ArcyLeft;
    public GameObject ArcyRight;

    private int Power;
    private int BombCount;

    private bool Shooting = false;
    private float ShotCooldown = 0f;

    private float ShotAngle = 10f;
    private float ShotSpace = 1f;

    public long Score = 0;
    public static long HighScore = 0;
    public long ScoreLives = 0;

    public bool BeatGame;
    public bool BeatExtra;

    public readonly long[] ExtraLifeScores = new long[] { 1000000, 5000000, 8000000, 10000000, 20000000, 30000000, 40000000 };

    private int Lives;
    private float InvTime;

    private void Awake() {
        StageController.Player = gameObject;
    }

    void Start() {
        Power = StartingPower;
        Lives = 3;
        BombCount = 5000;

        if(Name == PlayerName.ARCY) {
            ArcyLeft.SetActive(true);
            ArcyRight.SetActive(true);

            Speed = 22f;
        } else {
            CarbonLeft.SetActive(true);
            CarbonRight.SetActive(true);

            Speed = 15f;

            GetComponent<CircleCollider2D>().radius = 0.2f;  // Carbon has smaller hitbox
        }
    }

    void Update() {
        if(InvTime > 0) {
            InvTime -= Time.deltaTime;
        } else if(InvTime < 0) {
            InvTime = 0;
        }

        if(ShotCooldown > 0f) {
            ShotCooldown = Math.Max(0, ShotCooldown - Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.B)) {
            StageController.SpawnBoss();
        }

        if (Input.GetKeyDown(KeyCode.Q)) Power += 10;

        if (Input.GetKeyDown(KeyCode.E)) Power -= 10;

        if (Input.GetKeyDown(KeyCode.X)) UseBomb();

        if (Input.GetKeyDown(KeyCode.G)) SaveSystem.SaveData();

        // Shooting
        if (Input.GetKeyDown(KeyCode.Z)) Shooting = true;
        if (Input.GetKeyUp(KeyCode.Z)) Shooting = false;

        SideShot.Shooting = Shooting;

        if (Shooting) {
            HandleShoot();
        }
    }

    void FixedUpdate() {
        SideShot.Power = GetLvl();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 clPos = new Vector3(
            Mathf.Clamp(rb.position.x, Bounds.xMin, Bounds.xMax),  
            Mathf.Clamp(rb.position.y, Bounds.yMin, Bounds.yMax));

        // Movement
        float mod = 1.0f;

        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) movement.y += 1.0f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) movement.y -= 1.0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) movement.x -= 1.0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) movement.x += 1.0f;

        if (Input.GetKey(KeyCode.LeftShift)) {
            mod = 0.4f;  // for sneak
            HitboxMarker.SetActive(true);
        } else {
            HitboxMarker.SetActive(false);
        }

        // make sure that the player doesn't push the bounds
        if (clPos.x == Bounds.xMax) movement.x = Mathf.Min(movement.x, 0);
        if (clPos.x == Bounds.xMin) movement.x = Mathf.Max(movement.x, 0);
        if (clPos.y == Bounds.yMin) movement.y = Mathf.Max(movement.y, 0);
        if (clPos.y == Bounds.yMax) movement.y = Mathf.Min(movement.y, 0);

        rb.velocity = movement.normalized * Speed * mod;

        // Stay within bounds
        rb.position = clPos;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("EnemyBullet") || collision.gameObject.CompareTag("EnemyBulletIndestructible")) {
            Die();
        }
    }

    private void UseBomb() {
        if(BombCount > 0) {
            BombCount--;

            // SFX
            bool arcy = Name != PlayerName.CARBON;

            GameObject bomb = arcy ? BombA : BombB;
            GameObject b = Instantiate(bomb, transform.position, transform.rotation);

            StartCoroutine(BombSFXCoroutine(arcy));

            InvTime += 10.0f;
        }
    }

    private IEnumerator BombSFXCoroutine(bool arcy) {
        StageController.AudioManager.PlaySFX("spellstart", 0.6f);
        
        if(!arcy) {
            yield return new WaitForSeconds(0.5f);
            StageController.AudioManager.PlaySFX("starbomb", 0.6f);
        }
    }

    public void SetPlayer(PlayerName name) {
        Name = name;

        if(Name == PlayerName.CARBON) {
            // Narrow Spread
            ShotAngle = 5f;
        }
    }

    public void AddPoints(long p) {
        Score += p;

        // High Score
        if (Score > HighScore) HighScore = Score;

        // Extra Lives
        if(ScoreLives < ExtraLifeScores.Length && Score >= ExtraLifeScores[ScoreLives]) {
            Lives++;
            ScoreLives++;
        }
    }

    public void AddPower(int p) {
        int lvl = GetLvl();
        Power += p;
        if (Power > MaxPower) Power = MaxPower;
        if (GetLvl() > lvl) StageController.AudioManager.PlaySFX("powerup", 0.5f);
    }

    public void SetPower(int p) {
        Power = p;
        if (Power > MaxPower) Power = MaxPower;
    }

    public int GetPower() {
        return Power;
    }

    public float GetPercentMaxPower() {
        if (Power > MaxPower) Power = MaxPower;
        return Power / (float)MaxPower;
    }

    public string GetPowerDisplay() {
        if(Power >= MaxPower) {
            Power = MaxPower;
            return "MAX";
        } else {
            return $"{Power}";
        }
    }

    public void Die() {
        if (InvTime > 0) return;

        InvTime = 2.0f;  // two seconds of invicibility

        Lives--;

        // play explosion
        GameObject expl = Instantiate(DeathExplosion, transform.position, transform.rotation);
        expl.GetComponent<ParticleSystem>().Play(); // explode
        Destroy(expl, expl.GetComponent<ParticleSystem>().main.duration);

        // TODO: Play sound effect

        if (Lives <= 0) {
            print("you died.");
        }
    }

    public long GetPoints() {
        return Score;
    }

    private void HandleShoot() {
        if (ShotCooldown == 0) {
            ShotCooldown = 0.05f; // cooldown of 0.1 seconds for the shot.

            GameObject target;

            switch (Name) {
                case PlayerName.ARCY:
                    target = Shots.ArcyMain;
                    break;

                case PlayerName.CARBON:
                    target = Shots.CarbonMain;
                    break;

                default:
                    target = Shots.CarbonMain;
                    break;
            }

            int lvl = GetLvl();

            float playerMod = Name == PlayerName.ARCY ? 1.0f : 0.5f;
            float shiftMod = Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1.0f;

            float totalMod = shiftMod * playerMod;

            int shots;

            switch(lvl) {
                case 1:
                case 2:
                    shots = 1;
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                    shots = lvl-1;
                    break;

                default:
                    shots = 1;
                    break;
            }

            for(int i = 0; i < shots; i++) {
                GameObject bullet = Instantiate(target, ShotSpawn.position - new Vector3((shots - 1) * ShotSpace/2f, 0, 0) * totalMod + new Vector3(i*ShotSpace, 1.5f, 0) * totalMod, ShotSpawn.rotation);
                bullet.transform.Rotate(new Vector3(0, 0, -((-(shots - 1) * (ShotAngle / 2f)) + i * ShotAngle)) * totalMod);
                bullet.transform.localScale = Vector3.one * 2f;

                bullet.transform.SetParent(ShotSpawn.transform);
            }
        }
    }

    private int GetLvl() {
        if(Power >= 100) {
            return 6;
        } else if (Power >= 75) {
            return 5;
        } else if (Power >= 50) {
            return 4;
        } else if (Power >= 25) {
            return 3;
        } else if (Power >= 10) {
            return 2;
        } else {
            return 1;
        }
    }

    public int GetHealth() {
        return Lives;
    }

    public void AddHealth(int h = 1) {
        StageController.AudioManager.PlaySFX("extralife");
        Lives += h;
    }

    public void SetHealth(int h) {
        Lives = h;

        if (Lives < 0) Die();
    }

    public int GetBombs() {
        return BombCount;
    }
    
    public void AddBombs(int b = 1) {
        BombCount += b;
        if (BombCount < 0) BombCount = 0;
        StageController.AudioManager.PlaySFX("getbomb");
    }

    public void SetBombs(int b) {
        BombCount = b;
        if (BombCount < 0) BombCount = 0;
    }
}

public enum PlayerName
{
    CARBON,
    ARCY
}

[Serializable]
public class ShotTemplates
{
    public GameObject CarbonMain;
    public GameObject ArcyMain;
}

[Serializable]
public class PlayerBounds
{
    public float xMin, xMax, yMin, yMax;
}
