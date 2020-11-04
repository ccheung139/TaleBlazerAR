using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour {
    public GameObject housePlacedPrefab;
    public GameObject housePlaced;

    private List<GameObject> storedBirds = new List<GameObject> ();

    public void PlaceHouse (Vector3 position, Quaternion rotation) {
        Quaternion newRotation = rotation;
        newRotation.x = 0;
        newRotation.z = 0;
        housePlaced = Instantiate (housePlacedPrefab, position, newRotation);
        if (storedBirds.Count != 0) {
            foreach (GameObject storedBird in storedBirds) {
                PlaceBird (storedBird);
            }
        }
        storedBirds.Clear ();
    }

    public void MoveBirdToHouse (GameObject bird) {
        if (housePlaced == null) {
            storedBirds.Add (bird);
        } else {
            PlaceBird (bird);
        }
    }

    private void PlaceBird (GameObject bird) {
        System.Random rand = new System.Random ();
        float randX = (float) (rand.NextDouble () * 0.2f);
        float randZ = (float) (rand.NextDouble () * 0.2f);
        Vector3 newPosition = housePlaced.transform.position + new Vector3 (rand.Next (2) == 1 ? randX : -randX, 0,
            rand.Next (2) == 1 ? randZ : -randZ);
        // GameObject newBird = Instantiate (bird, newPosition, Quaternion.identity);
        bird.transform.position = newPosition;
        bird.SetActive (true);
    }
}