using UnityEngine;

public class LevitationEffect : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public Transform shadowTransform;

    private Vector3 shadowStartScale;
    private Vector3 startPosition;
    private float shadowMinScale = 0.5f;
    private SpriteRenderer shadowRenderer;

    void Start()
    {
        startPosition = transform.position;
        if (shadowTransform != null)
        {
            shadowStartScale = shadowTransform.localScale;
            shadowRenderer = shadowTransform.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        float levitation = Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
        transform.position = new Vector3(startPosition.x, startPosition.y + levitation, startPosition.z);
        if (shadowTransform != null)
        {
            float scale = 1 - (levitation / amplitude) * (1 - shadowMinScale);
            scale = Mathf.Clamp(scale, shadowMinScale, 1f);
            shadowTransform.localScale = new Vector3(shadowStartScale.x * scale, shadowStartScale.y, shadowStartScale.z * scale);

            Color shadowColor = shadowRenderer.color;
            shadowColor.a = scale;
            shadowRenderer.color = shadowColor;
        }
    }
}
