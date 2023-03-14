using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.Mathematics;
using UnityEngine.XR;

public class AgenteCoche2 : Agent
{
    [SerializeField] private ChecksPista checks;
    [SerializeField] private Transform spawn;

    private CarController2 carController;
    private float maxTiempo=5f;
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
            AddReward(-10f);
            EndEpisode();
        }
    }
    private void ChecksPista_OnPlayerCorrectCheck(object sender, EventArgs e) {
        ChecksPista.carThroughCheckEventArgs ev= (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform)
        {
            //AddReward((0.25f)*((maxTiempo-tiempoRestante)/maxTiempo+1));
            AddReward(100f/checks.CheckPoints.Count/(this.tiempoRestante/ this.maxTiempo));
            
            tiempoRestante = maxTiempo;
        }
    }
    private void ChecksPista_OnPlayerWrongCheck(object sender, EventArgs e)
    {

        ChecksPista.carThroughCheckEventArgs ev = (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform) AddReward((-1f/checks.CheckPoints.Count));
        
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
        Vector3 checkpointForward = checks.siguienteCheck(this).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
        var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity);
        sensor.AddObservation(localVel.z);
        /*
        sensor.AddObservation(ObserveRay(1.5f, .5f, 25f)); // FrontR
        sensor.AddObservation(ObserveRay(1.5f, 0f, 0f)); // Front
        sensor.AddObservation(ObserveRay(1.5f, -.5f, -25f)); // FrontL
        sensor.AddObservation(ObserveRay(-1.5f, 0, 180f)); // Back
        */
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var input = actions.ContinuousActions;
        var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity);
        carController.getInputIa(input[1], input[0]);
        if (localVel.z < -0f && i< maxAtras) i++;

        else if (i >= maxAtras)
        {
            AddReward(-10f);
            i = 0;
            EndEpisode();
        }
        
        //AddReward(-1/60f);
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
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro)) AddReward(-20f / checks.CheckPoints.Count);
    }
}
