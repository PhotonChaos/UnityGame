using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float CountdownTime;
    public float FadeTime;
    public bool DoFade;

    private void Start() {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut() {
        yield return new WaitForSeconds(CountdownTime);

        if (DoFade) {
            Material material = GetComponent<Renderer>().material;
            // Cache the current color of the material, and its initial opacity.
            Color matCol = material.GetColor("_Material_Color");
            Color glowCol = material.GetColor("_Glow_Color");

            float startOpacity = material.GetFloat("_Total_Alpha");

            // Track how many seconds we've been fading.
            float t = 0;

            while (t < FadeTime) {
                // Step the fade forward one frame.
                t += Time.deltaTime;
                // Turn the time into an interpolation factor between 0 and 1.
                float blend = Mathf.Clamp01(t / FadeTime);

                material.SetFloat("_Total_Alpha", Mathf.Lerp(startOpacity, 0, blend));

                // Blend to the corresponding opacity between start & target.
                // matCol.a = Mathf.Lerp(startOpacity, 0, blend);

                // Apply the resulting color to the material.
                // material.color = matCol;

                // Wait one frame, and repeat.
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
