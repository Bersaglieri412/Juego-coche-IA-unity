using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.Mathematics;

public class AgenteCoche : Agent
{
    [SerializeField] private ChecksPista checks;
    [SerializeField] private Transform spawn;

    private CarController2 carController;
    private float maxTiempo=60f;
    public float tiempoRestante;
    private float rotacionObj;
    public int nextIndex;
    private int i = 0;
    private int maxAtras = 1;
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
            //AddReward((1f)*((maxTiempo-tiempoRestante)/maxTiempo+1));
            AddReward(100f / checks.CheckPoints.Count);
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
            //AddReward(2f*((maxTiempo-tiempoRestante)/maxTiempo+1));
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
        Vector3 checkpointForward = checks.siguienteCheck(this).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
        var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity);
        sensor.AddObservation(localVel.z);
        /*var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity);
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
        rotacionObj = (carRot.y - checkRot.y);*/
        //AddReward(0.00006f - math.abs(rotacionObj)* 0.00006f);
        //print(0.00006f - math.abs(carRot.y - checkRot.y) * 0.0006f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var input = actions.ContinuousActions;
        var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity); 
        if(localVel.z<=0 && input[1]<0) carController.getInputIa(math.abs(input[1]), input[0]);
        else carController.getInputIa(input[1], input[0]);

        if (localVel.z < 0f && i < maxAtras)
        {
            AddReward(-100f);
            EndEpisode();           
        }
        /*if (localVel.z > 0f && i < maxAtras) AddReward(0.1f*localVel.z);
        if ((rotacionObj < 0.1 && input[0] > 0.1) || (rotacionObj > 0.1 && input[0] < 0.1)) AddReward(0.1f);

        else if (i >= maxAtras)
        {
            AddReward(-1f);
            i = 0;
            EndEpisode();
        }
        
        //AddReward(-0.00006f);*/
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float acelerar = 0;
        if(Input.GetKey(KeyCode.UpArrow)) acelerar= 1;
        if (Input.GetKey(KeyCode.DownArrow)) acelerar = -1;

        float girar = 0;
        if (Input.GetKey(KeyCode.RightArrow)) girar = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) girar = -1;

        ActionSegment<float> discreteActions = actionsOut.ContinuousActions;
        discreteActions[1]=acelerar;
        discreteActions[0]=girar;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro)) AddReward(-15f / checks.CheckPoints.Count);
        
        
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro)) AddReward(-10f / checks.CheckPoints.Count);
    }
}
