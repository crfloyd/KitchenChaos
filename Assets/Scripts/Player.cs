using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There is more than one Player in the scene!");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteract += GameInput_OnInteract;
    }

    private void GameInput_OnInteract(object sender, EventArgs e)
    {
        if(selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        var inputVector = gameInput.GetMovementVectorNormalized();
        var direction = new Vector3(inputVector.x, 0f, inputVector.y);
        if(direction != Vector3.zero)
        {
            lastInteractDir = direction;
        }

        var interactDistance = 2f;

        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if(clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            } 
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }

        Debug.Log(selectedCounter);
    }

    private void HandleMovement()
    {
        var inputVector = gameInput.GetMovementVectorNormalized();

        var direction = new Vector3(inputVector.x, 0f, inputVector.y);

        var moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        var playerHeight = 2f;
        var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, direction, moveDistance);

        if (!canMove)
        {
            // Attempt only X movement
            var xDirection = new Vector3(direction.x, 0f, 0f).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, xDirection, moveDistance);

            if (canMove)
            {
                direction = xDirection;
            }
            else
            {
                // Attempt only Z movement
                var zDirection = new Vector3(0f, 0f, direction.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, zDirection, moveDistance);
                if (canMove)
                {
                    direction = zDirection;
                }
                else
                {
                    // Cannot move in any direction

                }
            }

        }

        if (canMove)
        {
            transform.position += moveDistance * direction;
        }
        isWalking = direction != Vector3.zero;
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
