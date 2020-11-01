using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoopScript : MonoBehaviour {
    public Camera arCamera;
    public FollowLineController flc;
    public Text searchingText;

    private bool seen = false;

    // Update is called once per frame
    void Update () {
        if (!seen) {
            if (Vector3.Distance (arCamera.transform.position, transform.position) <= .2f) {
                flc.poopsSeen += 1;
                seen = true;
                searchingText.text = "You found bird litter! Look for more clues.";
            }
        }

    }
}