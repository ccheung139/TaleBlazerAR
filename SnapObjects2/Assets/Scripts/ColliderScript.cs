using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour {
    public Material grayMaterial;
    public Material redMaterial;
    public static bool isColliding = false;

    private bool triggered = false;
    private Collider other;

    void Update () {
        if (triggered && !other) {
            gameObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            triggered = false;
        }
    }

    void OnTriggerEnter (Collider col) {
        if (!col.gameObject.name.Contains ("Cylinder") && col.gameObject.transform.parent != gameObject.transform) {
            isColliding = true;
            ChangeToRed (col.gameObject);
        }
        triggered = true;
        other = col;
    }

    void OnTriggerExit (Collider col) {
        if (!col.gameObject.name.Contains ("Cylinder")) {
            isColliding = false;
            ChangeToGray (col.gameObject);
        }
        triggered = false;
    }

    private void ChangeToGray (GameObject obj) {
        obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
    }

    private void ChangeToRed (GameObject obj) {
        obj.GetComponent<Renderer> ().sharedMaterial = redMaterial;
    }
}