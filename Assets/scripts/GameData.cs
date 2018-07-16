using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData {


    public int OutputTimesPerSecond;

    public GlobalGameData GlobalData;

    public List<int> BlockOrder; // The order of the trial blocks.

    public List<BlockData> BlockList; // List of all blocks.
    public List<int> currTrialOrder; // The order of trials within the current block.

    [Serializable]
    public class GlobalGameData
    {
        public float MovementSpeed; // Speed in meters/second.
    }

    [Serializable]
    public class BlockData
    {
        public int blockId;
        public string BlockName;
        public string Notes;
        public List<int> TrialOrder; // Ordering of trials in the particular block.
    }

    [Serializable]
    public class TrialData
    {
        public int trialId;
        public string Header; // <---- general instruction (should be in middle of HUD) (OM).
        public List<string> Roads; // <---- what prefabs are used. first stretch that is generated is road1 prefab and then road2 prefab these names correspond with prefab names in UNITY
        public string FileLocation; // Path to image file used for heads up display.
        public float TimeAlloted; // Amount of time in seconds that the trial will continue for. <--- when this is equal to -1 it means they need to press "SPACE" (OM). <---- When this is any positive number that means it remains on the screen for that long(OM)
        public List<string> Events; // <--- time in miliseconds @ 1 second sign1 (refers to sign data below) appears on the top right of HUD 1.5 seconds the right lane comes up.
        public List<string> End; //  <--- depending on what lane they end in, they recieve point value associated with lane.
    }

    [Serializable]
    public class SignData
    {
        public int signId;
        public string FileLocation;
        public string SoundLocation;
        public int Length;
        public int Height;
    }

}
