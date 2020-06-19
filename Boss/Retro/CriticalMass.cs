using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalMass : SpellCardBase
{
    public AudioClip BigShot;
    public AudioClip BigShotEcho;
    public AudioClip ChargeUp;

    private RetroBullets retrobullets = null;
    private GameObject RetroBullet = null;
    private SimpleMovement simp = null;
    private Rigidbody2D rb;

    private List<List<GameObject>> BulletSpawns = new List<List<GameObject>>();
    private List<GameObject> CurrentChain = new List<GameObject>();

    private const int MAX_CHAINS = 3;
    private const float accelerationSpeed = 14;
    private bool accelerating = false;
    private bool stopping = false;

    public override void CardFixedUpdate() {
        if (simp == null) return;

        if(RetroBullet != null) {
            RetroBullet.transform.position = transform.position;
        }

        if(accelerating) {
            if(transform.position.x < -14 || transform.position.x > 14 || transform.position.y < -2.8f || transform.position.y > 23.37f) {
                accelerating = false;
                simp.UpdateAcceleration(0, 0);
                rb.velocity = Vector2.zero;
                User.transform.position = new Vector2(Mathf.Clamp(transform.position.x, -13.99f, 13.99f), Mathf.Clamp(transform.position.y, -2.7f, 23.3f));

                StageController.AudioManager.PlaySFX(BigShot, 0.6f);
                StageController.AudioManager.PlaySFX(BigShotEcho, 0.4f);
            }
        } else if(!stopping) {
            StartCoroutine(AccelerateToPlayer());
        }
    }

    public override IEnumerator StartCooldown() {
        while(true) {
            while (User.IsMoving()) yield return new WaitForSeconds(0.0001f);

            StageController.AudioManager.PlaySFX(ChargeUp);
            yield return new WaitForSeconds(2.0f);

            Vector3 dir = StageController.Player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            for(int i = 0; i < 15; i++) {
                ShootSFX();
                Shotgun("BubbleCyan", 5, 50, 23f, angle);
                yield return new WaitForSeconds(0.05f);
            }

            switch (Random.Range(0, 6)) {
                case 0:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                    break;

                case 1:
                    User.GoToWaypoint(StageController.Stage.Waypoints.MID_LEFT);
                    break;

                case 2:
                    User.GoToWaypoint(StageController.Stage.Waypoints.MID_RIGHT);
                    break;
            }

            yield return new WaitForSeconds(0.9f);
        }
    }

    public override IEnumerator StartSpell() {
        forceFollowParent = true;

        rb = User.GetComponent<Rigidbody2D>();
        retrobullets = User.GetComponent<RetroBullets>();
        
        while(User.IsMoving()) yield return new WaitForSeconds(0.0001f);

        simp = User.gameObject.AddComponent<SimpleMovement>();

        StageController.AudioManager.PlaySFX(BigShot, 0.7f);
        StageController.AudioManager.PlaySFX(BigShotEcho, 0.7f);

        RetroBullet = Instantiate(retrobullets.RetroFireball, transform.position, Quaternion.identity);

        while(true) {
            if(accelerating) {
                GameObject bullet = ObjectPooler.Spawn("EnergyBallCyan", transform.position, Quaternion.identity);
                bullet.GetComponent<SimpleMovement>().UpdateAcceleration(0, 0);
                CurrentChain.Add(bullet);
            }

            yield return new WaitForSeconds(0.09f);
        }
    }

    private IEnumerator AccelerateToPlayer() {
        stopping = true;

        BulletSpawns.Add(CurrentChain);
        CurrentChain = new List<GameObject>();

        if (BulletSpawns.Count >= MAX_CHAINS) {
            
            foreach (GameObject obj in BulletSpawns[0]) {
                if(obj.activeSelf) {
                    Vector2 dir = (StageController.Player.transform.position + (Vector3)Random.insideUnitCircle*2 - obj.transform.position).normalized * 4.5f;
                    Vector3 pos = obj.transform.position;

                    ObjectPooler.Disable(obj);
                    GameObject newobj = ObjectPooler.Spawn("EnergyBallRetro", pos, Quaternion.identity);

                    newobj.GetComponent<SimpleMovement>().UpdateAcceleration(dir.x + Random.Range(-1f, 1f), dir.y + Random.Range(-1f, 1f));
                }
            }

            BulletSpawns[0].Clear();
            BulletSpawns.RemoveAt(0);
        }

        yield return new WaitForSeconds(0.3f);
        Vector2 diff = (StageController.Player.transform.position - gameObject.transform.position).normalized * accelerationSpeed;
        simp.UpdateAcceleration(diff.x, diff.y);
        stopping = false;
        accelerating = true;
    }

    private void OnDestroy() {
        if (simp == null) return;
        simp.UpdateAcceleration(0, 0);
        rb.velocity = Vector2.zero;
        Destroy(simp);

        if (RetroBullet == null) return;
        Destroy(RetroBullet);
    }
}
