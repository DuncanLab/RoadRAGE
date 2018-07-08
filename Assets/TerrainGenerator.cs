using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the basic "infinite scroll" effect that is 
// is used for the simulation.
public class TerrainGenerator : MonoBehaviour {

    // This is the standard (prefab) chunk we want to load to simulate
    // an infinite roadway.
    public GameObject BasicTerrainChunk;

    public GameObject currTerrain;

    public GameObject currTerrainChunk;
    public GameObject prevTerrainChunk;

    public List<GameObject> _createdGameObjects;
   
	// Use this for initialization.
	void Start () {
        // Simulation starts with only one chunk.
        currTerrainChunk = GameObject.Find("BasicTerrainChunk1");
        prevTerrainChunk = currTerrainChunk;


        _createdGameObjects.Add(GameObject.Find("LaneRight1"));
        _createdGameObjects.Add(GameObject.Find("billboard1"));
        _createdGameObjects.Add(prevTerrainChunk);
    }
	
	// Update is called once per frame.
	void Update () {
        var relativePos = transform.position.z - currTerrain.transform.position.z;

        // Generate the next terrain chunk if the car is close enough.
        if (relativePos > 10)
        {
            currTerrainChunk = Instantiate(BasicTerrainChunk, new Vector3(0, 0, currTerrain.transform.position.z + 1000f), Quaternion.identity);   
            currTerrain = currTerrainChunk.transform.Find("BaseTerrain").gameObject;
            _createdGameObjects.Add(prevTerrainChunk);
        }

        // We have left the previous terrain chunk, never to return - so destroy all of it.
        if (transform.position.z - prevTerrainChunk.transform.Find("BaseTerrain").gameObject.transform.position.z > 1100)
        {
            DestroyPreviousChunk();
            prevTerrainChunk = currTerrainChunk;
        }

	}

    private void DestroyPreviousChunk()
    {
        for (int i = 0; i < _createdGameObjects.Count; i++)
        {
            Destroy(_createdGameObjects[i]);
        }
    }
}
