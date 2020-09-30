using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumWallsSlider : MonoBehaviour {
    public Text numWallsSliderText;

    public void ChangeNumWalls (float numWalls) {
        int numWallsInt = (int) numWalls;
        numWallsSliderText.text = "Available Walls: " + numWallsInt;
    }
}