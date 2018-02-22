using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public int ID = 0;
    public int state = 0;
    public GameObject[] Lights;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Lights.Length; i++)
        {
            if (i == state) Lights[i].SetActive(true);
            else Lights[i].SetActive(false);
        }
    }
}
