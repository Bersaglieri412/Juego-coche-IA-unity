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
    private float tiempoCircuito;
    private float maxTiempo=4f;
    public float tiempoRestante;
    private float rotacionObj;
    public int nextIndex;
    private int muroi = 0;
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
            AddReward(-50f);
            EndEpisode();
        }
    }
    private void ChecksPista_OnPlayerCorrectCheck(object sender, EventArgs e) {
        ChecksPista.carThroughCheckEventArgs ev= (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform)
        {
            /*
            //AddReward((0.25f)*((maxTiempo-tiempoRestante)/maxTiempo+1));
            AddReward(100f/checks.CheckPoints.Count*(this.maxTiempo/(this.tiempoRestante + this.maxTiempo)));
            */
            AddReward(1f);
            tiempoRestante = maxTiempo;
        }
    }
    private void ChecksPista_OnPlayerWrongCheck(object sender, EventArgs e)
    {

        ChecksPista.carThroughCheckEventArgs ev = (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform) AddReward(-1f);//AddReward((-1f/checks.CheckPoints.Count));
        
    }

    private void ChecksPista_OnPlayerEnd(object sender, EventArgs e)
    {
        ChecksPista.carThroughCheckEventArgs ev = (ChecksPista.carThroughCheckEventArgs)e;
        if (ev.carTransform == transform)
        {
            //AddReward(0.5f*((maxTiempo-tiempoRestante)/maxTiempo+1));
            /*
            AddReward(100f / checks.CheckPoints.Count);
           
            print(GetCumulativeReward()+" "+(100f / checks.CheckPoints.Count));
            EndEpisode();*/
            tiempoRestante = maxTiempo;
            this.nextIndex = 0;
            float tiempoTotal = Time.time - this.tiempoCircuito;
            AddReward(2500f/tiempoTotal);
            print(GetCumulativeReward()+ " Tiempo total: "+ tiempoTotal);
            EndEpisode();
        }

        }

    public override void OnEpisodeBegin()
    {
        transform.position = spawn.position + new Vector3(UnityEngine.Random.Range(-2f, +2f), 0, UnityEngine.Random.Range(-2f, +2f));
        transform.forward= spawn.forward;
        carController.parar();
        nextIndex= 0;
        muroi = 0;
        tiempoRestante = maxTiempo;
        tiempoCircuito = Time.time;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        CheckPointSingle ch = checks.siguienteCheck(this);
        Vector3 checkpointForward = ch.transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
        Quaternion checkRot = ch.GetComponent<Transform>().rotation;
        sensor.AddObservation(checkRot.y);
        Quaternion carRot = this.carController.transform.rotation;
        sensor.AddObservation(carRot.y);
        rotacionObj = (carRot.y - checkRot.y);
        rotacionObj = (carRot.y - checkRot.y);
        AddReward((0.5f - math.abs(rotacionObj))*0.5f);
        //print(carRot.y + " Check " + checkRot.y + "Diff" + math.abs(rotacionObj));
        //var localVel = transform.InverseTransformDirection(carController._rigidbody.velocity);
        //sensor.AddObservation(localVel.z);
        //sensor.AddObservation(this.tiempoRestante);
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
            //AddReward(-1f);
            i = 0;
            //EndEpisode();
        }
        AddReward(-0.25f);
        if (((rotacionObj < 0.1 && input[0] > 0.1) || (rotacionObj > 0.1 && input[0] < 0.1)) && localVel.z > 1) AddReward(0.1f);
            
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

    public void OnTriggerEnter(Collider collision)
    {
        
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro))
        {
            muroi++;
            AddReward(-1.5f);
            print(GetCumulativeReward());
            print("entro");
            if (muroi >= 15)
            {
                muroi = 0;
                AddReward(-30f);
                EndEpisode();

            }
        }
        
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro)) AddReward(-1.5f);
        
    }

/*    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Muro>(out Muro muro))
        {
            AddReward(-1f);
            muroi++;
            if (muroi >= 8)
            {
                muroi = 0;
               // AddReward(-100f);
                //EndEpisode();
            }
        }//AddReward(-20f / checks.CheckPoints.Count);
    }*/
}
