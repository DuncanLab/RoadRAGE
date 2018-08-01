using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class CarAIEngine : MonoBehaviour
{

    private const float MAX_SPEED = 20f;

    public Transform currPath;
    private List<Transform> nodes;
    private int currNodeIndex = 0;

    private const float maxSteerAngle = 40f;

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    private CarController m_Car;
    private Rigidbody m_RigidBody;

    public string currPathTag;

    void Start()
    {
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
        //ApplySteer();
        Drive();
        CheckWaypointDistance();
        //print(m_RigidBody.velocity.magnitude);
    }

    private void CheckUserInput()
    {
        // Check for left/right input, and change lanes accordingly. 
        // All other movement is restricted for simulation purposes
        float h = CrossPlatformInputManager.GetAxis("Horizontal");

        if (h > 0f)
        {
            print("MOVING RIGHT");
            if (currPathTag == "LeftPath")
            {
                currPathTag = "CenterPath";
            }
            else
            {
                if (Math.Abs(wheelFL.steerAngle) < 1)
                {
                currPathTag = "RightPath";

                }
            }
            nodes.Clear();
            currNodeIndex = 0;
        } else if (h < 0f)
        {
            print("MOVING LEFT");
            if (currPathTag == "RightPath")
            {
                currPathTag = "CenterPath";
            } else
            {
                if (Math.Abs(wheelFL.steerAngle) < 1)
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
                if (pathTransforms[j] != paths[i].transform && Vector3.Angle(transform.forward, pathTransforms[j].position - transform.position) < 90  && Vector3.Distance(pathTransforms[j].position, transform.position) > 50)
                {
                    nodes.Add(pathTransforms[j]);
                }
            }
        }
    }

    private void ApplySteer()
    {
        Vector3 relativeVec = transform.InverseTransformPoint(nodes[currNodeIndex].position);

        float newSteer = (relativeVec.x / relativeVec.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
       // m_Car.Move(newSteer, 0, 0, 0);
    }

    private void Drive()
    {
        // Speed up to MAX_SPEED.
        if (m_RigidBody.velocity.magnitude < MAX_SPEED)
        {
            m_Car.Move(0, 1f, 0, 0);
            //print("ACCEL NOW");
        } else
        {
            m_Car.Move(0, 0, 0, 0);
        }

        Vector3 relativeVec = transform.InverseTransformPoint(nodes[currNodeIndex].position);

        float newSteer = (relativeVec.x / relativeVec.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currNodeIndex].position) < 8f)
        {
            if (currNodeIndex == nodes.Count - 1)
            {
                currNodeIndex = 0;
            }
            else
            {
                currNodeIndex++;
            }
        }
    }
}