using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameInput gameInput;

    private bool isWalking;

    private void Update()
    {
        var inputVector = gameInput.GetMovementVectorNormalized();

        var direction = new Vector3(inputVector.x, 0f, inputVector.y);

        var moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        var playerHeight = 2f;
        var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, direction, moveDistance);

        if(!canMove)
        {
            // Attempt only X movement
            var xDirection = new Vector3(direction.x, 0f, 0f).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, xDirection, moveDistance);

            if(canMove)
            {
                direction = xDirection;
            } else
            {
                // Attempt only Z movement
                var zDirection = new Vector3(0f, 0f, direction.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, zDirection, moveDistance);
                if(canMove)
                {
                    direction = zDirection;
                } else
                {
                    // Cannot move in any direction

                }
            }

        }

        if(canMove)
        {
            transform.position += moveDistance * direction;
        }
        isWalking = direction != Vector3.zero;
        if(direction != Vector3.zero)
        {
            transform.forward = direction;
        }

    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
