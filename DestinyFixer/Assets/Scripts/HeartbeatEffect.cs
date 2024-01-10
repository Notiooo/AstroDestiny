using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class HeartbeatEffect : MonoBehaviour
{
    [SerializeField] private Image uiImage;
    [SerializeField] private float heartbeatDistanceThreshold = 10f;
    [SerializeField] private AnimationCurve heartbeatCurve;
    [SerializeField] private float minHeartbeatRate = 1f;
    [SerializeField] private float maxHeartbeatRate = 3f;

    private Transform playerTransform;
    private Transform enemyTransform;
    private float timeSinceLastBeat;
    private bool isHeartbeatActive;
    private const float FadeOutDuration = 1f;

    private void Start()
    {
        playerTransform = FindObjectWithTag("Player");
        enemyTransform = FindObjectWithTag("Enemy");
        uiImage.enabled = false;
    }

    private void Update()
    {
        ManageHeartbeatEffect();
    }

    private void ManageHeartbeatEffect()
    {
        float distanceToEnemy = Vector3.Distance(playerTransform.position, enemyTransform.position);
        if (distanceToEnemy < heartbeatDistanceThreshold)
        {
            ActivateHeartbeat();
            PulseHeartbeat(distanceToEnemy);
        }
        else if (isHeartbeatActive)
        {
            StartCoroutine(FadeOutImage());
        }
    }

    private void ActivateHeartbeat()
    {
        if (!isHeartbeatActive)
        {
            isHeartbeatActive = true;
            uiImage.enabled = true;
        }
    }

    private IEnumerator FadeOutImage()
    {
        float initialAlpha = uiImage.color.a;

        for (float t = 0; t < 1; t += Time.deltaTime / FadeOutDuration)
        {
            uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, Mathf.Lerp(initialAlpha, 0, t));
            yield return null;
        }

        isHeartbeatActive = false;
        uiImage.enabled = false;
    }

    private void PulseHeartbeat(float distanceToEnemy)
    {
        float heartbeatRate = Mathf.Lerp(maxHeartbeatRate, minHeartbeatRate, distanceToEnemy / heartbeatDistanceThreshold);
        timeSinceLastBeat += Time.deltaTime;

        float beatInterval = 1f / heartbeatRate;
        if (timeSinceLastBeat > beatInterval)
        {
            timeSinceLastBeat = 0f;
        }

        float alpha = heartbeatCurve.Evaluate(timeSinceLastBeat / beatInterval);
        uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, alpha);
    }

    private Transform FindObjectWithTag(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj == null)
        {
            Debug.LogError($"Object with tag '{tag}' not found.");
            return null;
        }

        return obj.transform;
    }
}
