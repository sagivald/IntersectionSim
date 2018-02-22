//Written by Yossi Cohen <yossicohen2000@gmail.com>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;
    public bool ROSInterface = true;

    public float Timescale = 1,Timestep=0.005f;
    public bool DebugTime = false;
    //Awake is always called before any Start functions
    void Awake()
    {
        
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        if(ROSInterface&&!GetComponent<WebsocketClient>()) gameObject.AddComponent<WebsocketClient>();
    }

    // Use this for initialization
    void Start()
    {
        Time.fixedDeltaTime=Timestep;

    }

    // Update is called once per frame
    void Update()
    {
        if (DebugTime)
        {
            Timescale = Timescale < 0 ? 0 : Timescale;
            Time.timeScale = Timescale;
        }
        if(Input.GetKeyDown(KeyCode.Home)) UnityEngine.SceneManagement.SceneManager.LoadScene("Testing");
    }
private void OnValidate() {
        if (!DebugTime) Time.timeScale = 1;
    
}
}
