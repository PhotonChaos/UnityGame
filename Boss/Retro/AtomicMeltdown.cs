using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicMeltdown : SpellCardBase
{
    public AudioClip BigShot;
    public AudioClip BigShotEcho;
    public AudioClip Boon;
    public AudioClip Alert;
    public AudioClip ChargeUp;

    private List<GameObject> Bullets = new List<GameObject>();
    private int bulletSpawns = 0;

    public override IEnumerator StartCooldown() {
        while(true) {
            while (User.IsMoving()) yield return new WaitForSeconds(0.001f);

            StageController.AudioManager.PlaySFX(ChargeUp);
            yield return new WaitForSeconds(2.0f);

            for (int i = 0; i < 15; i++) {
                if(i % 2 == 0) ShootSFX();
                StartCoroutine(CircleBullet("BubbleCyan", 18, 26));
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
        }
    }

    public override IEnumerator StartSpell() {
        while (User.IsMoving()) yield return new WaitForSeconds(0.0001f);

        StageController.AudioManager.PlaySFX(Alert, 0.9f);

        const int MAX_LVL = 5;
        const float BORDER_SPEED = 20;
        List<Vector2> bulletLocations = new List<Vector2>();

        for(int level = 0; level < MAX_LVL; level++) {
            for (int x = -14; x <= 14; x++) {
                for (int y = -3; y <= 23; y++) {
                    if ((x % 2 == 0 && y % 2 == 0) || (Mathf.Abs(x % 2) == 1 && Mathf.Abs(y % 2) == 1)) continue;

                    bulletLocations.Add(new Vector2(x, y));
                }
            }

            int c = 0;

            StageController.AudioManager.PlaySFX(BigShot, 0.6f);
            StageController.AudioManager.PlaySFX(BigShotEcho, 0.6f);

            foreach(Vector2 pos in bulletLocations) {
                if (Random.Range(0, 9) < 5) {
                    if (c++ % 35 == 0) ShootSFX();
                    StartCoroutine(WarnSpawn(pos, level));
                    yield return new WaitForSeconds(0.0005f);
                }
            }

            bulletLocations.Clear();

            while (bulletSpawns > 0) {
                yield return new WaitForSeconds(0.0001f);
            }

            for(int i = 0; i < 20; i++) {
                if (i % 2 == 0) ShootSFX();

                GameObject bullet  = ObjectPooler.Spawn("BubbleGreen", new Vector3(-13.5f, -1.5f, 0), Quaternion.identity);
                GameObject bullet2 = ObjectPooler.Spawn("BubbleGreen", new Vector3(-13.5f, -1.5f, 0), Quaternion.identity);
                GameObject bullet3 = ObjectPooler.Spawn("BubbleGreen", new Vector3(12.5f, 21.5f, 0), Quaternion.identity);
                GameObject bullet4 = ObjectPooler.Spawn("BubbleGreen", new Vector3(12.5f, 21.5f, 0), Quaternion.identity);

                bullet.transform.localScale  = new Vector3(3.5f, 3.5f, 1);
                bullet2.transform.localScale = new Vector3(3.5f, 3.5f, 1);
                bullet3.transform.localScale = new Vector3(3.5f, 3.5f, 1);
                bullet4.transform.localScale = new Vector3(3.5f, 3.5f, 1);

                bullet.GetComponent<Rigidbody2D>().velocity  = new Vector2(0, BORDER_SPEED);
                bullet2.GetComponent<Rigidbody2D>().velocity = new Vector2(BORDER_SPEED, 0);
                bullet3.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -BORDER_SPEED);
                bullet4.GetComponent<Rigidbody2D>().velocity = new Vector2(-BORDER_SPEED, 0);

                bullet.GetComponent<Rigidbody2D>().angularVelocity  = Random.Range(90.0f, 110.0f);
                bullet2.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(90.0f, 110.0f);
                bullet3.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(90.0f, 110.0f);
                bullet4.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(90.0f, 110.0f);

                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(1.0f);

            StageController.AudioManager.PlaySFX(Boon, 0.6f);

            foreach(GameObject bullet in Bullets) {
                Vector2 dir = (StageController.Player.transform.position - bullet.transform.position).normalized * 1;
                SimpleMovement sm = bullet.GetComponent<SimpleMovement>();

                sm.UpdateAcceleration(dir.x, dir.y);
            }

            yield return new WaitForSeconds(10.0f - level);
        }
    }

    private IEnumerator WarnSpawn(Vector2 pos, int level) {
        bulletSpawns++;
        level++;
        pos = new Vector2(pos.x + Random.Range(-level*0.05f, level * 0.05f), pos.y + Random.Range(-level * 0.05f, level * 0.05f));

        ObjectPooler.Spawn("WarningRed", pos, Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
        GameObject b = ObjectPooler.Spawn("EnergyBallCyan", pos, Quaternion.identity);
        b.GetComponent<SimpleMovement>().UpdateAcceleration(0, 0);
        Bullets.Add(b);
        bulletSpawns--;
    }
}
