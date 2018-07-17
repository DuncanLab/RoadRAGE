using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the master class which contains all
// data pertain to the simulation that must be 
// kept "globally"
[Serializable]
public class GameData
{
    public int OutputTimesPerSecond;

    public bool isGameStarted;

    public Trial currTrial;
    public Block currBlock;

    public GlobalGameData GlobalData;

    public List<int> BlockOrder; // The order of the trial blocks.

    public List<Block> BlockList; // All blocks.
    public List<Trial> TrialList; // All trials.
    public List<int> currTrialOrder; // The order of trials within the current block.

    [Serializable]
    public class GlobalGameData
    {
        public float MovementSpeed; // Speed in meters/second.
    }

    [Serializable]
    public class Block
    {
        public int BlockId;
        public string BlockName;
        public string Notes;
        public List<int> TrialOrder; // Ordering of trials in the particular block.
    }

    [Serializable]
    public class Trial
    {
        public int TrialId;
        public string TrialName;
        public string Header; // <---- general instruction (should be in middle of HUD) (OM).
        public List<Road> Roads; // <---- what prefabs are used. first stretch that is generated is road1 prefab and then road2 prefab these names correspond with prefab names in UNITY
        public string FileLocation; // Path to image file used for heads up display.
        public float TimeAllotted; // Amount of time in seconds that the trial will continue for. <--- when this is equal to -1 it means they need to press "SPACE" (OM). <---- When this is any positive number that means it remains on the screen for that long(OM)
        public List<Event> Events; // List of all spawn events for the trial.
        public List<ScoreAssignment> FinalScoreAssignments; //  <--- depending on what lane they end in, they recieve point value associated with lane.
    }

    [Serializable]
    public class Event
    {
        public float SpawnTime; // Time in ms that the event will spawn (measured from trial start).
        public string EventType; // The type of spawn event.
        public int SignId; // The id of the sign to be used (if == sign event).
        public string SpawnSide; // Side of the road/screen for the spawn event to appear.
    }

    [Serializable]
    public class Road
    {
        public string PrefabName; // The name of the road prefab to use (must exist as a prefab in the assets/prefab directory).
        public float TimeToExist; // The length of time (in ms) that the prefab will exist for.
    }

    [Serializable]
    public class ScoreAssignment
    {
        public string EndingLane; // The lane that the user finished the trial in
        public int PointsAwarded; // Number of points awarded for choosing the given lane.
    }

    [Serializable]
    public class Sign
    {
        public int SignId;
        public string FileLocation;
        public string SoundLocation;
        public int Length;
        public int Height;
    }

}
