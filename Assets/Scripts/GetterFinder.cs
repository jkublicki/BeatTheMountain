using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetterFinder : MonoBehaviour {

    public GameObject enviro;
    public GameObject interactive;
    [HideInInspector]
    public GameObject climber;
    [HideInInspector]
    public Camera camera1;
    public GameObject standardStone;
    public GameObject standardObstacle;
    [HideInInspector]
    public EnviroGenerator enviroGeneratorScript;
    //[HideInInspector]
    //public EnviroManager enviroManagerScript;
    //[HideInInspector]
    //public Climber climberScript;


    private void Awake()
    {
        climber = interactive.transform.Find("Climber").gameObject;
        //camera1 = climber.transform.Find("Camera1").GetComponent<Camera>();
        enviroGeneratorScript = enviro.GetComponent<EnviroGenerator>();
        //enviroManagerScript = enviro.GetComponent<EnviroManager>();
        //climberScript = climber.GetComponent<Climber>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
