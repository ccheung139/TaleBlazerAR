using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintColliderScript : MonoBehaviour {
    public GameObject paintRollerObj;

    void OnTriggerEnter (Collider col) {
        Debug.Log (col.gameObject.name);
        if (col.gameObject.name.Contains ("ScaleSphere")) {
            return;
        }
        
        Color color = paintRollerObj.GetComponent<Renderer> ().material.color;
        Material colMat = col.gameObject.GetComponent<Renderer> ().material;
        colMat.color = color;
    }
}