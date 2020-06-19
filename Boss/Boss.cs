using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string BossName;

    public AudioClip BossTheme;
    public string ThemeCredit;

    public int StartingLives;
    public float StartingHealth;

    [HideInInspector]
    public int Lives;

    public float Health;

    public List<GameObject> SpellCards;

    private bool stopMovements = false;

    private GameObject Card;
    private SpellCardBase SpellCard;

    private float invTime = 0;
    private bool onSpell = false;
    [HideInInspector]
    public bool moving = false;
    private float defenseMod = 1.0f;
    private int activeSpell = 0;

    private void Start() {
        Health = StartingHealth;
        Lives = StartingLives;

        if(BossTheme != null) {
            StageController.AudioManager.SetVolume(0.3f);
            StageController.AudioManager.PlayBGM(BossTheme);
        }

        StartSpell();  // TODO: Do this after dialogue
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("PlayerBullet")) {
            Destroy(collision.gameObject);  // eliminate the bullet

            Damage(collision.gameObject.GetComponent<PlayerBullet>().GetPower()); // Damage the enemy
        }
    }

    private void Update() {
        if (invTime > 0) invTime -= Time.deltaTime;
    }

    public void Damage(float dmg, bool bomb = false) {
        if (bomb) {
            if (invTime > 0) return;
            invTime = 0.4f;
        }

        Health -= dmg * defenseMod;

        if((onSpell && Health <= 0) || (!onSpell && Health <= StartingHealth * SpellCard.SpellCooldownRatio)) {
            Die();
        }
    }

    public void Die(bool fullKill = false) {
        SpellCard.Interrupt();
        StageController.Stage.KillAllEnemyBullets();
        SetDefenseMod(1);

        if (onSpell || fullKill) {
            Lives--;
            Health = StartingHealth;
            SpellCard.cardAlive = false;
            Destroy(Card);
            onSpell = false;

            if (fullKill) Lives = 0;

            if (Lives <= 0) {
                // Explode
                StageController.AudioManager.StopBGM();
                StageController.AudioManager.PlaySFX("bossdeath", 0.2f);
                StageController.DropItem(EnumItemType.BOMB, transform.position);

                Destroy(gameObject);
                return;
            }

            // If we get here, that means the next spell should start
            activeSpell++;
            StartSpell();
            return;
        }

        onSpell = true;
        SetDefenseMod(SpellCard.DefenseMod);

        if(SpellCard.GetResetOverride() != null) {
            GoToWaypoint(SpellCard.GetResetOverride(), 2, true);
        } else {
            GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER, 2, true);   
        }
    }

    private void StartSpell() {
        defenseMod = 1.0f;
        Card = Instantiate(SpellCards[activeSpell], Vector3.zero, Quaternion.identity);
        Card.transform.SetParent(gameObject.transform);
        Card.transform.localPosition = Vector3.zero;

        SpellCard = Card.GetComponent<SpellCardBase>();
        SpellCard.User = this;
    }

    public float GetHealthPercent(bool asNum = false) {
        float p = Health / StartingHealth;

        if (asNum) return p * 100;
        else return p;
    }

    public string GetSpellTimeText() {
        return $"{Mathf.CeilToInt(SpellCard.GetTime())}";
    }

    public string GetSpellCardName() {
        if (!onSpell) return "";
        return $"{SpellCard.Name}";
    }

    public void GoToWaypoint(Vector3 waypoint, float speed = 2, bool force = false) {
        StartCoroutine(GoToWaypointCoroutine(waypoint, speed, force));
    }

    public void GoToWaypoint(Transform waypoint, float speed = 2, bool force = false) {
        StartCoroutine(GoToWaypointCoroutine(waypoint.position, speed, force));
    }

    public bool IsMoving() {
        return moving || stopMovements;
    }

    public void SetDefenseMod(float mod) {
        defenseMod = mod;
    }

    private IEnumerator GoToWaypointCoroutine(Vector3 waypoint, float speed = 2, bool force = false) {
        if (force) {
            stopMovements = true;
            moving = false;
        }

        if(!moving || force) {
            moving = true;
            Transform transf = gameObject.GetComponent<Transform>();

            while (Vector3.Distance(transf.position, waypoint) >= 0.01f && (!stopMovements || force)) {
                transf.position = Vector3.Lerp(transf.position, waypoint, Time.deltaTime * speed);
                yield return new WaitForSeconds(0.00001f);
            }

            if(!stopMovements && !force) transf.position = waypoint;

            moving = false;
        }

        if(force) {
            stopMovements = false;
        }
    }
}
