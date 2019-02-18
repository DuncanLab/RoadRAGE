using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class GameController : MonoBehaviour
{
    public GameData data;

    // Game objects
    public GameObject LoadImagePanel;

    // Use this for initialization
    void Start()
    {
        // Carry over data.
        data = Toolbox.Instance.data;

        CheckResourceState();

        Debug.Log("Start Called in game controller!!");

        // Init block, trial positions on the first go around
        if (!data.isGameStarted)
        {
            data.currBlock.trialOrderIndex = 0;
            data.blockOrderIndex = 0;

            Debug.Log("Loading block: " + data.BlockOrder[data.blockOrderIndex]);
            data.currBlock = data.BlockList[data.BlockOrder[data.blockOrderIndex] - 1];

            LoadNextTrial();
            data.isGameStarted = true;
        }
    }

    private void CheckResourceState()
    {
        if (data.currTrial.TrackResources || data.currTrial.TrackPoints)
        {
            GameObject car = GameObject.Find("Car");
            car.AddComponent(typeof(PointsController));
        }

        if (!data.currTrial.TrackResources && SceneManager.GetActiveScene().name.Equals("Main"))
        {
            GameObject hungerBar = GameObject.Find("HungerBar").gameObject;
            GameObject thirstBar = GameObject.Find("ThirstBar").gameObject;
            hungerBar.SetActive(false);
            thirstBar.SetActive(false);
        }

        if (!data.currTrial.TrackPoints && SceneManager.GetActiveScene().name.Equals("Main"))
        {
            GameObject pointsCounter = GameObject.Find("PointsCounter").gameObject;
            pointsCounter.SetActive(false);
        }
    }

    // Update is called once per frame
    // Used for the main game loop: moving through
    // blocks and trials
    void Update()
    {
        bool spacePressed = CrossPlatformInputManager.GetButtonDown("Jump");

        // The current trial has ended so take action.
        if (IsTrialOver(spacePressed))
        {
            UnloadCurrentTrial();
            data.currBlock.trialOrderIndex++;

            // Load next block (if it exists)
            if (data.currBlock.trialOrderIndex >= data.currBlock.TrialOrder.Count)
            {
                data.blockOrderIndex++;
                // On the last block so end the simulation
                if (data.blockOrderIndex >= data.BlockOrder.Count)
                {
                    Debug.LogWarning("Simulation End! (All blocks have been completed)");
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    return;
                }
                // blocks are remaining, so continue.
                else
                {
                    Debug.Log("Loading block: " + data.BlockOrder[data.blockOrderIndex]);
                    data.currBlock = data.BlockList[data.BlockOrder[data.blockOrderIndex] - 1];
                    LoadNextTrial();
                }
            }
            else
            {
                LoadNextTrial();
            }
        }
    }

    private bool IsTrialOver(bool spacePressed)
    {
        bool trialOver = false;

        // It's a menu screen so load next trial when space is pressed
        if (data.currTrial.TimeAllotted == -1)
        {
            if (spacePressed)
            {
                trialOver = true;
            }
        }

        // This should check if the current trial's allotted time has passed, or if 
        // the user is out of resources
        else if (data.currTrial.Timer.ElapsedMilliseconds >= Math.Abs(data.currTrial.TimeAllotted) ||
            (!data.currTrial.resourcesRemain && data.currTrial.TrackResources))
        {
            trialOver = true;
        }

        return trialOver;
    }


    private void UnloadCurrentTrial()
    {
        RawImage loadImageObject = (RawImage)LoadImagePanel.GetComponent("RawImage");
        loadImageObject.enabled = false;
    }

    private void LoadNextTrial()
    {
        Debug.Log("Loading trial: " + data.currBlock.TrialOrder[data.currBlock.trialOrderIndex]);
        data.currTrial = data.TrialList[data.currBlock.TrialOrder[data.currBlock.trialOrderIndex] - 1];
        data.currTrial.Timer = new System.Diagnostics.Stopwatch();

        // If the file location variable has been set in the config
        // We want to display an image instead of starting an actual trial.
        if (data.currTrial.FileLocation != null)
        {
            if (!SceneManager.GetActiveScene().name.Equals("Instructions"))
            {
                SceneManager.LoadScene("Instructions");
            }

            var filedata = File.ReadAllBytes(Application.dataPath + "/StreamingAssets/config/images/" + data.currTrial.FileLocation);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(filedata);

            RawImage loadImageObject = (RawImage)LoadImagePanel.GetComponent("RawImage");
            loadImageObject.texture = tex;
            loadImageObject.enabled = true;
        }

        //Trial resets and starts from scratch.
        else if (data.isGameStarted)
        {
            //Reset Scene
            SceneManager.LoadScene("Main");
            //Load the appropriate prefabs.
        }

        data.currTrial.Timer.Stop();
        data.currTrial.Timer.Reset();
        data.currTrial.Timer.Start();
    }
}
