using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour {
    public GameObject ArAlignedMap;
    public bool debugOn = false;

    public void OpenDebug () {
        debugOn = !debugOn;
        foreach (Transform child in ArAlignedMap.transform) {
            child.gameObject.layer = LayerMask.NameToLayer (debugOn ? "Default" : "LowerMap");

            foreach (Transform building in child) {
                MeshRenderer meshRenderer = building.gameObject.GetComponent<MeshRenderer> ();
                meshRenderer.enabled = debugOn;
            }
        }
        
    }
}