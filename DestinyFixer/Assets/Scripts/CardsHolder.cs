using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CardsHolder : MonoBehaviour
{
    public static CardsHolder Instance { get ; private set; }

    private const float CardsDepth = 10.0f;
    private const float TimeScale = 0.98f;

    private readonly GameObject[] cardsInHand = new GameObject[3];
    private readonly LinkedList<GameObject> cardsTrash = new LinkedList<GameObject>();

    private Camera mainCamera;
    private Vector3 leftBottom;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private GameObject openDoorsPrefab;

    [SerializeField]
    private GameObject repairPrefab;

    [SerializeField]
    private GameObject speedPrefab;

    private int selectedCardIndex = -1;

    public bool OpenDoorCardInHand => cardsInHand[0] != null;
    public bool RepairCardInHand => cardsInHand[1] != null;
    public bool SpeedCardInHand => cardsInHand[2] != null;

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

    public void AddOpenDoorsCard()
    {
        if (cardsInHand[0] == null)
            AddCard(openDoorsPrefab, 0);
    }

    public void DiscardOpenDoorsCard()
    {
        cardsTrash.AddLast(cardsInHand[0]);
        cardsInHand[0] = null;
    }

    public void AddRepairCard()
    {
        if (cardsInHand[1] == null)
            AddCard(repairPrefab, 1);
    }

    public void AddSpeedCard()
    {
        if (cardsInHand[2] == null)
            AddCard(speedPrefab, 2);
    }

    private void Awake()
    {
        Instance = this;

        if (!transform.parent.TryGetComponent(out mainCamera))
        {
            Debug.LogError("Could not find main camera in the parent node");
            return;
        }

        leftBottom = mainCamera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, CardsDepth));
        leftBottom = mainCamera.transform.InverseTransformPoint(leftBottom);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddSpeedCard();
        }

        selectedCardIndex = -1;
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f))
        {
            if (hitInfo.transform.TryGetComponent<FlyingCard>(out var flyingCard))
            {
                selectedCardIndex = flyingCard.HandIndex;
            }
        }

        if (Input.GetMouseButtonDown(0) && selectedCardIndex == 2)
        {
            cardsTrash.AddLast(cardsInHand[2]);
            cardsInHand[2] = null;

            playerController.StartSprinting();
            StartCoroutine(StopSprint());
        }

        UpdateCardTransforms();
        UpdateCardsTrash();
    }

    private IEnumerator StopSprint()
    {
        yield return new WaitForSeconds(5);
        playerController.StopSprinting();
    }

    private void UpdateCardsTrash()
    {
        var toDestroy = new List<GameObject>();
        foreach (var card in cardsTrash)
        {
            var sr = card.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a * 0.97f);
                card.transform.position += Time.deltaTime * 0.4f * Vector3.right;
                if (sr.color.a < 0.1f)
                {
                    toDestroy.Add(card);
                }
            }
        }

        foreach (var card in toDestroy)
        {
            Destroy(card);
            cardsTrash.Remove(card);
        }
    }

    private void UpdateCardTransforms()
    {
        float angle = 0.0f;
        float displacementX = 0.2f;
        float displacementY = 0.5f;

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            GameObject cardGameObject = cardsInHand[i];
            if (cardGameObject == null)
            {
                continue;
            }

            SpriteRenderer spriteRenderer = cardGameObject.GetComponent<SpriteRenderer>();
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
                    TimeScale
                );
                spriteRenderer.sortingOrder = 31000 + i;
            }
            

            float lerpedAngle = Mathf.LerpAngle(angle, cardGameObject.transform.localEulerAngles.z, TimeScale);
            cardGameObject.transform.localEulerAngles = new Vector3(0, 0, lerpedAngle);

            angle -= 30.0f;
            displacementY -= 0.18f;
            displacementX += 0.12f;
        }
    }
}
