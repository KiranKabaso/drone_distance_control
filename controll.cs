using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control : MonoBehaviour
{
    public float wantedDistance = 5f;


    public float droneMaxSpeed = 8.0f;
    public float droneMaxAcceleration = 3.0f;
    private float elapsedTime;


    private Vector3 D2vector;
    public Vector3 D1vector;

    private Vector3 directionD1toD2;

    private Vector3 D1Location;
    private Vector3 D2Location;

    public Vector3 D1v0;  // Initial velocity for D1
    private Vector3 D2v0;  // Initial velocity for D2

    private float updateInterval = 1f; // Time in seconds between updates
    private float nextUpdateTime = 0f; // Time when the next update should occur
    private float D2Acceleration;

    public GameObject AnchorDrone; // Assign this in the Unity Inspector
    public GameObject TagDrone; // Assign this in the Unity Inspector
    private float droneDistance;
    private float receivedDistance;

    private float D1dragCoefficient;
    private float D2dragCoefficient;

    void Start()
    {

        D1dragCoefficient = TagDrone.GetComponent<Rigidbody>().drag;
        D2dragCoefficient = AnchorDrone.GetComponent<Rigidbody>().drag;
        receivedDistance = Vector3.Distance(AnchorDrone.transform.position, TagDrone.transform.position);
        droneDistance = receivedDistance;

        D2vector = AnchorDrone.GetComponent<drone_movement>().droneMovement;
        D2Acceleration = AnchorDrone.GetComponent<drone_movement>().currentAcceleration;
        D1vector = Vector3.zero;

        D1Location = TagDrone.transform.position;
        D2Location = AnchorDrone.transform.position;

        directionD1toD2 = D2Location - D1Location;

        D1v0 = Vector3.zero;
        D2v0 = Vector3.zero;

        nextUpdateTime = Time.time + updateInterval; // Set the time for the first update
    }
void FixedUpdate(){
    Debug.Log("distance: " + Vector3.Distance(AnchorDrone.transform.position, TagDrone.transform.position));
    if (Time.time >= nextUpdateTime)
        {
            float receivedDistance = Vector3.Distance(AnchorDrone.transform.position, TagDrone.transform.position);
            D1Location = ClosestPointOnSphere(D2Location, receivedDistance, D1Location);
            // Set the time for the next update
            nextUpdateTime = Time.time + updateInterval;
        }
    // drone no 2 acceleration
    D2vector = AnchorDrone.GetComponent<drone_movement>().droneMovement;
    // drone no 2 magnitude
    D2Acceleration = AnchorDrone.GetComponent<drone_movement>().currentAcceleration;

    float elapsedTime = Time.fixedDeltaTime;

    droneDistance = Vector3.Distance(D2Location, D1Location);

    D2Location = CalculateNewLocation1(D2Location, ref D2v0, D2vector.normalized*D2Acceleration, elapsedTime, droneMaxSpeed, D2dragCoefficient);

    D1vector = CalculateD1NextVector2(D2Location, D1Location, D2v0, D1v0, wantedDistance, droneDistance, droneMaxAcceleration);

    D1Location = CalculateNewLocation1(D1Location, ref D1v0, D1vector, elapsedTime, droneMaxSpeed, D1dragCoefficient);
}


    public Vector3 CalculateD1NextVector2(Vector3 D2location, Vector3 D1location, Vector3 D2v0, Vector3 D1v0, float wantedDistance, float currentDistance, float MaxAcceleration){
        Vector3 bestD1Vector;
        float distanceError = currentDistance - wantedDistance;
        Vector3 velocityError = D1v0 - D2v0;

        float deaccelerationDistance = calculateDeaccelerationDistance(D1v0, D2v0, MaxAcceleration);
        if(distanceError == 0 + 0.001f || distanceError == 0 - 0.001f){
            bestD1Vector = calculateDeacceleration(D1v0, D2v0, MaxAcceleration);
        }else if(distanceError > 0){
            if(deaccelerationDistance > distanceError){
                Vector3 deacceleration = calculateDeacceleration(D1v0, D2v0, MaxAcceleration);
                bestD1Vector = deacceleration;
            }else{
                bestD1Vector = (2*(D2location - D1location) - velocityError).normalized * MaxAcceleration;
            }
        }else{//distanceError < 0
            if(deaccelerationDistance > -distanceError){
                Vector3 deacceleration = calculateDeacceleration(D1v0, D2v0, MaxAcceleration);
                bestD1Vector = deacceleration;
            }else{
                bestD1Vector = ((2*(D1location - D2location) - velocityError)).normalized * MaxAcceleration;
            }
        }
        return bestD1Vector.normalized * MaxAcceleration;
    }

    public Vector3 calculateDeacceleration(Vector3 D1v0, Vector3 D2v0, float MaxAcceleration){
        return (D2v0 - D1v0).normalized * MaxAcceleration;
    }

    public float calculateDeaccelerationDistance(Vector3 D1v0, Vector3 D2v0, float maxAcceleration){
        float time = (D1v0 - D2v0).magnitude/maxAcceleration;
        float distance = (D1v0.magnitude + D2v0.magnitude) * time / 2;
        return distance;
    }

    public Vector3 ClosestPointOnSphere(Vector3 center, float radius, Vector3 point)
    {
        Vector3 direction = (point - center).normalized;
        return center + direction * radius;
    }

    public static Vector3 CalculateNewLocation1(Vector3 initialPosition, ref Vector3 initialVector, Vector3 acceleration, float time, float MaxSpeed, float dragCoefficient){

        initialVector *= 1f - (dragCoefficient * time);  // Reduce velocity due to drag

        Vector3 newLocation;
        if (acceleration.magnitude == 0) {
            newLocation = initialPosition + initialVector*time;
        } else {
            float T_max = calculateTimeToMaxSpeed(initialVector, acceleration, MaxSpeed);
            if (time < T_max) {
                newLocation = initialPosition + initialVector*time + 0.5f * acceleration * time * time;
                initialVector = initialVector + acceleration * time;
            } else {
                newLocation = initialPosition + initialVector*T_max + 0.5f * acceleration * T_max * T_max;
                initialVector = (initialVector + acceleration * time).normalized * MaxSpeed; // check later
                newLocation += initialVector * (time - T_max);
            }
        }
        return newLocation;
    }

    public static float calculateTimeToMaxSpeed(Vector3 v0, Vector3 a, float MaxSpeed){

        float maxTime;
        float b = 2*(v0.x * a.x + v0.y * a.y + v0.z * a.z);
        maxTime = (-b + Mathf.Sqrt( b*b - 4*a.magnitude*a.magnitude*(v0.magnitude*v0.magnitude - MaxSpeed*MaxSpeed) ))/(2*a.magnitude*a.magnitude);
        return maxTime;
    }
}