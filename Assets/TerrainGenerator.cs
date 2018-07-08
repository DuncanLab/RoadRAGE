using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the basic "infinite scroll" effect that is 
// is used for the simulation.
public class TerrainGenerator : MonoBehaviour {

    // This is the standard chunk we want to load to simulate
    // an infinite roadway.
    public GameObject BasicTerrainChunk;

    public GameObject currTerrain;

    // Keep Track of curr, and previous chunks for GC.
    public GameObject currTerrainChunk;
    public GameObject prevTerrainChunk;
   

	// Use this for initialization.
	void Start () {
        // Simulation starts with only one chunk.
        currTerrainChunk = GameObject.Find("BasicTerrainChunk1");
        prevTerrainChunk = currTerrainChunk;
        ;

    }
	
	// Update is called once per frame.
	void Update () {
        var relativePos = transform.position.z - currTerrain.transform.position.z;

        // Generate the next terrain chunk if the car is close enough.
        if (relativePos > 10)
        {
            currTerrainChunk = Instantiate(BasicTerrainChunk, new Vector3(0, 0, currTerrain.transform.position.z + 1000f), Quaternion.identity);   
            currTerrain = currTerrainChunk.transform.Find("BaseTerrain").gameObject;
        }

        // We have left the previous terrain chunk so destroy it.
        if (transform.position.z - prevTerrainChunk.transform.Find("BaseTerrain").gameObject.transform.position.z > 1000)
        {
            // TODO : We should garbage collect everything from the previous chunk, not
            // just the basic terrain.
            Destroy(prevTerrainChunk);
            prevTerrainChunk = currTerrainChunk;
        }

	}
}
