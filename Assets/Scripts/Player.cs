using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

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
        gameInput.OnInteractAlternate += GameInput_OnInteractAlternate;
    }

    private void GameInput_OnInteractAlternate(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteract(object sender, EventArgs e)
    {
        if(selectedCounter != null)
        {
            selectedCounter.Interact(this);
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
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
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

        //Debug.Log(selectedCounter);
    }

    private void HandleMovement()
    {
        var inputVector = gameInput.GetMovementVectorNormalized();

        var moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        var moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        var playerHeight = 2f;
        var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Attempt only X movement
            var xDirection = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, xDirection, moveDistance);

            if (canMove)
            {
                moveDir = xDirection;
            }
            else
            {
                // Attempt only Z movement
                var zDirection = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, zDirection, moveDistance);
                if (canMove)
                {
                    moveDir = zDirection;
                }
                else
                {
                    // Cannot move in any direction

                }
            }

        }

        if (canMove)
        {
            transform.position += moveDistance * moveDir;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public Transform GetKitchenObjectFollowTransform() => KitchenObjectHoldPoint;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject() => kitchenObject;

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject() => kitchenObject != null;
}
