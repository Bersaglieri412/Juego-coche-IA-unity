using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public inputController controller { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        controller = GetComponentInChildren<inputController>();
    }
}
