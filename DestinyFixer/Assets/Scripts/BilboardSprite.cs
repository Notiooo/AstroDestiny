using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Camera cameraToLookAt;

    void Start()
    {
        if (cameraToLookAt == null)
        {
            cameraToLookAt = Camera.main;
        }
    }

    void Update()
    {
        transform.LookAt(transform.position + cameraToLookAt.transform.rotation * Vector3.forward,
            cameraToLookAt.transform.rotation * Vector3.up);
    }
}
