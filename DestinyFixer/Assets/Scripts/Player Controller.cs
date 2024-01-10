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

    bool sprinting = false;

    private Vector3 _velocity;
    private Vector3 _acceleration;
    private Vector3 _momentum;

    [SerializeField] public float movementSpeedGround = 8;
    [SerializeField] private float movementForceSpace = 5;

    [SerializeField] private Sprite runningSprite;
    [SerializeField] private Sprite standingSprite;
    [SerializeField] private Sprite floatingInSpaceSprite;

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
        if (ZoneTracker.Instance.IsInZone())
        {
            if (_input.sqrMagnitude == 0)
            {
                _spriteRenderer.sprite = standingSprite;
            }
            else
            {
                _spriteRenderer.sprite = runningSprite;
            }
        }
        else
        {
            _spriteRenderer.sprite = floatingInSpaceSprite;
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

        if (ZoneTracker.Instance.IsInZone()) {
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

    private void ToggleSprint()
    {
        sprinting = !sprinting;
        if(sprinting)
        {
            movementSpeedGround += 5;
            movementForceSpace += 4;
        }
        else
        {
            movementSpeedGround -= 5;
            movementForceSpace -= 4;
        }
    }

    public void StartSprinting()
    {
        sprinting = true;
        movementSpeedGround += 5;
        movementForceSpace += 4;
    }

    public void StopSprinting()
    {
        movementSpeedGround -= 5;
        movementForceSpace -= 4;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if(context.started || context.canceled)
        {
            ToggleSprint();
        }
    }
}
