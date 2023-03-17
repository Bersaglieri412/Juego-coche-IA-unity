using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    public float maxtorque = 200f;
    public float maxSteer = 20f;
    public bool IA;

    public Transform centerOfMass;
    public Rigidbody _rigidbody;
    
    public float steer { get; set; }
    public float torque { get; set; }

    private Wheel[] wheels;

    // Start is called before the first frame update
    void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();   
        _rigidbody=GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        bool frena = false;
        if (!IA) { 
        steer=GameManager.instance.controller.inputWheel;
        torque = GameManager.instance.controller.inputThrottle;
        }
        else
        {
            if (torque < 0) frena = true;

            else frena = false;
            
        }

        foreach (Wheel wheel in wheels)
        {
            wheel.steerAngle = steer * maxSteer;
            wheel.torque = maxtorque * torque;
            if (Input.GetKey(KeyCode.Space) && !IA)
            {
                wheel.brake = 900f;

            }
            else if (!IA)
            {
                wheel.brake = 0f;
            }

            }
    }
    public void parar()
    {
        GetComponent<Rigidbody>().velocity=Vector3.zero;
    }
    public void getInputIa(float acelerar, float girar)
    {
        torque = acelerar;
        steer=girar;
    }
}
