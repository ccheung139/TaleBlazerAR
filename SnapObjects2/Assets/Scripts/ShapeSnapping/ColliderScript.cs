using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour {
    public SetGovernScript setGovernScript;
    public Material grayMaterial;
    public Material redMaterial;
    public static bool isColliding = false;
    public List<GameObject> collidingObjects = new List<GameObject> ();

    private bool triggered = false;
    private Collider other;

    void Update () {
        if (triggered && !other) {
            gameObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            triggered = false;
        }
    }

    void OnTriggerEnter (Collider col) {
        if (col.gameObject.name.Contains ("ScaleSphere")) {
            return;
        }
        if (!collidingObjects.Contains (col.gameObject)) {
            collidingObjects.Add (col.gameObject);
        }
        if (!col.gameObject.name.Contains ("Cylinder") &&
            col.gameObject.transform.parent != gameObject.transform &&
            !CheckSameSet (col.gameObject)) {
            isColliding = true;
            ChangeToRed (col.gameObject);
        }
        triggered = true;
        other = col;
    }

    void OnTriggerExit (Collider col) {
        if (col.gameObject.name.Contains ("ScaleSphere")) {
            return;
        }
        if (collidingObjects.Contains (col.gameObject)) {
            collidingObjects.Remove (col.gameObject);
        }
        if (!col.gameObject.name.Contains ("Cylinder")) {
            isColliding = false;
            ChangeToGray (col.gameObject);
        }
        triggered = false;
    }

    private bool CheckSameSet (GameObject obj) {
        foreach (List<GameObject> potentialSet in setGovernScript.shapeSets) {
            if (potentialSet.Contains (gameObject) && potentialSet.Contains (obj)) {
                return true;
            }
        }
        return false;
    }

    private void ChangeToGray (GameObject obj) {
        obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
    }

    private void ChangeToRed (GameObject obj) {
        obj.GetComponent<Renderer> ().sharedMaterial = redMaterial;
    }
}