using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUIController : MonoBehaviour
{
    public GameObject ArcyBar;
    public GameObject CarbonBar;

    public Color ArcyMin;
    public Color ArcyMax;
    public Color CarbonMin;
    public Color CarbonMax;

    private PlayerController pc;
    private Image ArcyBarImage;
    private Image CarbonBarImage;
    private float maxSize;
    private bool isArcy;

    // Start is called before the first frame update
    void Start() {
        ArcyBarImage = ArcyBar.GetComponent<Image>();
        CarbonBarImage = CarbonBar.GetComponent<Image>();
        maxSize = ArcyBarImage.rectTransform.sizeDelta.x;
        pc = StageController.Player.GetComponent<PlayerController>();
        isArcy = pc.Name == PlayerName.ARCY;
        
        if(isArcy) {
            CarbonBar.SetActive(false);
        } else {
            ArcyBar.SetActive(false);
        }

        ArcyBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        CarbonBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
    }

    // Update is called once per frame
    void Update() {
        if (isArcy) {
            ArcyBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize * pc.GetPercentMaxPower());
            ArcyBarImage.color = Color.Lerp(ArcyMin, ArcyMax, pc.GetPercentMaxPower());
        } else {
            CarbonBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize * pc.GetPercentMaxPower());
            CarbonBarImage.color = Color.Lerp(CarbonMin, CarbonMax, pc.GetPercentMaxPower());
        }
    }
}
