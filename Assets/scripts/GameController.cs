﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class GameController : MonoBehaviour
{

    public StartController startController;
    public GameData data;

    // Use stopwatch for it's accuracy.
    public System.Diagnostics.Stopwatch currTrialTimer;

    public int trialOrderIndex;
    public int blockOrderIndex;

    // Game objects
    public GameObject LoadImagePanel;

    // Use this for initialization
    void Awake()
    {
        Debug.Log("Awake Called in game controller!!");
        currTrialTimer = new System.Diagnostics.Stopwatch();

        // Carry over data.
        data = Toolbox.Instance.data;

        // Init block, trial positions on the first go around
        if (!data.isGameStarted)
        {
            trialOrderIndex = 0;
            blockOrderIndex = 0;

            data.currBlock = data.BlockList[data.BlockOrder[blockOrderIndex] - 1];
            data.currTrial = data.TrialList[data.currBlock.TrialOrder[trialOrderIndex] - 1];

            data.isGameStarted = true;
            LoadNextTrial();
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

            // Load next block (if it exists)
            if (trialOrderIndex + 1 == data.currBlock.TrialOrder.Count)
            {
                // On the last block so end the simulation
                if (blockOrderIndex + 1 == data.BlockOrder.Count)
                {
                    Debug.LogWarning("Simulation End!");
                    Application.Quit();
                    UnityEditor.EditorApplication.isPlaying = false;
                    return;
                }
                // blocks are remaining, so continue.
                else
                {
                    trialOrderIndex = -1;
                    blockOrderIndex++;
                    data.currBlock = data.BlockList[data.BlockOrder[blockOrderIndex] - 1];
                }
            }

            trialOrderIndex++;
            LoadNextTrial();
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

        // This should check if the current trial's allotted time has passed.
        else if (currTrialTimer.ElapsedMilliseconds >= Math.Abs(data.currTrial.TimeAllotted))
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
        data.currTrial = data.TrialList[data.currBlock.TrialOrder[trialOrderIndex] - 1];

        // If the file location variable has been set in the config
        // We want to display an image instead of starting an actual trial.
        if (data.currTrial.FileLocation != null)
        {
            var filedata = File.ReadAllBytes(Application.dataPath + "/config/images/" + data.currTrial.FileLocation);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(filedata);

            RawImage loadImageObject = (RawImage)LoadImagePanel.GetComponent("RawImage");
            loadImageObject.texture = tex;
            loadImageObject.enabled = true;
        }

        // Trial resets and starts from scratch.
        // else
        // {
        // Reset Scene
        //   SceneManager.LoadScene("Main");

        // Load the appropriate prefabs.
        //}        

        currTrialTimer.Stop();
        currTrialTimer.Reset();
        currTrialTimer.Start();
    }

}
