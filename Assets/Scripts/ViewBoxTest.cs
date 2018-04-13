using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBoxTest : ViewBox {

    GetterFinder testGF;
    public static EnviroGenerator.MapLayered2DItem[,] testMapLayered2D;

    public GameObject debugFrame;

    Vector3 DFPos()
    {
        return new Vector3(
            (NearEdge(center, d).x + FarEdge(center, d).x - 1.0f) / 2.0f,
            (NearEdge(center, d).y + FarEdge(center, d).y - 1.0f) / 2.0f,
            (NearEdge(center, d).z + FarEdge(center, d).z - 1.0f) / 2.0f);            
    }

    void AdjustFrame(GameObject frame)
    {
        frame.transform.position = DFPos();
        frame.transform.localScale = new Vector3(d, d, d);
    }

	// Use this for initialization
	void Start () {

        //Debug.Log(NearEdge(new Helper.IntVector3(15, 15, 15), 7).ToString());
        //Debug.Log(FarEdge(new Helper.IntVector3(15, 15, 15), 7).ToString());

        center = new Helper.IntVector3(this.transform.position);
        testMapLayered2D = EnviroGenerator.DeliverLayered2DMap(0);

        testGF = GameObject.FindGameObjectWithTag("Config").GetComponent<GetterFinder>();


        Debug.Log(NearEdge(center, d).ToString() + ", " + FarEdge(center, d).ToString());

        SpawnAll(testMapLayered2D);
    }
	
	// Update is called once per frame
	void Update () {

        AdjustFrame(debugFrame);
		
	}
}
