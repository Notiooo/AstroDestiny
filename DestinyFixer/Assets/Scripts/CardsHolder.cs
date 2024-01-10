using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardsHolder : MonoBehaviour
{
    private const float CardsDepth = 10.0f;

    private readonly GameObject[] cardsInHand = new GameObject[3];

    private Camera mainCamera;
    private Vector3 leftBottom;


    [SerializeField]
    private GameObject openDoorsPrefab;

    [SerializeField]
    private GameObject repairPrefab;

    [SerializeField]
    private GameObject speedPrefab;

    private int selectedCardIndex = -1;

    private void AddCard(GameObject cardPrefab, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > cardsInHand.Length)
        {
            Debug.LogError("Incorrect slot index");
            return;
        }

        GameObject card = Instantiate(cardPrefab);
        card.transform.SetParent(transform);
        card.transform.localPosition = new Vector3(0, 0, CardsDepth);
        card.transform.localScale = new Vector3(0.06f, 0.08f, 1.0f);
        card.transform.localEulerAngles = new Vector3(0, 0, 0);

        cardsInHand[slotIndex] = card;
    }

    private void AddOpenDoorsCard()
    {
        AddCard(openDoorsPrefab, 0);
    }

    private void AddRepairCard()
    {
        AddCard(repairPrefab, 1);
    }

    private void AddSpeedCard()
    {
        AddCard(speedPrefab, 2);
    }

    private void Awake()
    {
        if (!transform.parent.TryGetComponent(out mainCamera))
        {
            Debug.LogError("Could not find main camera in the parent node");
            return;
        }

        leftBottom = mainCamera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, CardsDepth));
        leftBottom = mainCamera.transform.InverseTransformPoint(leftBottom);

        AddOpenDoorsCard();
        AddRepairCard();
        AddSpeedCard();
    }

    private void Update()
    {
        selectedCardIndex = -1;
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f))
        {
            var flyingCard = hitInfo.transform.GetComponent<FlyingCard>();
            if (flyingCard != null)
            {
                selectedCardIndex = flyingCard.HandIndex;
            }
        }

        UpdateCardTransforms();
    }

    private void UpdateCardTransforms()
    {
        float angle = 0.0f;
        float displacementX = 0.2f;
        float displacementY = 0.5f;

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            GameObject cardGameObject = cardsInHand[i];
            SpriteRenderer spriteRenderer = cardGameObject.GetComponent<SpriteRenderer>();

            if (cardGameObject == null)
            {
                continue;
            }

            if (selectedCardIndex == i)
            {
                float x = leftBottom.x + displacementX;
                float y = leftBottom.y + displacementY;

                if (i == 0)
                {
                    y += 0.2f;
                }
                else if (i == 1)
                {
                    y += 0.2f;
                    x += 0.2f;
                }
                else
                {
                    x += 0.2f;
                }

                cardGameObject.transform.localPosition = Vector3.Slerp(
                    new Vector3(x, y, CardsDepth - 0.2f),
                    cardGameObject.transform.localPosition,
                    0.99f
                );
                spriteRenderer.sortingOrder = 32000;
            }
            else
            {
                cardGameObject.transform.localPosition = Vector3.Slerp(
                    new Vector3(leftBottom.x + displacementX, leftBottom.y + displacementY, CardsDepth),
                    cardGameObject.transform.localPosition,
                    0.99f
                );
                spriteRenderer.sortingOrder = 31000 + i;
            }
            

            float lerpedAngle = Mathf.LerpAngle(angle, cardGameObject.transform.localEulerAngles.z, 0.99f);
            cardGameObject.transform.localEulerAngles = new Vector3(0, 0, lerpedAngle);

            angle -= 30.0f;
            displacementY -= 0.18f;
            displacementX += 0.12f;
        }
    }
}
