using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerOcclusionTransparency : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float playerYOffset;
    [SerializeField] private LayerMask occludingLayers;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Camera mainCamera;

    private class OccludingObject {
        public OccludingObject(Renderer _renderer, Material _material)
        {
            renderer = _renderer;
            originalMaterial = _material;
            hitThisFrame = true;
        }
        public Renderer renderer;
        public Material originalMaterial;
        public bool hitThisFrame;
    };
    private List<OccludingObject> occludingObjects = new List<OccludingObject>();

    private void Update()
    {
        ApplyTransparencyToOccludingObjects();
    }

    private void ApplyTransparencyToOccludingObjects()
    {
        ResetOccludersHitMemory();
        if (IsPlayerOccluded(out List<Renderer> hitRenderers))
        {
            Debug.Log(hitRenderers.Count);
            for(int i = 0; i < hitRenderers.Count; i++){
                UpdateOccludingObject(hitRenderers[i]);
            }
        }
        RestoreMaterialsToOriginal();
    }

    private bool IsPlayerOccluded(out List<Renderer> hitRenderers)
    {
        RaycastHit[] hits;
        hitRenderers = new List<Renderer>();
        Vector3 directionToPlayer = player.position + Vector3.up * playerYOffset - mainCamera.transform.position;
        float distanceToPlayer = Vector3.Distance(mainCamera.transform.position, player.position);
        hits = Physics.RaycastAll(mainCamera.transform.position, directionToPlayer, distanceToPlayer, occludingLayers);

        for(int i = 0; i < hits.Length; i++) {
            hitRenderers.Add(hits[i].collider.GetComponent<Renderer>());
        }
        return hitRenderers.Count > 0;
    }

    private void ResetOccludersHitMemory() {
        for(int i = 0; i < occludingObjects.Count; i++)
        {
            occludingObjects[i].hitThisFrame = false;
        }
    }

    private void UpdateOccludingObject(Renderer hitRenderer)
    {
        bool newOccluder = true;
        for(int i = 0; i < occludingObjects.Count; i++)
        {
            if(hitRenderer == occludingObjects[i].renderer)
            {
                occludingObjects[i].hitThisFrame = true;
                newOccluder = false;
            }
        }
        if (newOccluder)
        {
            occludingObjects.Add(
                new OccludingObject(
                    hitRenderer,
                    hitRenderer.material
                )
            );
            hitRenderer.material = transparentMaterial;
        }
    }

    private void RestoreMaterialsToOriginal()
    {
        for(int i = 0; i < occludingObjects.Count; i++)
        {
            if(!occludingObjects[i].hitThisFrame)
            {
                occludingObjects[i].renderer.material = occludingObjects[i].originalMaterial;
                occludingObjects.RemoveAt(i);
            }
        }
    }
}
