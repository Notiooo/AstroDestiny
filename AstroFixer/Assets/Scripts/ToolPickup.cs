using UnityEngine;

public class ToolInteraction : MonoBehaviour
{
    public PlayerController playerController;
    public float normalSpeed;
    public float speedWithTool;
    public Transform toolHolder;
    public GameObject toolsParent;
    public float pickupRadius = 1f;

    private GameObject currentTool = null;

    private void Start()
    {
        if (playerController != null)
        {
            normalSpeed = playerController.movementSpeedGround;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press E to pick up or drop
        {
            if (currentTool == null)
            {
                TryPickupTool();
            }
            else
            {
                DropTool();
            }
        }
    }

    void TryPickupTool()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
        float minDistance = float.MaxValue;
        Collider closestTool = null;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Tool"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    closestTool = collider;
                    minDistance = distance;
                }
            }
        }

        if (closestTool != null)
        {
            currentTool = closestTool.gameObject;
            AttachTool(currentTool);
            playerController.movementSpeedGround = speedWithTool;
        }
    }

    void AttachTool(GameObject tool)
    {
        tool.transform.SetParent(toolHolder);
        tool.transform.localPosition = Vector3.zero;
    }

    void DropTool()
    {
        if (currentTool != null)
        {
            currentTool.transform.SetParent(toolsParent.transform);
            currentTool.transform.position = transform.position;
            currentTool = null;
            playerController.movementSpeedGround = normalSpeed;
        }
    }
}
