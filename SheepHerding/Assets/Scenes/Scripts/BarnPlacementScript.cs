using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarnPlacementScript : MonoBehaviour {
    public Button placeBarnButton;
    public Camera arCamera;
    public GameObject barn;
    public SheepSpawnScript sheepSpawnScript;
    public Button drawSpaceButton;
    public GameObject grass;

    private Vector3 v3Center;
    private Vector3 v3Extents;

    private bool isPlacing = false;
    private bool isGrowing = false;

    // void Start () {
    //     placeBarnButton.onClick.AddListener (PlaceBarnPressed);
    // }

    void Update () {
        if (isPlacing) {
            if (barn.transform.position.y < grass.transform.position.y) {
                isPlacing = false;
                barn.transform.parent = null;
                Quaternion newRotation = arCamera.transform.rotation * Quaternion.Euler (0, 0, 0);
                newRotation.x = 0;
                newRotation.z = 0;
                barn.transform.rotation = newRotation;
                isGrowing = true;
            }
        } else if (isGrowing) {
            Grow ();
        }
    }

    private void Grow () {
        if (barn.transform.localScale.x >= .08f) {
            isGrowing = false;
            sheepSpawnScript.StartSheepHerd (v3Center, v3Extents);
            // drawSpaceButton.gameObject.SetActive (true);
            // sheepSpawnScript.StartSheepHerd ();
        } else {
            barn.transform.localScale += new Vector3 (0.001f, 0.001f, 0.001f);
        }
    }

    public void PlaceBarnPressed (Vector3 center, Vector3 extents) {
        v3Center = center;
        v3Extents = extents;

        barn.gameObject.SetActive (true);
        barn.transform.position = arCamera.transform.position + arCamera.transform.forward * 0.8f - new Vector3 (0, 0.2f, 0);
        barn.transform.rotation = arCamera.transform.rotation * Quaternion.Euler (0, 0, 0);
        barn.transform.parent = arCamera.transform;
        placeBarnButton.gameObject.SetActive (false);
        isPlacing = true;
    }
}