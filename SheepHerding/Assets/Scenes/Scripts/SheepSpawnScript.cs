using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheepSpawnScript : MonoBehaviour {
    public Camera arCamera;
    public GameObject sheepPrefab;
    public LoadSpaceScript loadSpaceScript;
    public GrassPlacementScript grassPlacementScript;
    public Button plantGrassButton;
    public Button openWaterCanButton;

    private int totalSheep = 5;
    private Vector3 v3Center;
    private Vector3 v3Extents;
    private Bounds room1Bounds;
    private Bounds room2Bounds;
    private List<Bounds> connectingRoomBounds;

    public void StartSheepHerd (Bounds r1b, Bounds r2b, List<Bounds> crb) {
        v3Center = r1b.center;
        v3Extents = r1b.extents;
        room1Bounds = r1b;
        room2Bounds = r2b;
        connectingRoomBounds = crb;

        System.Random rand = new System.Random ();
        for (int i = 0; i < totalSheep; i++) {
            SpawnNewSheep (rand);
        }

        plantGrassButton.gameObject.SetActive (true);
        openWaterCanButton.gameObject.SetActive (true);

        grassPlacementScript.room1Bounds = room1Bounds;
        grassPlacementScript.room2Bounds = room2Bounds;
    }

    private void SpawnNewSheep (System.Random rand) {
        Vector3 newPosition = GenerateRandomPoint (rand);
        newPosition.y = -0.8f;

        float randYRot = (float) (rand.NextDouble () * 180);
        Quaternion newRotation = Quaternion.Euler (0, randYRot, 0);
        GameObject newSheep = Instantiate (sheepPrefab, newPosition, newRotation);

        SheepMovementScript sheepMovementScript = newSheep.GetComponent<SheepMovementScript> ();
        sheepMovementScript.arCamera = arCamera;
        sheepMovementScript.rand = rand;
        sheepMovementScript.v3Center = v3Center;
        sheepMovementScript.v3Extents = v3Extents;
        sheepMovementScript.room1Bounds = room1Bounds;
        sheepMovementScript.room2Bounds = room2Bounds;
        sheepMovementScript.connectingRoomBounds = connectingRoomBounds;
    }

    private Vector3 GenerateRandomPoint (System.Random rand) {
        float randX = (v3Center.x) + (float) (rand.NextDouble () * v3Extents.x * (rand.Next (2) == 1 ? 1 : -1));
        float randZ = (v3Center.z) + (float) (rand.NextDouble () * v3Extents.z * (rand.Next (2) == 1 ? 1 : -1));
        Vector3 newPosition = new Vector3 (randX, 0, randZ);
        // return loadSpaceScript.RotatePointAroundPivot (newPosition, pivot, relative);
        return newPosition;
    }

    // private bool CheckInBounds (Vector3 position) {
    //     Vector3 v3CenterFlat = new Vector3 (v3Center.x, position.y, v3Center.z);
    //     Vector3 v3ExtentExpanded = new Vector3 (v3Extents.x * 2f, 100f, v3Extents.z * 2f);
    //     Bounds bounds = new Bounds (v3CenterFlat, v3ExtentExpanded);
    //     return bounds.Contains (position);
    // }
}