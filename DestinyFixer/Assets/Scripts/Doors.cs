using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private Animator animator;

    private bool playerNearby = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerNearby && CardsHolder.Instance.OpenDoorCardInHand)
        {
            CardsHolder.Instance.DiscardOpenDoorsCard();
            animator.SetBool("character_nearby", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out var _))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out var _))
        {
            playerNearby = false;
        }
    }
}
