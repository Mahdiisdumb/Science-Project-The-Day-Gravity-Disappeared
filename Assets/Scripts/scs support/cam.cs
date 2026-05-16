using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Image fadeImage;

    Coroutine fadeRoutine;

    void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }
    }

    public void FadeIn()
    {
        StartFade(0, 0); // transparent target
    }

    public void FadeOut()
    {
        StartFade(1, 1); // black target
    }

    void StartFade(float targetAlpha, float mode)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        float startAlpha = fadeImage.color.a;
        fadeRoutine = StartCoroutine(Fade(startAlpha, targetAlpha));
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t);
            c.a = a;
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }

    public void Effect(string e)
    {
        Debug.Log("Effect: " + e);
    }
}