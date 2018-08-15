using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class CarAIEngine : MonoBehaviour
{
    private GameData data;

    public Transform currPath;
    private List<Transform> nodes;
    private int currNodeIndex = 0;

    private const float maxSteerAngle = 40f;

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    private CarController m_Car;
    private Rigidbody m_RigidBody;

    public PID pid;
    public string currPathTag;

    void Start()
    {
        // Carry over data.
        data = Toolbox.Instance.data;

        // get the car controller
        m_Car = GetComponent<CarController>();
        // get the rigidbody for speed
        m_RigidBody = GetComponent<Rigidbody>();
        nodes = new List<Transform>();

        currPathTag = "CenterPath";
    }

    void FixedUpdate()
    {
        CheckUserInput();
        PopulateNodes();
        Drive();
        CheckWaypointDistance();
    }

    private void CheckUserInput()
    {
        // Need a check here to see if there on an off ramp then do nothing
        // when input arrives.


        // Check for left/right input, and change lanes accordingly. 
        // All other movement is restricted for simulation purposes
        float h = CrossPlatformInputManager.GetAxis("Horizontal");

        // Move right one lane.
        if (h > 0f)
        {
            if (currPathTag == "LeftPath")
            {
                currPathTag = "CenterPath";
            }
            else
            {
                // Only change one lane at a time.
                if (Math.Abs(wheelFL.steerAngle) < 0.001f)
                {
                    currPathTag = "RightPath";

                }
            }
            nodes.Clear();
            currNodeIndex = 0;
        }
        // Move left one lane.
        else if (h < 0f)
        {
            if (currPathTag == "RightPath")
            {
                currPathTag = "CenterPath";
            }
            else
            {
                if (Math.Abs(wheelFL.steerAngle) < 0.001f)
                {
                    currPathTag = "LeftPath";

                }
            }
            nodes.Clear();
            currNodeIndex = 0;

        }
    }

    private void PopulateNodes()
    {
        GameObject[] paths = GameObject.FindGameObjectsWithTag(currPathTag);

        for (int i = 0; i < paths.Length; i++)
        {
            Transform[] pathTransforms = paths[i].GetComponentsInChildren<Transform>();

            // Remove self from transform array.
            for (int j = 0; j < pathTransforms.Length; j++)
            {
                // Only add nodes to path that are infront of the car, and sufficiently far away. Otherwise, the car steers "too" well.
                // The filter for distance may be removed if paths with far less nodes are created.
                if (pathTransforms[j] != paths[i].transform && Vector3.Angle(transform.forward, pathTransforms[j].position - transform.position) < 90 && Vector3.Distance(pathTransforms[j].position, transform.position) > 40)
                {
                    if (!nodes.Contains(pathTransforms[j]))
                    {
                        nodes.Add(pathTransforms[j]);
                    }
                }
            }
        }
    }

    private void ApplySteer()
    {
        Vector3 relativeVec = transform.InverseTransformPoint(nodes[currNodeIndex].position);
        float newSteer = (relativeVec.x / relativeVec.magnitude) * maxSteerAngle;

        float correctionViaPID = pid.Update(newSteer, wheelFL.steerAngle, Time.deltaTime);

        wheelFL.steerAngle = correctionViaPID;
        wheelFR.steerAngle = correctionViaPID;
    }

    private void Drive()
    {
        // Speed up to MAX_SPEED.
        if (m_RigidBody.velocity.magnitude < data.GlobalData.MovementSpeed)
        {
            m_Car.Move(0, 1f, 0, 0);
        }
        else
        {
            m_Car.Move(0, 0, 0, 0);
        }

        ApplySteer();
    }

    private void CheckWaypointDistance()
    {
        // Advance the current node if we've passed the previous one.
        if (Vector3.Distance(transform.position, nodes[currNodeIndex].position) < 10f || Vector3.Angle(transform.forward, nodes[currNodeIndex].position - transform.position) > 90)
        {
            currNodeIndex++;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw current node in scene view (Used for debugging).
        Gizmos.DrawSphere(nodes[currNodeIndex].position, 6f);

        for (int i = 0; i < nodes.Count; i++)
        {
            Gizmos.DrawWireSphere(nodes[i].position, 4f);
        }
    }
}