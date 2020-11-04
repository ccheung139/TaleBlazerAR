using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public HoldButtonScript holdButtonScript;
    public InventoryScript invScript;
    public ToastThrownScript toastThrownScript;

    private Vector3 startPos;
    private Vector3 finalPos;

    private List<Vector3> shakes = new List<Vector3> ();

    void Start () {
        Accelerometer.Instance.OnShake += ShakeOccured;
    }

    void Update () {
        if (HoldButtonScript.isPressing) {
            if (shakes.Count <= 1) {
                return;
            }
            Vector3 firstShake = shakes[0];
            Vector3 lastShake = shakes[shakes.Count - 1];
            if (Vector3.Distance (firstShake, lastShake) >= 0.2f) {
                ThrowOccured (lastShake - firstShake);
            }
        } else {
            shakes.Clear ();
        }
    }

    private void ShakeOccured () {
        shakes.Add (arCamera.transform.position);
    }

    private void ThrowOccured (Vector3 direction) {
        resultText.text = "You threw!";
        toastThrownScript.StartThrow (direction,
            invScript.breadInHand.transform.position, invScript.breadInHand.transform.rotation);

        holdButtonScript.Released ();
        invScript.DisableActions ();
    }
}