using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceScript : MonoBehaviour {
    public GameObject connectorFencePrefab;
    public GameObject roomFencePrefab;

    public void DrawFences (Vector3 point1, Vector3 point2, int status) {
        Vector3 direction = Vector3.Normalize (point2 - point1);

        float distance = Vector3.Distance (point2, point1);
        float counter = 0;
        Vector3 pointToPlace = point1;

        GameObject typeOfFence;
        if (status == 0 || status == 2) {
            typeOfFence = roomFencePrefab;
        } else {
            typeOfFence = connectorFencePrefab;
        }
        float fenceLength = typeOfFence.GetComponent<MeshRenderer> ().bounds.size.x;
        while (counter < distance) {

            GameObject newFence = Instantiate (typeOfFence, pointToPlace, Quaternion.LookRotation (direction, Vector3.up) * Quaternion.Euler (0, 90, 0));

            if (counter + fenceLength > distance) {
                float remaining = distance - counter;
                float ratio = newFence.transform.localScale.x * remaining / fenceLength;
                newFence.transform.localScale = new Vector3 (ratio, newFence.transform.localScale.y, newFence.transform.localScale.z);
            }
            counter += fenceLength;
            pointToPlace = point1 + (direction * counter);
        }
    }

    public void TakeOutOverlapingFences (List<Bounds> connectingRooms, Bounds room1Bounds, Bounds room2Bounds) {
        List<Bounds> allBounds = new List<Bounds> (connectingRooms);
        allBounds.Add (room1Bounds);
        allBounds.Add (room2Bounds);

        List<GameObject> fencesToRemove = new List<GameObject> ();
        foreach (GameObject fence in GameObject.FindGameObjectsWithTag ("Fence")) {
            Vector3 fencePos = fence.transform.position;
            int count = 0;
            foreach (Bounds potentialBound in allBounds) {
                if (potentialBound.Contains (fencePos)) {
                    count += 1;
                }
            }
            if (count > 1) {
                fencesToRemove.Add (fence);
            }
        }
        foreach (GameObject fenceToRemove in fencesToRemove) {
            Destroy (fenceToRemove);
        }
    }
}