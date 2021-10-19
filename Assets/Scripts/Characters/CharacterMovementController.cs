using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    public float MaxSpeed = 1;

    void Start()
    {
        // check if stuff is missing
        if (!gameObject.GetComponent<CharacterController>()) throw new Exception("Missing CharacterController!");
        if (!gameObject.GetComponent<Rigidbody>()) throw new Exception("A moving character needs a Rigidbody");
    }

    /// <summary>
    /// Move transform position by given direction vector and speed
    /// </summary>
    /// <param name="vector">direction vector</param>
    /// <param name="speed">the speed to move at</param>
    public void Move(Vector3 vector, float speed)
    {
        if (speed > MaxSpeed) speed = MaxSpeed;
        if (speed < -MaxSpeed) speed = -MaxSpeed;

        // cant fly lol
        vector.y = 0;
        
        transform.position += vector.normalized * speed * Time.deltaTime;
    }

    /// <summary>
    /// Move transform position by given direction vector and speed
    /// </summary>
    /// <param name="vector">direction vector</param>
    /// <param name="speed">the speed to move at</param>
    public void Move(Vector2 vector, float speed)
        => this.Move(new Vector3(vector.x, 0, vector.y), speed);
    
    //TODO Add relative movement
}
