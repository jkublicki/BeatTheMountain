using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test1 : MonoBehaviour {

    public GameObject interactive;
    ViewBox vB;

    int moveCount = 0;

    void MoveVBCenter()
    {
        //Debug.Log("center: " + vB.center.ToString());
        if (moveCount % 2.0f == 0)
        {
            interactive.transform.position = new Vector3(interactive.transform.position.x + 1.0f, 
                interactive.transform.position.y + 1.0f, interactive.transform.position.z);
            
        }
        else
        {
            interactive.transform.position = new Vector3(interactive.transform.position.x,
                interactive.transform.position.y + 1.0f, interactive.transform.position.z + 1.0f);
        }

        moveCount++;

        vB.OnViewBoxMove(ViewBoxTest.testMapLayered2D);

        StoneCount();
        
    }

    void StoneCount()
    {
        GameObject[] getCount = GameObject.FindGameObjectsWithTag("DebugStone");
        Debug.Log("stone count: " + getCount.Length.ToString());
    }

	// Use this for initialization
	void Start () {

        vB = interactive.GetComponent<ViewBox>();


        InvokeRepeating("MoveVBCenter", 5.0f, 1.0f);


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
