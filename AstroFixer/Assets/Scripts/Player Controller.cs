using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _input;
    private Vector3 _inputMoveDirection;
    private float _currentRotationVelocity;
    protected const float rotationSmoothTime = 0.1f;

    private Vector3 _velocity;
    private Vector3 _acceleration;
    private Vector3 _momentum;
    private bool _isInside = true;

    [SerializeField] private float movementSpeedGround = 8;
    [SerializeField] private float movementForceSpace = 5;

    [SerializeField] private Sprite runningSprite;
    [SerializeField] private Sprite standingSprite;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        updatePlayerSprites();
        CalculateForces();

        ApplyMovement();
    }

    private void ApplyMovement()
    {
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void updatePlayerSprites()
    {
 
        if (_input.sqrMagnitude == 0)
        {
            _spriteRenderer.sprite = standingSprite;
        }
        else
        {
            _spriteRenderer.sprite = runningSprite;
        }

        if (_inputMoveDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_inputMoveDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    private void CalculateForces()
    {
        // Calculate velocity from player input
        Vector3 inputMovementVelocity = Vector3.zero;

        if (_isInside) {
            _acceleration = Vector3.zero;
            inputMovementVelocity = _inputMoveDirection * movementSpeedGround;
            _momentum = inputMovementVelocity / Time.deltaTime;
        }
        else {
            _acceleration += _inputMoveDirection * movementForceSpace + _momentum;
            _momentum = Vector3.zero;
        }
        
        _velocity = _acceleration * Time.deltaTime + inputMovementVelocity;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _inputMoveDirection = new Vector3(_input.x, 0, _input.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "InsideZone") {
            _isInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "InsideZone") {
            _isInside = false;
        }
    }
}
