using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag : MonoBehaviour
{
    private Rigidbody rb;

    public GameObject control;

    private Vector3 DroneVector;
    private float droneMaxSpeed;
    private float droneAcceleration;
    void Start()
    {
        if (control == null){
            Debug.LogError("DroneMovement script not found on target drone!");
        }
        rb = GetComponent<Rigidbody>();

        control ControlScript = control.GetComponent<control>();
        droneMaxSpeed = ControlScript.droneMaxSpeed;    // Maximum speed
        droneAcceleration = ControlScript.droneMaxAcceleration; // Maximum acceleration
        DroneVector = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (control == null) {
            Debug.LogError("DroneMovement script not found on target drone!");
        };  // Exit if control is null

        control ControlScript = control.GetComponent<control>();
        // Get updated movement direction
        DroneVector = ControlScript.D1vector;

        if(DroneVector.magnitude > 0){
            ApplyForce(DroneVector, ForceMode.Acceleration);
        }

        float speed = GetComponent<Rigidbody>().velocity.magnitude;  // test current object speed
        if (speed > droneMaxSpeed)
        {
            float brakeSpeed = speed - droneMaxSpeed;  // calculate the speed decrease
            Vector3 normalisedVelocity = GetComponent<Rigidbody>().velocity.normalized;
            Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;  // make the brake Vector3 value
            GetComponent<Rigidbody>().AddForce( -brakeVelocity );  // apply opposing brake force
        }
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
            rb.velocity = rb.velocity.normalized * droneMaxSpeed;  // Cap velocity to the max value
        }
    }
}
