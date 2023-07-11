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

        isWalking = direction != Vector3.zero;
        if (isWalking)
        {
            Console.WriteLine("Walking");
        }
        transform.position += moveSpeed * Time.deltaTime * direction;
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
