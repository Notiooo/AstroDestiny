using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class InSpaceVignetteFade : MonoBehaviour
{
    public PostProcessVolume volume;
    public Image blackoutPanel;
    private Vignette vignette;
    private Grain grain;
    public TextMeshProUGUI messageText;
    public float vignetteFadeDuration = 5.0f;
    public float blackoutFadeDuration = 5.0f;
    public float textFadeDuration = 2.0f;

    private void Start()
    {
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out grain);
        blackoutPanel.color = new Color(0, 0, 0, 0);
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InsideZone")) 
        {
            StopAllCoroutines();
            StartCoroutine(Fade(0f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InsideZone"))
        {
            StopAllCoroutines();
            StartCoroutine(Fade(1f));
        }
    }

    private IEnumerator Fade(float targetIntensity)
    {
        float timeElapsed = 0f;
        float startIntensity = vignette.intensity.value;

        while (timeElapsed < vignetteFadeDuration)
        {
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / vignetteFadeDuration);
            grain.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / vignetteFadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
        grain.intensity.value = targetIntensity;

        if (targetIntensity == 1f)
        {
            StartCoroutine(FadeScreenToBlack(blackoutFadeDuration));
        }
    }

    private IEnumerator FadeScreenToBlack(float duration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / duration);
            blackoutPanel.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        blackoutPanel.color = new Color(0, 0, 0, 1f);
        StartCoroutine(FadeTextToFullAlpha(textFadeDuration, messageText));
    }

    public IEnumerator FadeTextToFullAlpha(float duration, TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(0f, 1f, timeElapsed / duration));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }
}
