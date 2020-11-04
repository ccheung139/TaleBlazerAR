using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NestSpawner : MonoBehaviour {
    public Canvas canvas;
    public GameObject blueJayPrefab;
    public GameObject cardinalPrefab;
    public GameObject chickadeePrefab;
    public GameObject crowPrefab;
    public GameObject goldfinchPrefab;
    public GameObject robinPrefab;
    public GameObject sparrowPrefab;
    public Button startButton;
    public Button holdButton;
    public Button cancelButton;
    public Camera arCamera;
    public Text resultText;

    public ToastThrownScript toastThrownScript;
    public InventoryScript invScript;
    public HouseScript houseScript;

    private List<GameObject> differentBirds;
    private System.Random rand;

    void Start () {
        differentBirds = new List<GameObject> () {
            blueJayPrefab,
            cardinalPrefab,
            chickadeePrefab,
            crowPrefab,
            goldfinchPrefab,
            robinPrefab,
            sparrowPrefab,
        };
    }

    public void SpawnBird (Vector3 location) {
        GameObject selectedPrefab = PickRandomBird ();
        GameObject newBird = Instantiate (selectedPrefab, location, Quaternion.identity);

        BirdAttraction baScript = newBird.GetComponent<BirdAttraction> ();
        baScript.invScript = invScript;
        baScript.arCamera = arCamera;
        baScript.cancelButton = cancelButton;
        baScript.toastThrownScript = toastThrownScript;
        baScript.houseScript = houseScript;
    }

    private GameObject PickRandomBird () {
        rand = new System.Random ();
        int randomIndex = rand.Next (differentBirds.Count);
        return differentBirds[randomIndex];
    }
}