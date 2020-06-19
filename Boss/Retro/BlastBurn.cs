using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastBurn : SpellCardBase
{
    private RetroBullets Fireballs;

    public override IEnumerator StartCooldown() {
        while(true) {
            Vector3 dir = StageController.Player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            if(Random.Range(0,2) == 0) {
                ShootSFX();
                Shotgun("StarRed", 7, 35, 14, angle, new Vector2(2, 2));
                Shotgun("BubbleOrange", 5, 25, 12, angle);
                Shotgun("CircleBallYellow", 7, 25, 10.5f, angle, null, true, 6.5f, 7);
                Shotgun("CircleBallYellow", 5, 20, 9.5f, angle, null, true, 5.5f, 7);
            } else {
                ShootSFX();
                Shotgun("BubbleRed", 7, 35, 14, angle);
                yield return new WaitForSeconds(0.1f);
                Shotgun("StarOrange", 5, 25, 11, angle, new Vector2(2, 2));
                Shotgun("CircleBallYellow", 7, 25, 11.5f, angle, null, true, 9.5f, 11.5f);
                Shotgun("CircleBallYellow", 5, 20, 10.5f, angle, null, true, 8.5f, 10.5f);
            }
            

            // random movement
            Transform pos = null;

            int rand = Random.Range(0, 5);

            switch (rand) {
                case 0:
                    pos = StageController.Stage.Waypoints.TOP_LEFT;
                    break;

                case 1:
                    pos = StageController.Stage.Waypoints.TOP_CENTER;
                    break;

                case 2:
                    pos = StageController.Stage.Waypoints.TOP_RIGHT;
                    break;

                case 3:
                    pos = StageController.Stage.Waypoints.MID_LEFT;
                    break;

                case 4:
                    pos = StageController.Stage.Waypoints.MID_RIGHT;
                    break;
            }

            User.GoToWaypoint(pos, 5);

            yield return new WaitForSeconds(0.4f);

            while (User.IsMoving()) yield return new WaitForSeconds(0.0001f);
        }
    }

    public override IEnumerator StartSpell() {
        Fireballs = User.gameObject.GetComponent<RetroBullets>();

        int[] fireballType = { 0, 0, 0, 1, 1, 1, 2, 2 };
        string[] shardTags = { "FireShardRed", "FireShardOrange", "FireShardOrange", "FireShardOrange", "FireShardYellow", "FireShardYellow", "FireShardYellow", "FireShardBlue" }; 
        string[] bubbleTags = { "BubbleRed", "BubbleRed", "BubbleOrange", "BubbleOrange", "BubbleOrange", "BubbleYellow", "BubbleYellow", "BubbleLightBlue" }; 
        string[] starTags = { "StarRed", "StarRed", "StarOrange", "StarOrange", "StarOrange", "StarYellow", "StarYellow",    "StarBlue" };

        const int maxstage = 7;
        const int MaxFireballsPerRow = 7; // just to be safe. Don't want too much light
        const int yspawn = 24;

        int stage = 0;
        int shots = 0;
        // Stages:
        // 0 - Red fire, red crystals and fireballs and stars
        // 1 - Orange crystals, red fireballs and stars
        // 2 - Orange crystals and stars, red fireballs
        // 3 - Orange crystals and stars + yellow fireballs
        // 4 - Yellow crystals and Orange stars and Yellow fireballs
        // 5 - Yellow everything
        // 6 - Blue fireballs
        // 7 - Blue everything

        while(true) {
            int rowFireballCount = 0;

            shots++;

            if(stage < maxstage && shots > maxstage + 4 - stage) {
                stage++;
                shots = 0;
            }

            ShootSFX();

            for(int i = -13; i <= 13; i++) {
                if (Mathf.Abs(i % 4) != 1) continue;

                GameObject bullet = null;
                float r = Random.Range(0, 1f);

                if(r <= 0.1f && rowFireballCount < MaxFireballsPerRow && !(stage == maxstage && rowFireballCount > MaxFireballsPerRow / 2)) {
                    // fireball (Spawn with instantiate, DO NOT POOL or HDR textures will flip out)
                    rowFireballCount++;

                    switch(fireballType[stage]) {
                        case 0:
                            bullet = Instantiate(Fireballs.RedFireball, new Vector3(i, yspawn, 0), Quaternion.identity);
                            break;

                        case 1:
                            bullet = Instantiate(Fireballs.YellowFireball, new Vector3(i, yspawn, 0), Quaternion.identity);
                            break;

                        case 2:
                            bullet = Instantiate(Fireballs.BlueFireball, new Vector3(i, yspawn, 0), Quaternion.identity);
                            break;
                    }
                } else if(r <= 0.5f) {
                    // Shard
                    bullet = ObjectPooler.Spawn(shardTags[stage], new Vector3(i, yspawn, 0), Quaternion.identity);
                } else if(r <= 0.7f) {
                    // Bubble
                    bullet = ObjectPooler.Spawn(bubbleTags[stage], new Vector3(i, yspawn, 0), Quaternion.identity);
                } else if(r <= 0.9f) {
                    // Star
                    bullet = ObjectPooler.Spawn(starTags[stage], new Vector3(i, yspawn, 0), Quaternion.identity);
                    bullet.transform.localScale = new Vector3(2, 2, 1);
                } else {
                    continue;
                }

                // now shoot at player, but with a wide potential offset
                Vector3 dir = StageController.Player.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.AngleAxis(angle - 90 + Random.Range(-35f, 35f), Vector3.forward);
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * Random.Range(6.2f + stage * 0.9f, 8.2f + stage * 0.9f);
            }

            float time;

            if (stage < 2) time = 0.75f;
            else if (stage < 4) time = 0.5f;
            else time = 0.44f;

            yield return new WaitForSeconds(time);
        }
    }
}
