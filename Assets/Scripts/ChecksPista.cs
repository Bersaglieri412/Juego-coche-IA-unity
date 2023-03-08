using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class ChecksPista : MonoBehaviour
{
    public event EventHandler OnPlayerCorrectCheck;
    public event EventHandler OnPlayerWrongCheck;
    public event EventHandler OnPlayerEnd;//Temporalmente se conveirte en evento de terminar circuito
    public List<CheckPointSingle> CheckPoints;
    void Awake()
    {
        CheckPoints = new List<CheckPointSingle>();
        Transform checks = transform.Find("Checkpoints");
        
        foreach (Transform check in checks) 
        { 
            CheckPointSingle check1= check.GetComponent<CheckPointSingle>();
            check1.setPista(this); 
            CheckPoints.Add(check1);
        }
    }

    public void carThroughCheck(carThroughCheckEventArgs check) {
        AgenteCoche c= check.carTransform.GetComponentInParent<AgenteCoche>(); //Temporal
        if (CheckPoints.IndexOf(check.check) != c.nextIndex)
        {
            OnPlayerWrongCheck?.Invoke(this, check);
            return;
        }
        c.nextIndex++;
        //print(nextIndex + " ahora:" + CheckPoints.Count);
        if (c.nextIndex < CheckPoints.Count)
        {
            OnPlayerCorrectCheck?.Invoke(this, new carThroughCheckEventArgs(check.check, check.carTransform));
        }
        else
        {
            OnPlayerEnd?.Invoke(this, check);
        }
    }

    public CheckPointSingle siguienteCheck(AgenteCoche c)
    {
        if(c.nextIndex<CheckPoints.Count) return CheckPoints[c.nextIndex];
        else return CheckPoints[c.nextIndex-1];
    }
    public CheckPointSingle siguienteCheck(AgenteCoche1 c)
    {
        if (c.nextIndex < CheckPoints.Count) return CheckPoints[c.nextIndex];
        else return CheckPoints[c.nextIndex - 1];
    }

    public class carThroughCheckEventArgs : EventArgs
    {
        public carThroughCheckEventArgs(CheckPointSingle check, Transform carTransform)
        {
            this.check = check;
            this.carTransform = carTransform;
        }

        public CheckPointSingle check { get; set; }
        public Transform carTransform { get; set; }
    }


}
