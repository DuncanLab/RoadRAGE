using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class CarAutomator : MonoBehaviour
{

    // Car stuffs
    private Rigidbody m_Rigidbody;

    // Track whether we're in a lane or not
    public bool inLane;
    public bool movingRight;
    public bool movingLeft;

    public GameController GC;

    // Use this for initialization
    void Start()
    {
        GC = (GameController)GameObject.Find("GameScriptHolder").GetComponent("GameController");

        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = new Vector3(0, 0, GC.data.GlobalData.MovementSpeed);
    }

    // Called every frame
    private void Update()
    {
        // Maintain constant forward velocity
        var previous = m_Rigidbody.velocity;
        m_Rigidbody.velocity = new Vector3(previous.x, previous.y, GC.data.GlobalData.MovementSpeed);

        // Check for left/right input, and change lanes accordingly. 
        // All other movement is restricted for simulation purposes
        float h = CrossPlatformInputManager.GetAxis("Horizontal");

        // Turn right...GLIDE right
        if (RightLaneExists() && h > 0f || movingRight)
        {
            Debug.Log("right lane move");
            var previousPos = transform.position;

            if (previousPos.x < 256.5f)
            {
                if (inLane && h > 0f || !inLane)
                {
                    movingRight = true;
                    movingLeft = false;
                    transform.position = new Vector3(previousPos.x + 0.04f, previousPos.y, previousPos.z);
                }
                else
                {
                    movingRight = false;
                }

            }

        }

        // Turn left...GLIDE left
        else if (LeftLaneExists() && h < 0f || movingLeft)
        {
            Debug.Log("left lane move");
            var previousPos = transform.position;

            if (previousPos.x > 243.7f)
            {
                if (inLane && h < 0f || !inLane)
                {
                    movingLeft = true;
                    movingRight = false;
                    transform.position = new Vector3(previousPos.x - 0.04f, previousPos.y, previousPos.z);
                }
                else
                {
                    movingLeft = false;
                }
            }
        }

        inLane = CheckInLane();
    }

    // Determine if the vehicle is in one of the existing lanes.
    private bool CheckInLane()
    {
        var currPos = transform.position;
        currPos = new Vector3(float.Parse(Math.Round(currPos.x, 1).ToString()), float.Parse(Math.Round(currPos.y, 1).ToString()), float.Parse(Math.Round(currPos.z, 1).ToString()));

        if (currPos.x == 250f || currPos.x == 254.2f || currPos.x == 245.8f)
        {
            movingLeft = false;
            movingRight = false;
            return true;
        }

        return false;
    }

    private bool RightLaneExists()
    {
        bool LaneExists = false;
        GameObject[] LaneObjects;

        try
        {
            LaneObjects = GameObject.FindGameObjectsWithTag("LaneRight_100m");
        }
        catch (Exception e)
        {
            return LaneExists;
        }

        for (int i = 0; i < LaneObjects.Length; i++)
        {
            if (LaneObjects[i].transform.position.z - 50 < transform.position.z && transform.position.z < LaneObjects[i].transform.position.z + 50)
            {
                LaneExists = true;
                break;
            }
        }

        return LaneExists;
    }

    private bool LeftLaneExists()
    {
        bool LaneExists = false;
        GameObject[] LaneObjects;

        try
        {
            LaneObjects = GameObject.FindGameObjectsWithTag("LaneLeft_100m");
        }
        catch (Exception e)
        {
            return LaneExists;
        }

        if (LaneObjects == null) return LaneExists;

        for (int i = 0; i < LaneObjects.Length; i++)
        {
            if (LaneObjects[i].transform.position.z - 50 < transform.position.z && transform.position.z < LaneObjects[i].transform.position.z + 50)
            {
                LaneExists = true;
                break;
            }
        }

        return LaneExists;
    }
}
