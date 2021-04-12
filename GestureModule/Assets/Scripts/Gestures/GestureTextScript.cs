using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureTextScript : MonoBehaviour {
    public Text gestureText;

    private float gestureTextTimer = 0.0f;
    private float gestureTextTotal = 3.0f;

    void Update () {
        HandleGestureText ();
    }

    private void HandleGestureText () {
        if (gestureText.text != "") {
            if (gestureTextTimer < gestureTextTotal) {
                gestureTextTimer += Time.deltaTime;
            } else {
                gestureTextTimer = 0;
                gestureText.text = "";
            }
        }
    }
}