using System;
using System.Collections.Generic;
using UnityEngine;

// This class controls the basic "infinite scroll" effect that is 
// is used for the simulation.
public class TerrainGenerator : MonoBehaviour
{

    const float RIGHT_LANE_POS_X = 254.2f;
    const float RIGHT_LANE_POS_Y = 0.006f;
    const float RIGHT_LANE_POS_Z = 50f;

    const float LEFT_LANE_POS_X = 245.8f;
    const float LEFT_LANE_POS_Y = 0.006f;
    const float LEFT_LANE_POS_Z = 50f;

    const float BILLBOARD_RIGHT_POS_X = 261.56f;
    const float BILLBOARD_RIGHT_POS_Y = 0.0f;
    const float BILLBOARD_RIGHT_POS_Z = 218.87f;


    // This is the standard (prefab) chunk we want to load to simulate
    // an infinite roadway.
    public GameObject road_normal_100m;

    // Prefabs for loading
    public GameObject billboard;
    public GameObject LaneRight_100m;
    public GameObject LaneLeft_100m;

    // Reference pointers
    public GameObject currTerrain;
    public GameObject currTerrainChunk;
    public GameObject prevTerrainChunk;

    public int CurrentRoadPrefab;

    public List<GameObject> _createdGameObjects;

    public GameData data;

    public enum LaneSide { left, right };

    // Use this for initialization.
    void Start()
    {
        // Carry over data.
        data = Toolbox.Instance.data;

        // Simulation starts with only one chunk.
        GameObject RoadPrefab = Resources.Load<GameObject>("prefabs/" + data.currTrial.Roads[0].PrefabName);
        currTerrainChunk = Instantiate(RoadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //currTerrainChunk = GameObject.Find("road1Example");
        prevTerrainChunk = currTerrainChunk;
        _createdGameObjects.Add(prevTerrainChunk);
        CurrentRoadPrefab = 0;

        // pre-load all the lanes here
        LoadLanes();


    }

    // Update is called once per frame.
    void Update()
    {
        var relativePos = transform.position.z - currTerrainChunk.transform.position.z;

        // We have left the previous terrain chunk, never to return - so destroy all of it.
        // TODO : figure out a way to make this work with the new, config driven chunk spawn.
        if (relativePos > 105)
        {
            DestroyPreviousChunk();
            prevTerrainChunk = currTerrainChunk;
        }

        // Generate the required chunks in the current prefab spec.
        if (CurrentRoadPrefab < data.currTrial.Roads.Count)
        {
            GameObject RoadPrefab = (GameObject)Resources.Load("prefabs/" + data.currTrial.Roads[0].PrefabName);

            // Here we want to instantiate enough chunks to last the time specified in the config
            var ChunksRequired = CalculateNumberOfChunksRequired(data.currTrial.Roads[0].TimeToExist, data.GlobalData.MovementSpeed, 100);
            for (int i = 0; i < ChunksRequired; i++)
            {
                currTerrainChunk = Instantiate(RoadPrefab, new Vector3(0, 0, currTerrainChunk.transform.position.z + 100f), Quaternion.identity);
                _createdGameObjects.Add(currTerrainChunk);
            }

            CurrentRoadPrefab++;
        }
    }

    private void DestroyPreviousChunk()
    {
        for (int i = 0; i < _createdGameObjects.Count; i++)
        {
            Destroy(_createdGameObjects[i]);
        }
    }


    // TODO : refactor.
    private void LoadLanes()
    {
        for (int i = 0; i < data.currTrial.Events.Count; i++)
        {
            GameData.Event CurrEvent = data.currTrial.Events[i];
            if (CurrEvent.EventType.ToLower().Equals("lane"))
            {
                if (CurrEvent.SpawnSide.ToLower().Equals("r"))
                {
                    var ChunksRequired = CalculateNumberOfChunksRequired(CurrEvent.DespawnTime - CurrEvent.SpawnTime, data.GlobalData.MovementSpeed, 100);
                    for (int j = 0; j <= ChunksRequired; j++)
                    {
                        float SpawnDistance = CalculateLaneSpawnDistance(CurrEvent.SpawnTime, data.GlobalData.MovementSpeed);
                        GameObject LaneInstance = Instantiate(Resources.Load<GameObject>("prefabs/LaneRight_100m"), new Vector3(RIGHT_LANE_POS_X, RIGHT_LANE_POS_Y, SpawnDistance + (100 * j)), Quaternion.Euler(0, 180, 0));
                        _createdGameObjects.Add(LaneInstance);
                    }
                }
                else
                {
                    var ChunksRequired = CalculateNumberOfChunksRequired(CurrEvent.DespawnTime - CurrEvent.SpawnTime, data.GlobalData.MovementSpeed, 100);
                    for (int j = 0; j <= ChunksRequired; j++)
                    {
                        float SpawnDistance = CalculateLaneSpawnDistance(CurrEvent.SpawnTime, data.GlobalData.MovementSpeed);
                        GameObject LaneInstance = Instantiate(Resources.Load<GameObject>("prefabs/LaneLeft_100m"), new Vector3(LEFT_LANE_POS_X, LEFT_LANE_POS_Y, SpawnDistance + (100 * j)), Quaternion.identity);
                        _createdGameObjects.Add(LaneInstance);
                    }
                }
            }


        }
    }

    private int CalculateNumberOfChunksRequired(float MillesecondsOfChunkExistance, float MovementSpeed, int PrefabRoadLength)
    {
        float SecondsOfExistence = MillesecondsOfChunkExistance / 1000;

        float MetersOfRoadRequired = MovementSpeed * SecondsOfExistence;

        int ChunksRequired = (int)Math.Ceiling(MetersOfRoadRequired / PrefabRoadLength);

        // We are on the first road of the trial, so we need one less chunk (it will be loaded on start).
        if (CurrentRoadPrefab == 0)
        {
            ChunksRequired--;
        }

        return ChunksRequired;
    }

    private float CalculateLaneSpawnDistance(float MillesecondsToWaitForSpawn, float MovementSpeed)
    {
        float SecondsToWait = MillesecondsToWaitForSpawn / 1000;

        // We add 50 here because lane origin is dead center.
        return (MovementSpeed * SecondsToWait) + 50;
    }
}
