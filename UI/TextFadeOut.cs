using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextFadeOut : MonoBehaviour
{
    //Fade time in seconds
    [Min(0)]
    public float fadeInTime;

    [Min(0)]
    public float stayTime;

    [Min(0)]
    public float fadeOutTime;

    public void FadeOut() {
        StartCoroutine(FadeInOut());
    }

    private IEnumerator FadeInOut() {
        Text text = GetComponent<Text>();
        Color originalColor = text.color;
        for (float t = 0.01f; t < fadeInTime; t += Time.deltaTime) {
            text.color = Color.Lerp(Color.clear, originalColor, Mathf.Min(1, t / fadeInTime));
            yield return null;
        }

        yield return new WaitForSeconds(stayTime);

        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime) {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }
    }
}