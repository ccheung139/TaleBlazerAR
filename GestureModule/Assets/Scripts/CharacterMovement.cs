using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    private float movementSpeed = .02f;

    void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
#if UNITY_EDITOR

        KeyBoardMovement ();

#else
#endif

    }

    private void KeyBoardMovement()
    {
        Rotate();
        Movement();
    }

    private void Movement()
    {
        float speed = 3.0f;

        Vector3 pos = transform.position;
        if (Input.GetKey("w"))
        {
            pos += transform.forward * movementSpeed;
        }
        if (Input.GetKey("s"))
        {
            pos += transform.forward * -movementSpeed;
        }
        if (Input.GetKey("d"))
        {
            pos += transform.right * movementSpeed;
        }
        if (Input.GetKey("a"))
        {
            pos += transform.right * -movementSpeed;
        }
        if (Input.GetKey("e"))
        {
            pos += transform.up * movementSpeed;
        }
        if (Input.GetKey("q"))
        {
            pos += transform.up * -movementSpeed;
        }
        transform.position = pos;
    }

    private void Rotate()
    {
        float rotateSpeed = 60.0f;

        if (Input.GetKey("up"))
        {
            transform.Rotate(-Vector3.right, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey("down"))
        {
            transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey("right"))
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey("left"))
        {
            transform.Rotate(-Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}