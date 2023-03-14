using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.Mathematics;

public class AgenteCoche1 : Agent
{
    [SerializeField] private ChecksPista checks;
    [SerializeField] private Transform spawn;

    private CarController2 carController;
    private float maxTiempo=30f;
    public float tiempoRestante;
    private float rotacionObj;
    public int nextIndex;
    private int i = 0;
    private int maxAtras = 50;
    // Start is called before the first frame update

    private void Awake()
    {
        carController= GetComponent<CarController2>();
        tiempoRestante = maxTiempo;
    }
    private void Start()
    {
        checks.OnPlayerCorrectCheck += ChecksPista_OnPlayerCorrectCheck;
        checks.OnPlayerWrongCheck += ChecksPista_OnPlayerWrongCheck;
        checks.OnPlayerEnd += ChecksPista_OnPlayerEnd;
    }
    private void Update()
    {
        this.tiempoRestante -= Time.deltaTime;
        if (this.tiempoRestante <= 0)
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }
    private void ChecksPista_OnPlayerCorrectCheck(object sender, EventArgs e) {
        ChecksPista.carThroughCheckEventArgs ev= (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform)
        {
            //AddReward((0.25f)*((maxTiempo-tiempoRestante)/maxTiempo+1));
            AddReward(100f/checks.CheckPoints.Count);
            tiempoRestante = maxTiempo;
        }
    }
    private void ChecksPista_OnPlayerWrongCheck(object sender, EventArgs e)
    {

        ChecksPista.carThroughCheckEventArgs ev = (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform) { }// AddReward((-0.5f/(nextIndex+1)) / checks.CheckPoints.Count);
        
    }

    private void ChecksPista_OnPlayerEnd(object sender, EventArgs e)
    {
        ChecksPista.carThroughCheckEventArgs ev = (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform)
        {
            //AddReward(0.5f*((maxTiempo-tiempoRestante)/maxTiempo+1));
            AddReward(100f / checks.CheckPoints.Count);
            tiempoRestante = maxTiempo;
            print(GetCumulativeReward()+" "+(100f / checks.CheckPoints.Count));
            EndEpisode();
        }

        }

    public override void OnEpisodeBegin()
    {
        transform.position = spawn.position + new Vector3(UnityEngine.Random.Range(-2f, +2f), 0, UnityEngine.Random.Range(-2f, +2f));
        transform.forward= spawn.forward;
        carController.parar();
        nextIndex= 0;
        tiempoRestante = maxTiempo;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity);
        sensor.AddObservation(localVel.z);
        CheckPointSingle ch = checks.siguienteCheck(this);
        Vector3 diff = ch.transform.position-transform.position;
        sensor.AddObservation(diff/20f);
        Vector3 checkpointForward = ch.GetComponent<Transform>().forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
        Quaternion checkRot = ch.GetComponent<Transform>().rotation;
        sensor.AddObservation(checkRot.y);
        Quaternion carRot = this.carController.transform.rotation;  
        sensor.AddObservation(carRot.y);
        rotacionObj = (carRot.y - checkRot.y);
        AddReward(0.00006f - math.abs(rotacionObj)* 0.00006f);
        //print(0.00006f - math.abs(carRot.y - checkRot.y) * 0.0006f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;
        var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity);
        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +1f; break;
            case 2: forwardAmount = -1f; break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +1f; break;
            case 2: turnAmount = -1f; break;
        }

        carController.getInputIa(forwardAmount, turnAmount);
        if (localVel.z < -0.9f && i< maxAtras) i++;
        if (rotacionObj < 0.1 && turnAmount > 0 || rotacionObj > 0.1 && turnAmount < 0) AddReward(0.01f);

        else if (i >= maxAtras)
        {
            AddReward(-1f);
            i = 0;
            EndEpisode();
        }
        
        AddReward(-0.00006f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int acelerar = 0;
        if(Input.GetKey(KeyCode.UpArrow)) acelerar= 1;
        if (Input.GetKey(KeyCode.DownArrow)) acelerar = -1;

        int girar = 0;
        if (Input.GetKey(KeyCode.RightArrow)) girar = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) girar = -1;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[1]=acelerar;
        discreteActions[0]=girar;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro)) { }// AddReward(-0.001f / checks.CheckPoints.Count);
        
        
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro)) { }//AddReward(-0.0001f / checks.CheckPoints.Count);
    }
}
