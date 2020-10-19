using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereColliderScript : MonoBehaviour {

    public static bool isColliding = false;

    void OnTriggerEnter (Collider col) {
        Debug.Log ("sphere colliding: " + col.gameObject.name);
        if (!col.gameObject.name.Contains ("Cylinder")) {
            isColliding = true;
            ChangeToRed (col.gameObject);
        }
    }

    void OnTriggerExit (Collider col) {
        Debug.Log ("no longer colliding: " + col.gameObject.name);
        if (!col.gameObject.name.Contains ("Cylinder")) {
            isColliding = false;
        }
    }

    private void ChangeToRed (GameObject obj) {
        obj.GetComponent<MeshRenderer> ().material.color = Color.red;
    }
}