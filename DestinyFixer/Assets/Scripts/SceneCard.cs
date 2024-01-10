using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CardType
{
    OpenDoors,
    Repair,
    Speed
}

public class SceneCard : MonoBehaviour
{
    public CardType Type;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out var _))
        {
            switch (Type)
            {
                case CardType.OpenDoors:
                    if (!CardsHolder.Instance.OpenDoorCardInHand)
                    {
                        CardsHolder.Instance.AddOpenDoorsCard();
                        Destroy(gameObject);
                    }
                    break;

                case CardType.Repair:
                    if (!CardsHolder.Instance.RepairCardInHand)
                    {
                        CardsHolder.Instance.AddRepairCard();
                        Destroy(gameObject);
                    }
                    break;

                case CardType.Speed:
                    if (!CardsHolder.Instance.SpeedCardInHand)
                    {
                        CardsHolder.Instance.AddSpeedCard();
                        Destroy(gameObject);
                    }
                    break;
            }
        }
    }
}
