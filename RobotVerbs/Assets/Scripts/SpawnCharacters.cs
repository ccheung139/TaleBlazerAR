using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent (typeof (ARRaycastManager))]
public class SpawnCharacters : MonoBehaviour {
    public Camera arCamera;
    public GameObject robotPrefab;
    public GameObject selectedSpherePrefab;
    public Text imposterGuessesText;
    public Text resultText;
    public Button guessImposterButton;
    public Button holdButton;
    public Button shakeHandButton;
    public Button punchButton;

    public static bool handshakeOn = false;
    public static bool punchOn = false;

    private int numRobots = 3;
    private int numImposterGuesses = 3;

    [Header ("For detecting button press")]
    private ARRaycastManager arRaycastManager;

    private GameObject selectedRobot = null;
    private GameObject imposter = null;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();

    private float textTimer = 0.0f;
    private float textTimerTotal = 5.0f;
    private bool interacting = false;

    void Start () {
        guessImposterButton.onClick.AddListener (CheckImposter);
        guessImposterButton.gameObject.SetActive (false);
        holdButton.gameObject.SetActive (false);
        shakeHandButton.onClick.AddListener (StartHandshake);
        shakeHandButton.gameObject.SetActive (false);
        punchButton.onClick.AddListener (StartPunch);
        punchButton.gameObject.SetActive (false);

        imposterGuessesText.text = "Imposter Guesses: " + numImposterGuesses;
        arRaycastManager = GetComponent<ARRaycastManager> ();

        System.Random rand = new System.Random ();
        for (int i = 0; i < numRobots; i++) {
            SpawnNewRobot (rand, false);
        }
        SpawnNewRobot (rand, true);
    }

    void Update () {
        HandleTouch ();
        HandleInteractRobot ();
        HandleResultText ();
    }

    private void HandleResultText () {
        if (resultText.text != "") {
            ClearInteractions ();
            if (textTimer >= textTimerTotal) {
                resultText.text = "";
                textTimer = 0;
            } else {
                textTimer += Time.deltaTime * 1.0f;
            }
        }
    }

    private void ClearInteractions () {
        interacting = false;
        handshakeOn = false;
        punchOn = false;
    }

    private void HandleInteractRobot () {
        if (selectedRobot != null) {
            float distance = Vector3.Distance (arCamera.transform.position, selectedRobot.transform.position);
            if (distance < 2f && !interacting) {
                shakeHandButton.gameObject.SetActive (true);
                punchButton.gameObject.SetActive (true);
            } else if (!interacting && distance >= 2f) {
                DisableAllGestureButtons();
            }
        }
    }

    private void SpawnNewRobot (System.Random rand, bool isImposter) {
        Vector3 playerPosition = arCamera.transform.position;

        float randX = (float) (rand.NextDouble () * 0.5);
        float randZ = (float) ((rand.NextDouble () * 2) + 1);
        Vector3 newPosition = playerPosition + new Vector3 (rand.Next (2) == 1 ? randX : -randX, -1.5f, randZ);

        GameObject newRobot = Instantiate (robotPrefab, newPosition, Quaternion.Euler (0, 180, 0));
        GameObject selectedSphere = Instantiate (selectedSpherePrefab, newPosition + new Vector3 (0, 1, 0), Quaternion.identity, newRobot.transform);
        selectedSphere.SetActive (false);

        UseDegreeTurningProperty (newRobot, isImposter, newPosition, selectedSphere);
        UseHandshakeProperty (newRobot, isImposter, selectedSphere);
        UsePunchProperty (newRobot, isImposter, selectedSphere);

        if (isImposter) {
            imposter = newRobot;
        }
    }

    private void UsePunchProperty (GameObject newRobot, bool isImposter, GameObject selectedSphere) {
        PunchScript script = newRobot.AddComponent<PunchScript> ();
        script.arCamera = arCamera;
        script.isImposter = isImposter;
        script.resultText = resultText;
        script.gestureButtonScript = holdButton.GetComponent<GestureButton> ();
        script.selectedSphere = selectedSphere;
    }

    private void UseHandshakeProperty (GameObject newRobot, bool isImposter, GameObject selectedSphere) {
        HandshakeProperty script = newRobot.AddComponent<HandshakeProperty> ();
        script.arCamera = arCamera;
        script.isImposter = isImposter;
        script.resultText = resultText;
        script.gestureButtonScript = holdButton.GetComponent<GestureButton> ();
        script.selectedSphere = selectedSphere;
    }

    private void UseDegreeTurningProperty (GameObject newRobot, bool isImposter, Vector3 newPosition, GameObject selectedSphere) {
        DegreeTurningProperty script = newRobot.AddComponent<DegreeTurningProperty> ();
        script.arCamera = arCamera;
        script.isImposter = isImposter;
        script.selectedSphere = selectedSphere;
    }

    private void HandleTouch () {
        if (Input.touchCount == 0) {
            return;
        }
        Touch touch = Input.GetTouch (0);

        if (EventSystem.current.currentSelectedGameObject != null || touch.phase != TouchPhase.Began) {
            return;
        }

        TouchOccurred (touch);
    }

    private void TouchOccurred (Touch touch) {
        holdButton.gameObject.SetActive (false);
        shakeHandButton.gameObject.SetActive (false);
        interacting = false;

        Ray ray = arCamera.ScreenPointToRay (touch.position);
        RaycastHit hitObject;
        if (Physics.Raycast (ray, out hitObject)) {
            GameObject obj = hitObject.transform.gameObject;

            if (selectedRobot != null && selectedRobot != obj) {
                selectedRobot.GetComponent<DegreeTurningProperty> ().selectedSphere.SetActive (false);
                selectedRobot = null;
            }
            obj.GetComponent<DegreeTurningProperty> ().selectedSphere.SetActive (true);
            selectedRobot = obj;
            guessImposterButton.gameObject.SetActive (true);
        } else {

            if (selectedRobot != null) {
                selectedRobot.GetComponent<DegreeTurningProperty> ().selectedSphere.SetActive (false);
                selectedRobot = null;
            }
            guessImposterButton.gameObject.SetActive (false);
        }
    }

    private void CheckImposter () {
        if (imposter != null && selectedRobot != null) {
            if (selectedRobot == imposter) {
                imposterGuessesText.text = "You found the imposter!";
            } else {
                numImposterGuesses -= 1;
                imposterGuessesText.text = "Imposter Guesses: " + numImposterGuesses;
            }
        }
    }

    private void StartHandshake () {
        interacting = true;
        DisableAllGestureButtons();
        holdButton.gameObject.SetActive (true);
        resultText.text = "";
        handshakeOn = true;
    }

    private void StartPunch () {
        interacting = true;
        DisableAllGestureButtons();
        holdButton.gameObject.SetActive (true);
        resultText.text = "";
        punchOn = true;
    }

    private void DisableAllGestureButtons () {
        shakeHandButton.gameObject.SetActive (false);
        punchButton.gameObject.SetActive (false);
    }
}