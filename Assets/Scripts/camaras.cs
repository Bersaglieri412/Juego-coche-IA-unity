using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camaras : MonoBehaviour
{
    public Camera cam1, cam2;
    public GameObject camCenO, camGenO, camPovO;
    private CinemachineVirtualCamera camCen, camGen, camPov;
    // Start is called before the first frame update
    void Start()
    {
        cam1.enabled = true;
        cam2.enabled = false;
        camCen = camCenO.GetComponent<CinemachineVirtualCamera>();
        camGen = camGenO.GetComponent<CinemachineVirtualCamera>();
        camPov = camPovO.GetComponent<CinemachineVirtualCamera>();
        camPov.enabled = false;
        camCen.enabled = false;
        camGen.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !cam2.enabled) {
            vistaGeneral();
        }
        else if (Input.GetKeyDown(KeyCode.V) && cam2.enabled) {
            vistaNormal();
        }
        if (Input.GetKeyDown(KeyCode.C) && !camCen.enabled) {
            vistaCenital();
        }
        else if (Input.GetKeyDown(KeyCode.C) && camCen.enabled) {
            vistaNormal();
        }

        if(Input.GetKeyDown(KeyCode.P) && !camPov.enabled)
        {
            vistaPOV();
        }
        else if (Input.GetKeyDown(KeyCode.P) && camPov.enabled)
        {
            vistaNormal();
        }

    }
    private void vistaPOV (){
        camPov.enabled = true;
        camGen.enabled = false;
    }
    private void vistaCenital()
    {
        camCen.enabled = true;
        camGen.enabled = false;
    }
    private void vistaGeneral()
    {
        cam2.enabled = true;
        cam1.enabled = false;
    }

    private void vistaNormal() {
        camGen.enabled = true;
        camCen.enabled = false;
        cam2.enabled = false;
        cam1.enabled = true;
    }
}
