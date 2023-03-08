using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSingle : MonoBehaviour
{
    public ChecksPista pista;
    private void OnTriggerEnter(Collider other)
    {
        ChecksPista.carThroughCheckEventArgs e = new ChecksPista.carThroughCheckEventArgs(this, other.transform);
        if (other.TryGetComponent<CarController2>(out CarController2 car)) pista.carThroughCheck(e);
    }
    public void setPista(ChecksPista pista)
    {
        this.pista = pista;
    }
}   
