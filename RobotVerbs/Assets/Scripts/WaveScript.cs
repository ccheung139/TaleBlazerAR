using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Accelerometer.Instance.OnShake += DoSomething;
    }

    private void DoSomething() {
        Debug.Log("shaked!");
    }
}   
