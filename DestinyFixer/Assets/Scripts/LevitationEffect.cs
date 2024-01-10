using UnityEngine;

public class LevitationEffect : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public Transform shadowTransform;

    private Vector3 shadowStartScale;
    private float levitationOffset = 0f;
    private float lastLevitationOffset = 0f;
    private float shadowMinScale = 0.5f;
    private SpriteRenderer shadowRenderer;

    void Start()
    {
        if (shadowTransform != null)
        {
            shadowStartScale = shadowTransform.localScale;
            shadowRenderer = shadowTransform.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        lastLevitationOffset = levitationOffset;
        levitationOffset = Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        // Apply levitation offset
        Vector3 levitationDelta = new Vector3(0f, levitationOffset - lastLevitationOffset, 0f);
        transform.position += levitationDelta;

        if (shadowTransform != null)
        {
            float scale = 1 - (levitationOffset / amplitude) * (1 - shadowMinScale);
            scale = Mathf.Clamp(scale, shadowMinScale, 1f);
            shadowTransform.localScale = new Vector3(shadowStartScale.x * scale, shadowStartScale.y, shadowStartScale.z * scale);

            Color shadowColor = shadowRenderer.color;
            shadowColor.a = scale;
            shadowRenderer.color = shadowColor;
        }
    }
}
