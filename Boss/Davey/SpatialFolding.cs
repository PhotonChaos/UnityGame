using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialFolding : SpellCardBase
{
    public GameObject SourceRift;
    public GameObject WarpCircle;

    public override IEnumerator StartCooldown() {
        Boundary.Warp = true;
        Boundary.WarpThreshold = 2;

        int loopCount = 0;

        while(true) {
            loopCount++;

            Transform pos = StageController.Stage.Waypoints.TOP_LEFT;

            int rand = Random.Range(0, 6);

            switch(rand) {
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
                    pos = StageController.Stage.Waypoints.MID_CENTER;
                    break;

                case 5:
                    pos = StageController.Stage.Waypoints.MID_RIGHT;
                    break;
            }

            User.GoToWaypoint(pos, 2);

            for(int i = 0; i < 3; i++) {
                ShootSFX();
                StartCoroutine(CircleBullet("BubblePink", Random.Range(5, 7), Random.Range(7f, 9.5f), rotation:Random.Range(0f, 100f)));
                yield return new WaitForSeconds(0.26f);
            }

            // magic shotgun
            // wait until done moving
            while(User.moving) {
                yield return new WaitForSeconds(0.0001f);
            }

            List<GameObject> magicShotgun = new List<GameObject>(25);
            string[] tags = { "CircleBallGreen", "CircleBallPurple", "BubblePink" };

            const float sizeMod = 0.6f;

            for(int i = 0; i < 25; i++) {
                string tag = tags[Random.Range(0, tags.Length)];

                GameObject magicBullet = ObjectPooler.Spawn(tag, transform.position, Quaternion.identity);

                int row = Random.Range(-2, 3);
                int col = Random.Range(-2, 3);

                magicBullet.transform.Translate(row/sizeMod, col/sizeMod, 0);

                magicShotgun.Add(magicBullet);
                if(i % 2 == 0) ShootSFX();
                yield return new WaitForSeconds(0.04f);
            }

            yield return new WaitForSeconds(0.5f);
            ShootSFX();
            foreach(GameObject bullet in magicShotgun) {
                bullet.GetComponent<Rigidbody2D>().velocity = (StageController.Player.transform.position - bullet.transform.position).normalized * Random.Range(5f, 9.5f);
            }

            if (loopCount % 5 == 0) yield return new WaitForSeconds(3f);
            else yield return new WaitForSeconds(0.5f);
        }
    }

    public override IEnumerator StartSpell() {
        Boundary.Warp = false;
        Boundary.WarpThreshold = 1;
        YoYoShoot.DefaultChainSpawn = false;

        GameObject source = Instantiate(SourceRift, StageController.Stage.Waypoints.TOP_CENTER.position, Quaternion.identity);
        source.transform.Translate(0, -4, 0);
        source.GetComponent<RiftWarp>().BossUser = User.gameObject;

        Transform[] circlePoses = { StageController.Stage.Waypoints.TOP_LEFT, 
            StageController.Stage.Waypoints.TOP_RIGHT, 
            StageController.Stage.Waypoints.MID_LEFT,
            StageController.Stage.Waypoints.MID_RIGHT,
            StageController.Stage.Waypoints.BOT_LEFT,
            StageController.Stage.Waypoints.BOT_RIGHT};

        for(int i = 0; i < circlePoses.Length; i++) {
            GameObject circle = Instantiate(WarpCircle, circlePoses[i].position, Quaternion.identity);
            circle.transform.Rotate(0, 0, Random.Range(-30f, 30f));
            circle.GetComponent<RiftWarp>().BossUser = User.gameObject;
        }

        while(true) {
            ShootSFX();
            GameObject bullet = ObjectPooler.Spawn("BubbleYoYo", transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, Random.Range(-9f, -7f));

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDestroy() {
        YoYoShoot.DefaultChainSpawn = true;
    }
}
