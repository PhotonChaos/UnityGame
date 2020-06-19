using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossUI : MonoBehaviour
{
    public GameObject UIBoss;
    public GameObject BossLives;
    public GameObject BossName;
    public GameObject BossHealth;
    public GameObject CardTime;
    public GameObject CardName;

    private TextMeshProUGUI livesText;
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI spellText;

    private float startingWidth;
    private float startingX;
    private Boss boss;

    // Start is called before the first frame update
    void Start()
    {
        startingWidth = BossHealth.transform.localScale.x;
        startingX = BossHealth.transform.position.x;

        boss = UIBoss.GetComponent<Boss>();

        livesText = BossLives.GetComponent<TextMeshProUGUI>();
        timeText = CardTime.GetComponent<TextMeshProUGUI>();
        spellText = CardName.GetComponent<TextMeshProUGUI>();

        BossName.GetComponent<TextMeshProUGUI>().text = boss.BossName;

    }

    void Update() {
        if (UIBoss == null) {
            Destroy(gameObject);
            return;
        }

        // Update Health
        float perc = boss.GetHealthPercent();
        BossHealth.transform.localScale = new Vector3(startingWidth * perc, BossHealth.transform.localScale.y, BossHealth.transform.localScale.z);

        // Update Lives
        livesText.text = $"{boss.Lives}";

        // Update Timeout
        timeText.text = $"{boss.GetSpellTimeText()}";

        // Update Spell Name
        spellText.text = boss.GetSpellCardName();
    }
}
