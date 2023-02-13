using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    public float maxtorque = 200f;
    public float maxSteer = 20f;

    public Transform centerOfMass;
    private Rigidbody _rigidbody;

    public float steer { get; set; }
    public float torque { get; set; }

    private Wheel[] wheels;
    // Start is called before the first frame update
    void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        _rigidbody.GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        steer=GameManager.instance.controller.inputWheel;
        torque = GameManager.instance.controller.inputThrottle;
        foreach (Wheel wheel in wheels)
        {
            print(maxtorque * torque);
            wheel.steerAngle = steer * maxSteer;
            wheel.torque = maxtorque * torque;
            if (Input.GetKey(KeyCode.Space))
            {
                wheel.brake = 900f;
                print("si");
            }
            else wheel.brake = 0f;
        }
    }
}
