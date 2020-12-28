using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderColliderScript : MonoBehaviour {
    public List<GameObject> collidingObjects = new List<GameObject> ();
    public GameObject topNeighbor;
    public GameObject bottomNeighbor;

    private void CheckTop (Collider col) {
        GameObject top = gameObject.transform.Find ("TopOfCylinder").gameObject;
        if (col.bounds.Contains (top.transform.position)) {
            topNeighbor = col.gameObject;
        }
    }

    private void CheckBottom (Collider col) {
        GameObject bottom = gameObject.transform.Find ("BottomOfCylinder").gameObject;
        if (col.bounds.Contains (bottom.transform.position)) {
            bottomNeighbor = col.gameObject;
        }
    }

    void OnTriggerStay (Collider col) {
        if (collidingObjects.Contains (col.gameObject)) {
            if (topNeighbor == null) {
                CheckTop (col);
            }
            if (bottomNeighbor == null) {
                CheckBottom (col);
            }
        }
    }

    void OnTriggerEnter (Collider col) {
        if (col.gameObject.name.Contains ("Placed") || col.gameObject.name.Contains ("PaintColliderCapsule")) {
            return;
        }
        if (!collidingObjects.Contains (col.gameObject)) {
            collidingObjects.Add (col.gameObject);
        }
    }

    void OnTriggerExit (Collider col) {
        if (col.gameObject.name.Contains ("Placed") || col.gameObject.name.Contains ("PaintColliderCapsule")) {
            return;
        }
        if (collidingObjects.Contains (col.gameObject)) {
            if (col.gameObject == topNeighbor) {
                topNeighbor = null;
            } else if (col.gameObject == bottomNeighbor) {
                bottomNeighbor = null;
            }
            collidingObjects.Remove (col.gameObject);
        }
    }
}