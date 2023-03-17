using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class Wheel : MonoBehaviour
{
    public bool steering;
    public bool inverseSteering;
    public bool power;
    public bool handBrake;  

    public float steerAngle { get; set; }
    public float torque { get; set; }

    public float brake { get; set; }
    public ParticleSystem humo;

    private WheelCollider wheelCollider;
    private Transform WheelTransform;
    // Start is called before the first frame update
    void Start()
    {
        humo = GetComponentInChildren<ParticleSystem>();
        var emission = humo.emission;
        emission.enabled = false;
        wheelCollider = GetComponentInChildren<WheelCollider>();
        wheelCollider=GetComponentInChildren<WheelCollider>();
        WheelTransform=GetComponentInChildren<MeshFilter>().GetComponent<Transform>();
        wheelCollider.brakeTorque = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        /*wheelCollider.GetGroundHit(out WheelHit wheelData);
        float slipLat = wheelData.sidewaysSlip;
        float slipLong = wheelData.forwardSlip;
        print("sidewaysSlip"+slipLat);
        print("forwardSlip"+slipLong);
        var emission = humo.emission;
        if (slipLong > 0.5) emission.enabled = true;
        else emission.enabled = false;
        */
        comprobarSlip();
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot); 
        WheelTransform.rotation = rot;
        WheelTransform.position = pos;
    }

    private void FixedUpdate()
    {
        if (steering) wheelCollider.steerAngle = steerAngle * (inverseSteering ? -1 : 1);
        if(power) wheelCollider.motorTorque=torque;
        if (handBrake) wheelCollider.brakeTorque=brake;
    }

    private void comprobarSlip()
    {
        WheelHit hit = new WheelHit();
        var emission = humo.emission;
        if (this.wheelCollider.GetGroundHit(out hit))
        {
            if (hit.sidewaysSlip < -0.25) { } // emission.enabled = emission.enabled = true;
            else if (hit.forwardSlip > 0.5) { } // emission.enabled = emission.enabled = true;
            else emission.enabled = false;
        }
    }
}
