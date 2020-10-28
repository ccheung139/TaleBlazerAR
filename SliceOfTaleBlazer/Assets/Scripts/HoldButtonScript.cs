using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldButtonScript : MonoBehaviour {
    public static bool isPressing = false;
    public static GameObject birdObject;

    public void PressingDown () {
        isPressing = true;
    }

    public void Released () {
        isPressing = false;
    }
}