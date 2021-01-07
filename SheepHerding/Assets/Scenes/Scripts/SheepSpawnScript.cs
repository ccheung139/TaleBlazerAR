using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpawnScript : MonoBehaviour {
    public Camera arCamera;
    public GameObject sheepPrefab;
    public GameObject barn;
    public BarnAndSheepScript barnAndSheepScript;

    private int totalSheep = 5;
    private Vector3 v3Center;
    private Vector3 v3Extents;

    public void StartSheepHerd (Vector3 v3C, Vector3 v3E) {
        v3Center = v3C;
        v3Extents = v3E;
        System.Random rand = new System.Random ();
        for (int i = 0; i < totalSheep; i++) {
            SpawnNewSheep (rand);
        }
    }

    private void SpawnNewSheep (System.Random rand) {
        Vector3 newPosition = GenerateRandomPoint (rand);
        newPosition.y = barn.transform.position.y;

        float randYRot = (float) (rand.NextDouble () * 180);
        Quaternion newRotation = Quaternion.Euler (0, randYRot, 0);
        GameObject newSheep = Instantiate (sheepPrefab, newPosition, newRotation);

        SheepMovementScript sheepMovementScript = newSheep.GetComponent<SheepMovementScript> ();
        sheepMovementScript.arCamera = arCamera;
        sheepMovementScript.barn = barn;
        sheepMovementScript.rand = rand;
        sheepMovementScript.barnAndSheepScript = barnAndSheepScript;
        sheepMovementScript.v3Center = v3Center;
        sheepMovementScript.v3Extents = v3Extents;
    }

    private Vector3 GenerateRandomPoint (System.Random rand) {
        Vector3 playerPosition = arCamera.transform.position;
        float randX = (v3Center.x) + (float) (rand.NextDouble () * v3Extents.x * (rand.Next (2) == 1 ? 1 : -1));
        float randZ = (v3Center.z) + (float) (rand.NextDouble () * v3Extents.z * (rand.Next (2) == 1 ? 1 : -1));
        Vector3 newPosition = new Vector3 (randX, 0, randZ);
        return newPosition;
    }

    private bool CheckInBounds (Vector3 position) {
        Vector3 v3CenterFlat = new Vector3 (v3Center.x, position.y, v3Center.z);
        Vector3 v3ExtentExpanded = new Vector3 (v3Extents.x * 2f, 100f, v3Extents.z * 2f);
        Bounds bounds = new Bounds (v3CenterFlat, v3ExtentExpanded);
        return bounds.Contains (position);
    }
}