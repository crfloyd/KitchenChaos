using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;

    private bool isWalking;

    private void Update()
    {
        Vector2 inputVector = new(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y += 1;

        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y -= 1;

        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x -= 1;

        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x += 1;

        }

        inputVector.Normalize();

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
