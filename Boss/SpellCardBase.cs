using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for spell cards. These just need a timeout, as the boss will handle the health.
/// </summary>
public abstract class SpellCardBase : MonoBehaviour
{
    public float TimeLimit;
    public float DefenseMod = 1.0f;
    public string Name;

    [Range(0, 1)]
    public float SpellCooldownRatio;

    [HideInInspector]
    public Boss User;

    protected float time;
    protected bool onSpell = false;
    protected bool forceFollowParent = false;

    [HideInInspector]
    public bool cardAlive = false;

    protected Coroutine currentCoroutine;

    protected Transform ResetMovementOverride = null;

    private void Start() {
        time = TimeLimit;
        cardAlive = true;
        currentCoroutine = StartCoroutine(StartCooldown());
    }

    private void Update() {
        if (time < 0) {
            time = 0;
            if (onSpell) {
                User.Die();
            } else {
                onSpell = true;
                StartCoroutine(StartSpell());
            }
        } else {
            time -= Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        if(forceFollowParent) {
            transform.position = User.transform.position;
        }

        CardFixedUpdate();
    }

    public void Interrupt() {
        StageController.Stage.KillAllEnemyBullets();

        if(currentCoroutine != null) StopCoroutine(currentCoroutine);

        time = TimeLimit;

        User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);

        OnDefeat(onSpell);

        if(!onSpell) {
            onSpell = true;
            StageController.AudioManager.PlaySFX("spellstart", 0.6f);
            currentCoroutine = StartCoroutine(StartSpell());
        } else {
            StageController.AudioManager.PlaySFX("enemydeath", 0.6f);
        }
    }

    public float GetTime() {
        return time;
    }
    
    public Transform GetResetOverride() {
        return ResetMovementOverride;
    }

    #region Sound Effect Methods
    public void ShootSFX(float volume = 0.2f) {
        StageController.AudioManager.PlaySFX("shootbullet", volume);
    }
    #endregion

    #region Bullet Util Methods
    // Bullet utility methods
    public IEnumerator Spiral(string tag, int branches, int shotsPerBranch, float speed = 10f, float delay = 0.1f) {
        float shotAngle = shotsPerBranch;
        float branchAngle = 360f / branches;

        while (true) {
            for (int i = 0; i < branches; i++) {
                for (int j = 0; j < shotsPerBranch; j++) {
                    GameObject bullet = ObjectPooler.Spawn(tag, transform.position, transform.rotation);
                    bullet.transform.Rotate(0, 0, -((-(shotsPerBranch - 1) * (shotAngle / 2f)) + j * shotAngle) + branchAngle * i);
                    bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * -speed;
                }
            }

            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator Ring(string tag, int numPoints, float speed, int countPerRing = 1, bool loop = false, float ringDelay = 0, float loopDelay = 0.1f, Vector2? pos = null, float r = 0) {
        if (pos == null) pos = transform.position;
        if (loopDelay <= 0) loopDelay = 0.1f;  // avoid infinite loops

        do {
            for (int i = 0; i < numPoints; i++) {
                for(int j = 0; j < countPerRing; j++) {
                    GameObject g = ObjectPooler.Spawn(tag, new Vector3(pos.Value.x, pos.Value.y, 0), Quaternion.identity);
                    g.transform.Rotate(new Vector3(0, 0, (360 / numPoints * j) - 90));
                    g.GetComponent<Rigidbody2D>().velocity = g.transform.up * speed;

                    if (ringDelay > 0) yield return new WaitForSeconds(ringDelay);
                }
            }

            yield return new WaitForSeconds(loopDelay);
        } while (loop) ;

        yield return null;
    }

    public IEnumerator CircleBullet(string tag, int numInCircle, float velocity, int ringCount = 1, float ringDelay = 1, Vector2? pos = null, Vector2? acceleration = null, float minV = 0, float maxV = 1000000, float rotation = 0.0f) {
        Vector3 realPos;

        if(pos == null) {
            realPos = transform.position;
        } else {
            realPos = new Vector3(pos.Value.x, pos.Value.y, 0);
        }

        for (int _ = 0; _ < ringCount; _++) {
            for (int i = 0; i < numInCircle; i++) {
                GameObject bullet = ObjectPooler.Spawn(tag, realPos, Quaternion.identity);
                bullet.transform.Rotate(new Vector3(0, 0, (360 / numInCircle * i) - 90 + rotation));
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * velocity;
            }

            yield return new WaitForSeconds(ringDelay);
        }
    }

    public static IEnumerator CircleBulletStatic(string tag, int numInCircle, float velocity, Vector2 pos, int ringCount = 1, float ringDelay = 1, Vector2? acceleration = null, float minV = 0, float maxV = 1000000, float rotation = 0) {
        for (int _ = 0; _ < ringCount; _++) {
            for (int i = 0; i < numInCircle; i++) {
                GameObject bullet = ObjectPooler.Spawn(tag, pos, Quaternion.identity);
                bullet.transform.Rotate(new Vector3(0, 0, (360 / numInCircle * i) - 90 + rotation));
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * velocity;
            }

            yield return new WaitForSeconds(ringDelay);
        }
    }

    public void Shotgun(string bulletTag, int bulletCount, float shotAngle, float velocity, float rotation, Vector2? scale = null, bool randVeclocity = false, float minv = 0, float maxv = 1) {
        for (int i = 0; i < bulletCount; i++) {
            GameObject bullet = ObjectPooler.Spawn(bulletTag, transform.position, Quaternion.identity);

            float theta = i * (shotAngle / (bulletCount-1)) - shotAngle / 2f;

            bullet.transform.Rotate(new Vector3(0, 0, theta + rotation));
            
            if(scale.HasValue) {
                bullet.transform.localScale = new Vector3(scale.Value.x, scale.Value.y, bullet.transform.localScale.z);
            }

            if (randVeclocity) {
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * Random.Range(minv, maxv);
            } else {
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * velocity;
            }
        }
    }

    public void BulletStar(string bulletTag, Vector3 position, int starPoints, int starDensity = 4, float velocity = 8, float acceleration = 0, Vector3? scale = null, float rotation = 0f) {
        int numInCircle = starDensity * starPoints;
        int layer = 2;

        for (int i = 0; i < numInCircle; i++) {
            float v = Mathf.Abs(i % starDensity - starDensity / 2f) * 3f + velocity;

            GameObject bullet = ObjectPooler.Spawn(bulletTag, position, Quaternion.identity);

            if (scale != null) bullet.transform.localScale = scale.Value;
            bullet.transform.Rotate(new Vector3(0, 0, (360f / numInCircle * i) - 90 + rotation));
            bullet.GetComponent<SpriteRenderer>().sortingOrder = layer++;
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * v;
        }
    }
    #endregion

    public virtual void OnDefeat(bool isSpell) {
        List<EnumItemType> dpool = new List<EnumItemType>();

        if(StageController.Player.GetComponent<PlayerController>().GetPercentMaxPower() != 1.0f) {
            for(int i = 0; i < 8; i++) {
                dpool.Add(EnumItemType.POWER);
            }

            dpool.Add(EnumItemType.BIGPOWER);
        } else {
            for (int i = 0; i < 14; i++) {
                dpool.Add(EnumItemType.POINT);
            }

            if(Random.Range(0, 6) == 1) {
                dpool.Add(EnumItemType.BOMB);
            }
        }

        foreach(EnumItemType item in dpool) {
            StageController.DropItem(item, transform.position, true);
        }
    }

    public virtual void CardFixedUpdate() {
        // NO-OP
    }

    // ABSTRACT METHODS
    public abstract IEnumerator StartCooldown();

    public abstract IEnumerator StartSpell();
}
