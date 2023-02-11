using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    private bool echado=false;

    void Start()
    {
        frontLeftWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);
        frontRightWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);
        rearLeftWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);
        rearRightWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);
    }

    private void FixedUpdate() {
        GetInput();
        
        HandleSteering();
        UpdateWheels();
        
    }
    private void Update(){
        if (Input.GetKeyDown(KeyCode.Space)) {
         //frenoMano();
         echado=true;
     }
        else{
            echado=false;
        }

    }
    private void LateUpdate(){
        HandleMotor();
    }
    private void frenoMano(){
        while (Input.GetKeyDown(KeyCode.Space))
        {
            rearLeftWheelCollider.brakeTorque = -3000;
            rearRightWheelCollider.brakeTorque = -3000;
        }
    }
    private void GetInput() {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");

        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor() {
        print(Time.deltaTime);
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce * 500 * Time.deltaTime;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce* 500 * Time.deltaTime;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking() {
        
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        if(!echado){
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
        }
        
    }

    private void HandleSteering() {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels() {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform) {
        Vector3 pos;
        Quaternion rot; 
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}

