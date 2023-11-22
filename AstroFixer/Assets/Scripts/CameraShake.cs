using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 1.0f;
    public Camera camera;
    private bool isInInsideZone = true;

    void Start()
    {
        StartCoroutine(RandomShake());
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = camera.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            originalPosition = camera.transform.localPosition;
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            camera.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InsideZone"))
        {
            isInInsideZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InsideZone"))
        {
            isInInsideZone = false;
        }
    }

    public IEnumerator RandomShake()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 30f));
            if (isInInsideZone)
            {
                StartCoroutine(Shake(shakeDuration, shakeMagnitude));
            }
        }
    }
}
