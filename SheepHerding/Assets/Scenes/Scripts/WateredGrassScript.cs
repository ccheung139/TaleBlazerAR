using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateredGrassScript : MonoBehaviour {
    public bool beenWatered = false;
    public bool beingEaten = false;

    void Update () {
        if (beenWatered && transform.localScale.x < 0.2f) {
            transform.localScale += new Vector3 (0.001f, 0.001f, 0.001f);
        }
    }

    public bool TryWateringGrass () {
        if (beenWatered) {
            return false;
        }
        beenWatered = true;
        return true;
    }

    public void ClaimGrass () {
        beingEaten = true;
    }

    public void UnclaimGrass () {
        beingEaten = false;
    }
}