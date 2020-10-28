using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PathTaker : MonoBehaviour {
    // private GameObject blueJay = gameObject;
    public Button startButton;
    public Button holdButton;
    public Camera arCamera;
    public Text resultText;
    public JumpWatcher jumpWatcher;
    public PunchWatcher punchWatcher;
    public ThrowWatcher throwWatcher;
    public HandshakeWatcher handshakeWatcher;
    public WaveWatcher waveWatcher;

    private bool isPressing = false;
    private List<Vector3> points = new List<Vector3> ();
    private List<float> pointsInTime = new List<float> ();
    private List<float> shakeTimes = new List<float> ();
    private List<float> rotationTimes = new List<float> ();

    private float timer = 0.0f;
    private float timerTotal = 0.01f;

    void Start () {
        startButton.onClick.AddListener (PressedStart);
        holdButton.gameObject.SetActive (false);
        Accelerometer.Instance.OnShake += ShakeOccured;
        AddEventTriggers ();
    }

    void Update () {
        float distance = Vector3.Distance (gameObject.transform.position, arCamera.transform.position);
        if (distance < 1.5f) {
            if (!holdButton.gameObject.activeSelf) {
                startButton.gameObject.SetActive (true);
            }
        } else {
            startButton.gameObject.SetActive (false);
            holdButton.gameObject.SetActive (false);
            isPressing = false;
        }
        if (!isPressing) {
            return;
        }
        if (timer >= timerTotal) {
            points.Add (arCamera.transform.position);
            pointsInTime.Add (Time.time);
            rotationTimes.Add (Input.acceleration.x);
            timer = 0.0f;
        } else {
            timer += Time.deltaTime * 1.0f;
        }
    }

    private void AddEventTriggers () {
        EventTrigger trigger = holdButton.gameObject.GetComponent<EventTrigger> ();
        EventTrigger.Entry entry = new EventTrigger.Entry ();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener ((eventData) => { PressedHold (); });
        trigger.triggers.Add (entry);

        EventTrigger.Entry entry2 = new EventTrigger.Entry ();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener ((eventData) => { ReleasedHold (); });
        trigger.triggers.Add (entry2);
    }

    public void PressedHold () {
        isPressing = true;
    }

    public void ReleasedHold () {
        isPressing = false;

        bool jumpResult = jumpWatcher.CheckJump (points, pointsInTime);
        bool throwResult = throwWatcher.CheckThrow (points, pointsInTime);
        bool punchResult = punchWatcher.CheckThrow (points, pointsInTime);
        bool handshakeResult = handshakeWatcher.CheckHandshake (points, pointsInTime, shakeTimes);
        bool waveResult = waveWatcher.CheckWave (pointsInTime, rotationTimes);

        // Debug.Log(gameObject);
        // Debug.Log(startButton);
        // Debug.Log("REsult: " + resultText.text);

        if (jumpResult) {
            resultText.text = "You jumped!";
            gameObject.SendMessage ("Hop");
        } else if (throwResult) {
            resultText.text = "You threw!";
            gameObject.SendMessage ("Preen");
        } else if (handshakeResult) {
            resultText.text = "You shook hands!";
            gameObject.SendMessage ("Peck");
        } else if (punchResult) {
            resultText.text = "You punched!";
            gameObject.SendMessage ("Scared");
        } else if (waveResult) {
            resultText.text = "You waved!";
            gameObject.SendMessage ("Ruffle");
        }

        points = new List<Vector3> ();
        pointsInTime = new List<float> ();
        shakeTimes = new List<float> ();
        rotationTimes = new List<float> ();
        startButton.gameObject.SetActive (true);
        holdButton.gameObject.SetActive (false);
    }

    public void PressedStart () {
        resultText.text = "";
        startButton.gameObject.SetActive (false);
        holdButton.gameObject.SetActive (true);
    }

    private void ShakeOccured () {
        if (isPressing) {
            shakeTimes.Add (Time.time);
        }
    }
}