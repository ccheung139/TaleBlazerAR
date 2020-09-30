using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieDamage : MonoBehaviour {
    public Text zombieDamageText;

    public void ChangeZombieDamage (float zombieDamageValue) {
        int zombieDamageInt = (int) zombieDamageValue;
        zombieDamageText.text = "Zombie Damage: " + zombieDamageInt;
    }
}