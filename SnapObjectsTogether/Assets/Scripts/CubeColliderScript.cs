using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeColliderScript : MonoBehaviour {

    public static bool isColliding = false;

    void OnTriggerEnter (Collider col) {
        // Debug.Log ("cube colliding: " + col.gameObject.name);
        if (!col.gameObject.name.Contains ("Cylinder")) {
            Debug.Log ("is colliding on: " + isColliding);
            isColliding = true;
            ChangeToRed (col.gameObject);
            // Destroy(col.gameObject);
        }
    }

    void OnTriggerExit (Collider col) {
        // Debug.Log ("no longer colliding: " + col.gameObject.name);
        if (!col.gameObject.name.Contains ("Cylinder")) {
            isColliding = false;
        }
    }

    private void ChangeToRed (GameObject obj) {
        obj.GetComponent<MeshRenderer> ().material.color = Color.red;
    }
}