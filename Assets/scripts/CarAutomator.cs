using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using GD = GameController;

public class CarAutomator : MonoBehaviour
{

    // Car stuffs
    private Rigidbody m_Rigidbody;

    // Track whether we're in a lane or not
    public bool inLane;
    public bool movingRight;
    public bool movingLeft;

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = new Vector3(0, 0, GD.data.GlobalData.MovementSpeed);
    }

    // Called every frame
    private void Update()
    {
        // Maintain constant forward velocity
        var previous = m_Rigidbody.velocity;
        m_Rigidbody.velocity = new Vector3(previous.x, previous.y, GD.data.GlobalData.MovementSpeed);

        // Check for left/right input, and change lanes accordingly. 
        // All other movement is restricted for simulation purposes
        float h = CrossPlatformInputManager.GetAxis("Horizontal");

        // Turn right...GLIDE right
        if (h > 0f || movingRight)
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
        else if (h < 0f || movingLeft)
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

        if (currPos.x == 252.3f || currPos.x == 256.5f || currPos.x == 247.6f || currPos.x == 243.7f)
        {
            movingLeft = false;
            movingRight = false;
            return true;
        }

        return false;
    }
}
