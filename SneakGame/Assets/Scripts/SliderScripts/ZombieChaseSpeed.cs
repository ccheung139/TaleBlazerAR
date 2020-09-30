using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieChaseSpeed : MonoBehaviour {
    public Text zombieChaseText;

    public void ChangeZombieChaseSpeed (float chaseSpeed) {
        zombieChaseText.text = "Zombie Chase Speed: " + chaseSpeed;
    }
}