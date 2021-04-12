using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimatorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Animator a = GetComponent<Animator>();
        a.keepAnimatorControllerStateOnDisable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
