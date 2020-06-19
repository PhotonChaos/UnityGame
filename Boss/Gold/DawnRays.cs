    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For nonspell:
/// TBA
/// 
/// For spell:
/// Shoots out 3 circles of pastel yellow bullets, then three random bursts of pastel blue
/// Wait 3 seconds, then repeat
/// </summary>
public class DawnRays : SpellCardBase
{
    private char state = 'c'; // the position of the boss, l is TOP_LEFT, c is TOP_CENTER, r is TOP_RIGHT

    public override IEnumerator StartCooldown() {
        while(true) {
            int attack = Random.Range(1, 4);

            switch (attack) {
                case 1:
                    
                    StartCoroutine(CircleBullet("PastelYellow", 17, 9.5f, 3, 0.125f));

                    for(int i = 0; i < 3; i++) {
                        ShootSFX();
                        yield return new WaitForSeconds(0.125f);
                    }
                    break;

                case 2:
                    // targeted streams of blue
                    for(int i = 0; i < 7; i++) {
                        GameObject bullet = ObjectPooler.Spawn("PastelBlue", transform.position, Quaternion.identity);
                        GameObject bullet2 = ObjectPooler.Spawn("PastelBlue", transform.position, Quaternion.identity);

                        Vector3 dir = StageController.Player.transform.position - transform.position;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90 + 6, Vector3.forward);
                        bullet2.transform.rotation = Quaternion.AngleAxis(angle - 90 - 6, Vector3.forward);

                        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * 9.5f;
                        bullet2.GetComponent<Rigidbody2D>().velocity = bullet2.transform.up * 9.5f;

                        if(i % 2 == 0) ShootSFX();

                        yield return new WaitForSeconds(0.142f);
                    }
                    break;

                case 3:
                    // Random burst of pink
                    ShootSFX();
                    for (int i = 0; i < 18; i++) {
                        GameObject bullet = ObjectPooler.Spawn("PastelRed", transform.position, Quaternion.identity);

                        Vector2 velocity = Random.insideUnitCircle * 9;

                        if (velocity.sqrMagnitude < 16) velocity = velocity.normalized * 6;

                        bullet.GetComponent<Rigidbody2D>().velocity = velocity;
                    }
                    break;
            }

            // TODO: Make boss move to random waypoint
            if(Random.Range(0, 2) == 0) {
                // left
                if(state == 'c') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_LEFT);
                    state = 'l';
                } else if(state == 'r') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                    state = 'c';
                }
            } else {
                // right
                if (state == 'c') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_RIGHT);
                } else if(state == 'l') {
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
                }
            }

            yield return new WaitForSeconds(3.0f);
        }
    }

    public override IEnumerator StartSpell() {
        User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER);
        StartCoroutine(RainBulletsCoroutine());

        while(true) {
            StartCoroutine(CircleBullet("PastelYellow", 17, 9.5f, 3, 0.125f));
            for (int i = 0; i < 3; i++) {
                ShootSFX();
                yield return new WaitForSeconds(0.125f);
            }
            yield return new WaitForSeconds(1.0f);

            for(int i = 0; i < 7; i++) {
                GameObject bullet = ObjectPooler.Spawn("PastelBlue", transform.position, Quaternion.identity);
                GameObject bullet2 = ObjectPooler.Spawn("PastelBlue", transform.position, Quaternion.identity);
                GameObject bullet3 = ObjectPooler.Spawn("PastelBlue", transform.position, Quaternion.identity);

                Vector3 dir = StageController.Player.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                bullet2.transform.rotation = Quaternion.AngleAxis(angle - 90 - 10, Vector3.forward);
                bullet3.transform.rotation = Quaternion.AngleAxis(angle - 90 + 10, Vector3.forward);

                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * 9.5f;
                bullet2.GetComponent<Rigidbody2D>().velocity = bullet2.transform.up * 9.5f;
                bullet3.GetComponent<Rigidbody2D>().velocity = bullet3.transform.up * 9.5f;

                ShootSFX();

                yield return new WaitForSeconds(0.142f);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator RainBulletsCoroutine() {
        while(cardAlive) {
            RainBullets();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RainBullets() {
        for(int x = -13; x <= 13; x++) {
            if (x % 3 != 0 || x > -4 && x < 4) continue;

            GameObject bullet = ObjectPooler.Spawn("PastelRed", new Vector3(x + Random.Range(-0.5f, 0.5f), 24, 0), Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().gravityScale = 0.25f;
        }
    }
}
