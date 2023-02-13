using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputController : MonoBehaviour
{
    public string inputWheelAxis = "Horizontal";
    public string inputThrottleAxis = "Vertical";

    public float inputWheel {get; private set;}
    public float inputThrottle {get; private set;} 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputWheel=Input.GetAxis(inputWheelAxis);
        inputThrottle=Input.GetAxis(inputThrottleAxis);

    }
}
