using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitateMovement : MonoBehaviour
{
    public float AngleOffset = 0;
    public SkeleBulletType BulletType;
    public float EndCircleRotation = 0;
    public float EndCircleDelay = 0;

    public bool OverrideDest = false;

    public SkeletalSerenade card;

    private Vector2 start;
    public Vector2 dest;
    private float rotationSpeed = 0.9f;  // degrees per frame
    private float pullSpeed = 0.0015f;
    private float lastRotation = 0;
    private Rigidbody2D rb;
    private long frameNum = 0;
    private long shotNum = 0;
    private bool endoverride = false;
    private string BulletTag = "EnergyBall";

    // Chronos stuff
    private bool onChronosSegment = false;
    private int chronosNum = 13;
    private int chronosCount;

    private Vector2 translate = Vector2.zero;
    private Vector2? scale = null;

    private Color RainbowCol = Color.white;

    private List<GameObject> bullets = new List<GameObject>();

    private void Start() {
        start = transform.position;
        PlayerBounds b = StageController.Player.GetComponent<PlayerController>().Bounds;
        if(!OverrideDest) dest = Vector2.Lerp(new Vector2(b.xMin, b.yMin), new Vector2(b.xMax, b.yMax), 0.5f);

        rb = gameObject.GetComponent<Rigidbody2D>();

        if(BulletType == SkeleBulletType.INK) {
            RainbowCol = Color.red;
        }
    }

    private void FixedUpdate() {
        frameNum++;

        transform.position = Vector2.Lerp(start, dest, pullSpeed * frameNum);
        transform.eulerAngles = Vector3.zero;
        gameObject.transform.RotateAround(dest, new Vector3(0, 0, 1), lastRotation + rotationSpeed);

        lastRotation += rotationSpeed;

        #region Shooting
        bool doShoot = false;
        float shotOffset = AngleOffset;

        if (frameNum % 5 == 0) {
            switch (BulletType) {
                case SkeleBulletType.SANS:
                    if (shotNum / 3 % 2 == 0) {
                        BulletTag = "KunaiBlue";
                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.PAPY:
                    if (shotNum / 3 % 2 == 0) {
                        BulletTag = "KunaiOrange";
                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.UNDERFELL:
                    if (shotNum % 8 > 1) {
                        BulletTag = "Kunai";

                        if (shotNum % 3 == 0) RainbowCol = GetCol(209, 36, 36); 
                        else if (shotNum % 3 == 1) RainbowCol = Color.grey;
                        else RainbowCol = GetCol(255, 248, 117);

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.UNDERFRESH:
                    if (shotNum % 8 > 3) {
                        if (shotNum % 3 == 0) BulletTag = "KunaiYellow";
                        else if (shotNum % 3 == 1) BulletTag = "KunaiPink";
                        else BulletTag = "KunaiPurple";
                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.DREAM:
                    if (shotNum % 8 > 1) {
                        BulletTag = "Kunai";

                        float r = Random.Range(0, 10);

                        if(r < 5) {
                            RainbowCol = GetCol(255, 252, 186);
                        } else if(r < 8) {
                            RainbowCol = GetCol(186, 255, 207);
                        } else {
                            RainbowCol = GetCol(186, 255, 249);
                        }

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.NIGHTMARE:
                    if (shotNum % 2 == 0) {
                        BulletTag = "Kunai";

                        float r = Random.Range(0, 10);

                        if (r < 7) {
                            RainbowCol = GetCol(107, 107, 107);
                        } else {
                            RainbowCol = GetCol(116, 248, 252);
                        }

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.UNDERVERSE:
                    if(shotNum / 2 % 2 == 0) {
                        BulletTag = "Kunai";

                        if(Random.Range(0, 2) == 0) {
                            RainbowCol = Color.red;
                        } else {
                            RainbowCol = Color.white;
                        }

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.DUSTTALE:
                    if (Random.Range(0, 7) != 0) {
                        BulletTag = "Kunai";

                        RainbowCol = GetCol(214, 214, 186);

                        int r = Random.Range(0, 6);

                        if (r == 0) {
                            RainbowCol = Color.red;
                        } else if (r == 2) {
                            RainbowCol = Color.blue;
                        }

                        shotOffset -= Random.Range(0f, 0.5f);

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.HORRORTALE:
                    if (shotNum % 5 != 0) {
                        BulletTag = "Kunai";

                        if(Random.Range(0, 8) > 2) {
                            RainbowCol = GetCol(69, 69, 69);
                        } else {
                            RainbowCol = GetCol(189, 72, 232);
                        }

                        shotOffset -= Random.Range(0, 10f);

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.UNDERSWAP:
                    if (shotNum / 5 % 2 == 0) { 
                        BulletTag = "Kunai";

                        RainbowCol = GetCol(100, 100, 100);

                        if (Random.Range(0, 10) > 7) {
                            RainbowCol = GetCol(255, 192, 66);
                        }

                        shotOffset -= Random.Range(0f, 1f);

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.PIRATE:
                    if (Mathf.Sin(shotNum) > -0.75f) {
                        BulletTag = "Kunai";

                        if(Mathf.Sin(shotNum) > 0.75f) {
                            RainbowCol = GetCol(54, 255, 171);
                        } else {
                            RainbowCol = GetCol(103, 181, 207);
                        }

                        shotOffset += -5 * Mathf.Sin(shotNum);

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.OUTERTALE:
                    if (shotNum / 3 % 2 == 0) {
                        BulletTag = "Kunai";

                        if(shotNum % 9 == 0) {
                            RainbowCol = GetCol(255, 200, 97);
                        } else {
                            RainbowCol = GetCol(54, 104, 179);
                        }

                        shotOffset += 12 * Mathf.Sin(shotNum);

                        doShoot = true;
                    }
                    break;

                case SkeleBulletType.INK:
                    BulletTag = "Star";

                    System.Drawing.Color c = System.Drawing.Color.FromArgb((int)(RainbowCol.r * 255), (int)(RainbowCol.g * 255), (int)(RainbowCol.b * 255));
                    float hue = (c.GetHue() + shotNum/7f) % 360 / 360f;
                    RainbowCol = Color.HSVToRGB(hue, 0.6f, 1);
                    gameObject.GetComponent<SpriteRenderer>().color = RainbowCol;

                    shotOffset -= gameObject.transform.rotation.eulerAngles.z;
                    Vector3 dir = (Vector3)dest - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
                    shotOffset += angle;

                    scale = new Vector2(0.5f, 0.5f);

                    doShoot = true;
                    break;

                case SkeleBulletType.ERROR:
                    BulletTag = "Kunai";
                    
                    switch(Random.Range(0, 10)) {
                        case 0:
                            RainbowCol = Color.blue;
                            break;

                        case 1:
                            RainbowCol = Color.red;
                            break;

                        case 2:
                            RainbowCol = Color.yellow;
                            break;

                        default:
                            RainbowCol = GetCol(74, 74, 74);
                            break;
                    }

                    doShoot = true;
                    break;

                case SkeleBulletType.CIDER:
                    if (shotNum % 2 == 0) BulletTag = "KunaiGreen";
                    else BulletTag = "KunaiPurple";

                    if(shotNum % 2 == 0) {
                        translate = Random.insideUnitCircle;
                    }

                    doShoot = true;
                    break;

                case SkeleBulletType.CHRONOS:
                    if(!onChronosSegment) {
                        onChronosSegment = true;
                        chronosCount = chronosNum;
                        chronosNum--;

                        if (chronosNum <= 0) chronosNum = 1;
                    }

                    if (chronosCount > 0) {
                        chronosCount--;

                        BulletTag = "Kunai";
                        RainbowCol = GetCol(0, 247, 255);

                        doShoot = true;
                    } else {
                        onChronosSegment = false;
                    }
                    break;

                case SkeleBulletType.ZEPHYR:
                    if(shotNum % 15 > 1) {
                        BulletTag = "KunaiGreen";
                        shotOffset = shotNum;

                        if (shotNum % 5 == 0) {
                            shotOffset -= 180;
                            BulletTag = "KunaiYellow";
                        }

                        doShoot = true;
                    }
                    break;
            }

            if (doShoot) {
                StageController.AudioManager.PlaySFX("shootbullet", 0.1f);
                GameObject bullet = ObjectPooler.Spawn(BulletTag, transform.position, Quaternion.identity);

                if(scale != null) {
                    bullet.transform.localScale = scale.Value;
                }

                if(BulletTag == "Kunai" || BulletTag == "Star") {
                    SpriteRenderer rend = bullet.GetComponent<SpriteRenderer>();
                    rend.color = RainbowCol;
                    rend.sortingOrder = (int)shotNum;
                }

                bullet.transform.Translate(translate);
                bullet.transform.Rotate(0, 0, transform.rotation.eulerAngles.z + shotOffset);

                if(BulletType == SkeleBulletType.ERROR) {
                    Vector2 acc = Random.insideUnitCircle * 0.03f;
                    bullet.GetOrAddComponent<SimpleMovement>().UpdateAcceleration(acc.x, acc.y);
                }

                if(BulletType == SkeleBulletType.ZEPHYR) {
                    SkeletalSerenade.ZephyrBullets.Add(bullet);
                }

                bullets.Add(bullet);
            }

            shotNum++;
        }
        #endregion

        if (pullSpeed * frameNum >= 1) {
            card.ShootCirclesStart(EndCircleDelay, BulletTag, EndCircleRotation, transform.position, RainbowCol, BulletType);

            foreach (GameObject bullet in bullets) {
                bullet.GetOrAddComponent<SimpleMovement>().UpdateAcceleration(bullet.transform.up.x, bullet.transform.up.y);
            }

            ObjectPooler.Disable(gameObject);
            return;
        }
    }

    public static Color GetCol(int r, int g, int b) {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}

public enum SkeleBulletType
{
    SANS,
    PAPY,

    UNDERFELL,
    UNDERFRESH,

    DREAM,
    NIGHTMARE, 

    UNDERVERSE,
    DUSTTALE, 

    HORRORTALE,
    UNDERSWAP, 

    PIRATE,
    OUTERTALE,

    INK,
    ERROR, // done

    CIDER,
    CHRONOS,
    ZEPHYR
}