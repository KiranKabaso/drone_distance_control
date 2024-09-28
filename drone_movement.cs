using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drone_movement : MonoBehaviour
{
    public GameObject control;
    public Vector3 droneMovement;
    private Rigidbody rb;

    private float droneMaxSpeed;
    private float droneAcceleration;
    public float currentAcceleration;

    void Start()
    {
        control ControlScript = control.GetComponent<control>();
        droneMaxSpeed = ControlScript.droneMaxSpeed;    // Maximum speed
        droneAcceleration = ControlScript.droneMaxAcceleration;    // Maximum acceleration
        droneMovement = Vector3.zero;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Reset the movement vector each frame
        droneMovement = Vector3.zero;

        // Lift (Up/Down movement using space and shift)
        if (Input.GetKey(KeyCode.Space))
        {
            droneMovement += Vector3.up; // Upward movement
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            droneMovement += Vector3.down; // Downward movement
        }

        // Forward/backward movement (W/S keys)
        if (Input.GetKey(KeyCode.W))
        {
            droneMovement += Vector3.forward; // Forward
        }
        else if (Input.GetKey(KeyCode.S))
        {
            droneMovement += -Vector3.forward; // Backward
        }

        // Left/right movement (A/D keys)
        if (Input.GetKey(KeyCode.A))
        {
            droneMovement += -Vector3.right; // Left
        }
        else if (Input.GetKey(KeyCode.D))
        {
            droneMovement += Vector3.right; // Right
        }
        if(droneMovement.magnitude == 0){
            currentAcceleration = 0;
        }
        if (droneMovement.magnitude > 0)
        {
            droneMovement = droneMovement.normalized * droneAcceleration;
            currentAcceleration = droneAcceleration;
        }

        ApplyForce(droneMovement, ForceMode.Acceleration);
    }
    public void ApplyForce(Vector3 force, ForceMode mode)
    {
        rb.AddForce(force, mode);  // Apply force
        CapVelocity();  // Immediately cap velocity after force is applied
    }
    private void CapVelocity()
    {
        if (rb.velocity.magnitude > droneMaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * droneMaxSpeed;
        }
    }
}
