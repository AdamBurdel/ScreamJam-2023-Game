using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum FadeType
{
    FadeIn,
    FadeOut,
    FadeInOut,
    FadeOutIn,
    FadeInDelay,
    FadeOutDelay
}

public class UIFade : MonoBehaviour
{
    [Tooltip("The UI element you want to fade")]
    [SerializeField]
    private MaskableGraphic element;

    [Tooltip("Fade type")]
    [SerializeField]
    private FadeType fadeType;

    [Tooltip("Fade time")]
    [SerializeField]
    private float fadeTime = 1f;

    private Color color;

    void Start()
    {
        color = element.color;
        switch (fadeType)
        {
            case FadeType.FadeIn:
                StartCoroutine(FadeIn());
                break;
            case FadeType.FadeOut:
                StartCoroutine(FadeOut());
                break;
            case FadeType.FadeInOut:
                StartCoroutine(FadeInOut());
                break;
            case FadeType.FadeOutIn:
                StartCoroutine(FadeOutIn());
                break;
            case FadeType.FadeInDelay:
                StartCoroutine(FadeInDelay());
                break;
            case FadeType.FadeOutDelay:
                StartCoroutine(FadeOutDelay());
                break;
        }
    }

    private IEnumerator FadeOut()
    {
        for (float a = fadeTime; a >= 0; a -= Time.deltaTime)
        {
            element.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        for (float a = 0; a <= fadeTime; a += Time.deltaTime)
        {
            element.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }
    }

    private IEnumerator FadeInOut()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(fadeTime);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOutIn()
    {
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeTime);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeInDelay() {        
        yield return new WaitForSeconds(fadeTime);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOutDelay()
    {
        yield return new WaitForSeconds(fadeTime);
        StartCoroutine(FadeOut());
    }

}
