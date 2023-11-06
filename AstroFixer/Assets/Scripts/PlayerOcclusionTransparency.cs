using UnityEngine;

public class PlayerOcclusionTransparency : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask occludingLayers;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Camera mainCamera;

    private Renderer occludingObjectRenderer;
    private Material originalOccludingMaterial;

    private void Update()
    {
        ApplyTransparencyToOccludingObjects();
    }

    private void ApplyTransparencyToOccludingObjects()
    {
        if (IsPlayerOccluded(out Renderer hitRenderer))
        {
            UpdateOccludingObject(hitRenderer);
        }
        else
        {
            RestoreMaterialToOriginal();
        }
    }

    private bool IsPlayerOccluded(out Renderer hitRenderer)
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - mainCamera.transform.position;
        bool isHit = Physics.Raycast(mainCamera.transform.position, directionToPlayer, out hit, Mathf.Infinity, occludingLayers);

        hitRenderer = isHit ? hit.collider.GetComponent<Renderer>() : null;

        return isHit && IsHitObjectCloserThanPlayer(hit);
    }

    private bool IsHitObjectCloserThanPlayer(RaycastHit hit)
    {
        float distanceToPlayer = Vector3.Distance(mainCamera.transform.position, player.position);
        return hit.distance < distanceToPlayer;
    }

    private void UpdateOccludingObject(Renderer hitRenderer)
    {
        if (hitRenderer != occludingObjectRenderer)
        {
            RestoreMaterialToOriginal();

            originalOccludingMaterial = hitRenderer.material;
            hitRenderer.material = transparentMaterial;
            occludingObjectRenderer = hitRenderer;
        }
    }

    private void RestoreMaterialToOriginal()
    {
        if (occludingObjectRenderer != null)
        {
            occludingObjectRenderer.material = originalOccludingMaterial;
            occludingObjectRenderer = null;
        }
    }
}
