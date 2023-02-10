using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camaras : MonoBehaviour
{
    public Camera cam1;
    public Camera cam2;
    // Start is called before the first frame update
    void Start()
    {
        cam1.enabled = true;
        cam2.enabled = false;    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !cam2.enabled) {
         cam2.enabled=true;
     }
        else if(Input.GetKeyDown(KeyCode.V) && cam2.enabled){
            cam2.enabled=false;
        }
    }
}
