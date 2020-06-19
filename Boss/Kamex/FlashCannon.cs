using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashCannon : SpellCardBase
{
    public GameObject FlashLaser;

    private Rigidbody2D rb;
    private bool? lastState = null;
    private GameObject laser;  // the instance
    private FCLaserRespawn laserController;
    private float spd = 8f;

    public override IEnumerator StartCooldown() {
        yield return null;
        
        const int ringc = 25;
        const int numRings = 5;
        const float r = 4;
        const float moveSpd = 2;

        string[] tags = { "StarWhite", "StarBlue" };

        List<List<GameObject>> rings = new List<List<GameObject>>(numRings);

        while(true) {
            while(User.moving) {
                yield return new WaitForSeconds(0.0001f);
            }

            for(int i = 0; i < numRings; i++) {
                rings.Add(new List<GameObject>(ringc));
            }

            for (int i = 0; i < ringc; i++) { // int i = 0; i < ringc; i++
                if(i % 3 == 0) ShootSFX();
                
                for (int j = 0; j < numRings; j++) {
                    GameObject ball = ObjectPooler.Spawn(tags[j%tags.Length], transform.position, Quaternion.identity);

                    ball.transform.Rotate(0, 0, 180f / ringc * i);
                    ball.transform.Translate(ball.transform.up * r);

                    rings[j].Add(ball);

                }
                yield return new WaitForSeconds(0.7f / ringc);
            }

            foreach(List<GameObject> ring in rings) {
                ShootSFX();

                foreach(GameObject bullet in ring) {
                    if (bullet == null) continue;
                    bullet.GetComponent<Rigidbody2D>().velocity = (StageController.Player.transform.position - bullet.transform.position).normalized * Random.Range(10f, 19f);
                }

                yield return new WaitForSeconds(0.3f);
            }
            
            // Go to random waypoint
            switch (Random.Range(0, 6)) {
                case 0:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_LEFT, moveSpd);
                    break;

                case 1:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_CENTER, moveSpd);
                    break;

                case 2:
                    User.GoToWaypoint(StageController.Stage.Waypoints.TOP_RIGHT, moveSpd);
                    break;
            }
            
            rings.Clear();
            yield return new WaitForSeconds(1f);
        } 
    }

    public override IEnumerator StartSpell() {
        ResetMovementOverride = StageController.Stage.Waypoints.MID_CENTER;
        yield return new WaitForSeconds(0.5f);

        while (User.IsMoving()) {
            yield return new WaitForSeconds(0.0001f);
        }

        User.GoToWaypoint(StageController.Stage.Waypoints.MID_CENTER);

        yield return new WaitForSeconds(0.1f);

        while (User.IsMoving()) {
            yield return new WaitForSeconds(0.0001f);
        }

        User.SetDefenseMod(DefenseMod);

        // LASER HERE
        rb = GetComponent<Rigidbody2D>();

        laser = Instantiate(FlashLaser, Vector3.zero, Quaternion.identity);
        laserController = laser.GetComponent<FCLaserRespawn>();
        laserController.fc = this;
        laser.transform.SetParent(transform);
        laser.transform.localPosition = new Vector3(0, -22.38f, 0);

        StartCoroutine(SteelBalls());
        StartCoroutine(Laser());

        while(true) {
            bool onLeft = IsTargetOnLeft(StageController.Player);

            if ((lastState == null || onLeft != lastState.Value) && (time / TimeLimit < 0.95f)) {
                // if the direction changed, change the rotation velocity
                rb.angularVelocity = 0;

                if (onLeft) {
                    rb.AddTorque(-spd);
                } else {
                    rb.AddTorque(spd);
                }

                lastState = onLeft;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator Laser() {
        bool laserState = false; // True if on, false if off
        while(true) {
            if(laserState) {
                spd = 90f;
                laserController.Shrink();
                yield return new WaitForSeconds(4.0f);
            } else {
                spd = 15f;
                laserController.Grow();
                yield return new WaitForSeconds(10.0f);
            }

            laserState = !laserState;
        }
    }

    public IEnumerator SteelBalls() {
        const int ringc = 8;

        while(true) {
            float randRot = 360 * Mathf.Sin(Time.time * 10);

            ShootSFX();

            for(int i = 0; i < ringc; i++) {
                GameObject ball = ObjectPooler.Spawn("StarWhite", transform.position, Quaternion.identity);
                ball.transform.localScale = new Vector3(2, 2, 1);
                ball.transform.Rotate(0, 0, 360 / ringc * i + randRot);

                Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
                rb.velocity = ball.transform.up * 5.0f;
                rb.angularVelocity = Random.Range(70f, 100f) * (Random.Range(0, 2) == 1 ? -1 : 1);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private bool IsTargetOnLeft(GameObject target) {
        return GetTargetDir(target) == -1f;
    }

    private float GetTargetDir(GameObject target) {
        Vector3 perp = Vector3.Cross(transform.forward, target.transform.position - transform.position);
        float dir = Vector3.Dot(perp, transform.up);

        if (dir > 0f) {
            return 1f;
        } else if (dir < 0f) {
            return -1f;
        } else {
            return 0f;
        }
    }
}
