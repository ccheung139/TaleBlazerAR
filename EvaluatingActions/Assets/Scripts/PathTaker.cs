using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathTaker : MonoBehaviour {
    public Button startButton;
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
        gameObject.SetActive (false);
        Accelerometer.Instance.OnShake += ShakeOccured;
    }

    void Update () {
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

        if (jumpResult) {
            resultText.text = "You jumped!";
        } else if (throwResult) {
            resultText.text = "You threw!";
        } else if (handshakeResult) {
            resultText.text = "You shook hands!";
        } else if (punchResult) {
            resultText.text = "You punched!";
        } else if (waveResult) {
            resultText.text = "You waved!";
        }

        points = new List<Vector3> ();
        pointsInTime = new List<float> ();
        shakeTimes = new List<float> ();
        rotationTimes = new List<float> ();
        startButton.gameObject.SetActive (true);
        gameObject.SetActive (false);
    }

    public void PressedStart () {
        resultText.text = "";
        startButton.gameObject.SetActive (false);
        gameObject.SetActive (true);
    }

    private void ShakeOccured () {
        if (isPressing) {
            shakeTimes.Add (Time.time);
        }
    }
}