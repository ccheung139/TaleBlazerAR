using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnAndSheepScript : MonoBehaviour {
    public GameObject barn;
    public GameObject leftSide;
    public GameObject rightSide;
    public GameObject backSide;
    public GameObject frontSide;

    public bool HittingSidesOfBarn (Vector3 newPosition) {
        Vector3 newPosFlat = new Vector3 (newPosition.x, 0, newPosition.z);
        Vector3 barnPosition = barn.transform.position;
        Vector3 barnPosFlat = new Vector3 (barnPosition.x, 0, barnPosition.z);

        float distance = Vector3.Distance (newPosFlat, barnPosFlat);
        if (distance > 0.5f) {
            return false;
        }
        float leftDistance = GetDistance (leftSide, newPosFlat);
        float rightDistance = GetDistance (rightSide, newPosFlat);
        float backDistance = GetDistance (backSide, newPosFlat);
        float frontDistance = GetDistance (frontSide, newPosFlat);

        if (frontDistance < leftDistance && frontDistance < rightDistance && frontDistance < backDistance) {
            return false;
        } else {
            return true;
        }
    }

    public bool InsideBarn (Vector3 newPosition) {
        Vector3 insidePosition = frontSide.transform.position;
        insidePosition.y = 0;
        Vector3 sheepPos = newPosition;
        sheepPos.y = 0;
        if (Vector3.Distance (insidePosition, sheepPos) <= 0.2f) {
            return true;
        }
        return false;
    }

    private float GetDistance (GameObject sideObj, Vector3 newPosFlat) {
        return Vector3.Distance (sideObj.transform.position, newPosFlat);
    }
}