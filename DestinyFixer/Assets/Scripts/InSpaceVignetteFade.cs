using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class InSpaceVignetteFade : MonoBehaviour, IZoneListener
{
    public PostProcessVolume volume;
    private Vignette vignette;
    private Grain grain;
    public float vignetteFadeDuration = 5.0f;

    private void Start()
    {
        ZoneTracker.Instance.RegisterListener(this);
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out grain);
    }

    private void OnDestroy()
    {
        ZoneTracker.Instance.UnregisterListener(this);
    }

    public void OnZoneEnter()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(0f));
    }

    public void OnZoneExit()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(1f));
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
            GameplayManager.Instance.EndGame("You are lost in a black abyss.\n\n A slow and unpleasant death from lack of oxygen awaits you.");
        }
    }
}
