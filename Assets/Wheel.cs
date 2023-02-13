using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool steering;
    public bool inverseSteering;
    public bool power;
    public bool handBrake;  

    public float steerAngle { get; set; }
    public float torque { get; set; }

    public float brake { get; set; }


    private WheelCollider wheelCollider;
    private Transform WheelTransform;
    // Start is called before the first frame update
    void Start()
    {
        wheelCollider=GetComponentInChildren<WheelCollider>();
        WheelTransform=GetComponentInChildren<MeshFilter>().GetComponent<Transform>();
        wheelCollider.brakeTorque = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot); 
        WheelTransform.rotation = rot;
        WheelTransform.position = pos;
    }

    private void FixedUpdate()
    {
        print(wheelCollider.brakeTorque);
        if (steering) wheelCollider.steerAngle = steerAngle * (inverseSteering ? -1 : 1);
        if(power) wheelCollider.motorTorque=torque;
        if (handBrake) wheelCollider.brakeTorque=brake;
    }
}
