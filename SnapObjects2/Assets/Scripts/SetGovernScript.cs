using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGovernScript : MonoBehaviour {
    public Material grayMaterial;
    public List<List<GameObject>> shapeSets = new List<List<GameObject>> ();

    public void AddToFormerSet (GameObject previousObject, GameObject newObject) {
        foreach (List<GameObject> potentialList in shapeSets) {
            if (potentialList.Contains (previousObject)) {
                potentialList.Add (newObject);
                return;
            }
        }
    }

    public void CombineSets (GameObject firstObject, GameObject secondObject) {
        int firstIndex = -1;
        int secondIndex = -1;
        for (int i = 0; i < shapeSets.Count; i++) {
            List<GameObject> potentialList = shapeSets[i];
            if (potentialList.Contains (firstObject)) {
                firstIndex = i;
            } else if (potentialList.Contains (secondObject)) {
                secondIndex = i;
            }
        }
        List<GameObject> firstList = shapeSets[firstIndex];
        List<GameObject> secondList = shapeSets[secondIndex];

        int maxIndex;
        int minIndex;
        if (firstIndex > secondIndex) {
            maxIndex = firstIndex;
            minIndex = secondIndex;
        } else {
            minIndex = firstIndex;
            maxIndex = secondIndex;
        }
        shapeSets.RemoveAt (maxIndex);
        shapeSets.RemoveAt (minIndex);

        firstList.AddRange (secondList);
        shapeSets.Add (firstList);
    }
}