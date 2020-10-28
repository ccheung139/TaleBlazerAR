using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NestSpawner : MonoBehaviour {
    public Canvas canvas;
    public GameObject blueJayPrefab;
    public Button startButton;
    public Button holdButton;
    public Camera arCamera;
    public Text resultText;

    void Start () {
        startButton.gameObject.SetActive (false);
        holdButton.gameObject.SetActive (false);
    }

    public void SpawnBird (Vector3 location) {
        GameObject newBird = Instantiate (blueJayPrefab, location, Quaternion.identity);
        GameObject startButtonNew = Instantiate (startButton.gameObject, canvas.transform);
        startButtonNew.SetActive (true);
        GameObject holdButtonNew = Instantiate (holdButton.gameObject, canvas.transform);
        PathTaker ptScript = newBird.GetComponent<PathTaker> ();
        ptScript.startButton = startButtonNew.GetComponent<Button> ();
        ptScript.holdButton = holdButtonNew.GetComponent<Button> ();
        ptScript.arCamera = arCamera;
        ptScript.resultText = resultText;
    }
}