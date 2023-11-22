using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectiveIndicator : MonoBehaviour
{
    public struct ObjectiveData
    {
        public Objective objective;
        public Image onScreenIndicator;
        public Image edgeIndicator;
        public Image iconOverlay;
    }

    [SerializeField] public GameObject player;
    [SerializeField] public Image onScreenIndicatorPrefab;
    [SerializeField] public Image edgeIndicatorPrefab;
    [SerializeField] public Image iconOverlayPrefab;
    [SerializeField] public Canvas uiCanvas;
    [SerializeField] public GameObject parentOfObjectives;

    public List<ObjectiveData> objectiveList = new List<ObjectiveData>();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        // Objective[] initialObjectives = FindObjectsOfType<Objective>();
        // foreach (Objective obj in initialObjectives)
        // {
        //     AddObjective(obj);
        // }
    }

    private void Update()
    {
        foreach (ObjectiveData objectiveData in objectiveList)
        {
            Vector3 screenPosition = mainCamera.WorldToViewportPoint(objectiveData.objective.transform.position);

            if (IsOnScreen(screenPosition))
            {
                displayOnlyScreenIndicator(objectiveData, screenPosition);
            }
            else
            {
                displayOnlyEdgeIndicator(objectiveData, screenPosition);
            }
            displayIconOnTheIndicator(objectiveData, screenPosition);
        }
        //removeIndicatorsWhichAreClose();
    }

    public void AddObjective(Objective newObjective)
    {
        Image onScreenIndicatorInstance = Instantiate(onScreenIndicatorPrefab, parentOfObjectives.transform);
        Image edgeIndicatorInstance = Instantiate(edgeIndicatorPrefab, parentOfObjectives.transform);
        Image iconOverlayInstance = Instantiate(iconOverlayPrefab, parentOfObjectives.transform);

        onScreenIndicatorInstance.enabled = false;
        edgeIndicatorInstance.enabled = false;
        iconOverlayInstance.enabled = false;

        if (newObjective.objectiveIcon)
        {
            iconOverlayInstance.sprite = newObjective.objectiveIcon;
        }

        ObjectiveData newData = new ObjectiveData
        {
            objective = newObjective,
            onScreenIndicator = onScreenIndicatorInstance,
            edgeIndicator = edgeIndicatorInstance,
            iconOverlay = iconOverlayInstance
        };
        objectiveList.Add(newData);
    }

    public void RemoveObjective(Objective objectiveToRemove)
    {
        ObjectiveData dataToRemove = objectiveList.Find(data => data.objective == objectiveToRemove);

        if (dataToRemove.objective != null)
        {
            Destroy(dataToRemove.onScreenIndicator.gameObject);
            Destroy(dataToRemove.edgeIndicator.gameObject);
            Destroy(dataToRemove.iconOverlay.gameObject);

            objectiveList.Remove(dataToRemove);
        }
    }

    // private void removeIndicatorsWhichAreClose()
    // {
    //     List<Objective> objectivesToRemove = new List<Objective>();
    //     foreach (ObjectiveData objectiveData in objectiveList)
    //     {
    //         Vector3 playerPos = player.transform.position;
    //         Vector3 objectivePos = objectiveData.objective.transform.position;

    //         float distance = Vector3.Distance(playerPos, objectivePos);
    //         if (distance < 1.0f)
    //         {
    //             objectivesToRemove.Add(objectiveData.objective);
    //         }
    //     }
    //     foreach (Objective objective in objectivesToRemove)
    //     {
    //         RemoveObjective(objective);
    //     }
    // }

    private void displayOnlyScreenIndicator(ObjectiveData data, Vector3 screenPosition)
    {
        data.onScreenIndicator.enabled = true;
        data.edgeIndicator.enabled = false;
        data.onScreenIndicator.rectTransform.position = new Vector3(
            screenPosition.x * Screen.width, screenPosition.y * Screen.height, 0);
    }

    private void displayOnlyEdgeIndicator(ObjectiveData data, Vector3 screenPosition)
    {
        data.onScreenIndicator.enabled = false;
        data.edgeIndicator.enabled = true;
        screenPosition = flipAxisWhenObjectIsBehindCamera(screenPosition);
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0);
        float angle = GetAngleFromVector(screenPosition - screenCenter);
        angle -= 90; // indicator image is facing north
        data.edgeIndicator.rectTransform.eulerAngles = new Vector3(0, 0, angle);
        data.edgeIndicator.rectTransform.position = GetEdgePosition(screenPosition);
    }

    private void displayIconOnTheIndicator(ObjectiveData data, Vector3 screenPosition)
    {
        if (data.onScreenIndicator.enabled || data.edgeIndicator.enabled)
        {
            data.iconOverlay.enabled = true;
            Vector3 position = data.onScreenIndicator.enabled ? data.onScreenIndicator.rectTransform.position : data.edgeIndicator.rectTransform.position;
            data.iconOverlay.rectTransform.position = position;
        }
        else
        {
            data.iconOverlay.enabled = false;
        }
    }

    private bool IsOnScreen(Vector3 screenPosition)
    {
        return screenPosition.x >= 0 && screenPosition.x <= 1 && screenPosition.y >= 0 && screenPosition.y <= 1;
    }

    private Vector3 flipAxisWhenObjectIsBehindCamera(Vector3 screenPosition)
    {
        if (screenPosition.z < 0)
        {
            screenPosition.y = 1f - screenPosition.y;
            screenPosition.x = 1f - screenPosition.x;
        }
        return screenPosition;
    }

    private float GetAngleFromVector(Vector2 dir)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, dir);
        float angle = rotation.eulerAngles.z;
        return angle;
    }

    private Vector3 GetEdgePosition(Vector3 screenPos)
    {
        float screenBorderX = 50.0f / (float)Screen.width;
        float screenBorderY = 50.0f / (float)Screen.height;

        if (screenPos.x < screenBorderX) screenPos.x = screenBorderX;
        if (screenPos.x > 1f - screenBorderX) screenPos.x = 1f - screenBorderX;
        if (screenPos.y < screenBorderY) screenPos.y = screenBorderY;
        if (screenPos.y > 1f - screenBorderY) screenPos.y = 1f - screenBorderY;

        return new Vector3(
            screenPos.x * Screen.width,
            screenPos.y * Screen.height,
            0
        );
    }
}