using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberOfZombies : MonoBehaviour {
    public Text numZombiesText;

    public void ChangeNumZombies (float numZombies) {
        int numZombiesInt = (int) numZombies;
        numZombiesText.text = "Number of Zombies: " + numZombiesInt;
    }
}