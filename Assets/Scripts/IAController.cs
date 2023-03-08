using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : MonoBehaviour
{

    public float inputWheel { get; private set; }
    public float inputThrottle { get; private set; }
    void Start()
    {

    }

    public void GetInputAcelerar(float acelerar)
    {
        inputThrottle = acelerar;
    }

    public void getInputGirar(float giro)
    {
        inputWheel= giro;
    }
}
