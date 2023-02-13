using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform centerOfMass;
    private Rigidbody _rigidBody;
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking, isHandBreaking;
    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;


    void Start()
    {
        _rigidBody=GetComponent<Rigidbody>();
        _rigidBody.centerOfMass=centerOfMass.localPosition;
        /*frontLeftWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);
        frontRightWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);
        rearLeftWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);
        rearRightWheelCollider.ConfigureVehicleSubsteps(10000, 1, 1);*/
    }

    private void FixedUpdate() {
        GetInput();
        
        HandleSteering();
        UpdateWheels();
        
    }
    private void Update(){

    }
    private void LateUpdate(){
        HandleMotor();
    }

    private void GetInput() {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");

        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.B);

        isHandBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor() {
        print(verticalInput * motorForce  * 500*Time.deltaTime);
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce  *500* Time.deltaTime;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce *500* Time.deltaTime;
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce  *500* Time.deltaTime;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce *500* Time.deltaTime;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
        if (isHandBreaking) frenoMano();
    }

    private void ApplyBreaking() {
        
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
        
    }
    private void frenoMano(){
        rearLeftWheelCollider.brakeTorque = 600f;
        rearRightWheelCollider.brakeTorque = 600f;
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

