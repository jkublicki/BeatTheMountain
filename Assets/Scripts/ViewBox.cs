using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to ma być nowy enviro manager, przy czym całe enviro ma być ograniczone do view box
//view box jest nieco większy niż obszar widziany przez kamerę
public class ViewBox : MonoBehaviour {

    GetterFinder gF;
    EnviroGenerator.MapLayered2DItem[,] mapLayered2D;

    //spawnowanie bloków
    //przenoszenie bloków
    //lista zapasowych bloków - czyszczona aby nie była za duża

    //wielkość view boxa
    public const //do testów
    int d = 10;

    //środek view boxa
    public Helper.IntVector3 center;
    Helper.IntVector3 previousCenter;

    //zbiór informacji o instancjach, pary IntVector3 adres i GameObject blok
    Hashtable stones = new Hashtable(); //problem? były warningi, teraz nie ma..
    Hashtable obstacles = new Hashtable(); //jw
    List<GameObject> stonePool = new List<GameObject>(); //jw
    List<GameObject> obstaclePool = new List<GameObject>(); //jw

    void SpawnBlock(Helper.IntVector3 pos)
    {
        if(stones[pos] == null)
        {
            stones.Add(pos, Instantiate(gF.standardStone, pos.ToVector3(), gF.standardStone.transform.rotation, gF.standardStone.transform.parent));
        }
    }

    void SpawnObstacle(Helper.IntVector3 pos)
    {
        if (obstacles[pos] == null)
        {
            obstacles.Add(pos, Instantiate(gF.standardObstacle, pos.ToVector3(), gF.standardObstacle.transform.rotation, gF.standardObstacle.transform.parent));
        }
    }

    public //do testów
    void SpawnAll(EnviroGenerator.MapLayered2DItem[,] _mapLayered2D)
    {
        for(int x = 0; x < d; x++)
        {
            for(int z = 0; z < d; z++)
            {
                if(_mapLayered2D[x, z].flavor == 1)
                {
                    //spawnowanie kilku, zgodnie z depth określającą grubość warstwy bloków pod wierzchnim blokiem
                    for(int d = 0; d < _mapLayered2D[x, z].depth; d++)
                    {
                        SpawnBlock(new Helper.IntVector3(x, _mapLayered2D[x, z].h - d, z));
                    }
                }
                else
                    if (_mapLayered2D[x, z].flavor == 2)
                {
                    SpawnObstacle(new Helper.IntVector3(x, _mapLayered2D[x, z].h, z));
                    //pod obstacle mają być bloko
                    for (int d = 1; d < _mapLayered2D[x, z].depth; d++)
                    {
                        SpawnBlock(new Helper.IntVector3(x, _mapLayered2D[x, z].h - d, z));
                    }
                }
            }
        }
    }

    int Flavor(Helper.IntVector3 pos, EnviroGenerator.MapLayered2DItem[,] _mapLayered2D)
    {
        int r = -1;
        int h = _mapLayered2D[pos.x, pos.z].h;
        int depth = _mapLayered2D[pos.x, pos.z].depth;

        if(pos.y == h)
        {
            r = _mapLayered2D[pos.x, pos.z].flavor;
        }

        if((pos.y < h) && (pos.y > h - depth))
        {
            r = 1; //kamienie pod przeszkodami
        }        

        return r;
    }

    public void OnViewBoxMove(EnviroGenerator.MapLayered2DItem[,] _mapLayered2D)
    {
        List<Helper.IntVector3> abandoned = new List<Helper.IntVector3>();
        List<Helper.IntVector3> gained = new List<Helper.IntVector3>();

        previousCenter = center;
        Helper.IntVector3 p0 = NearEdge(center, d); //?? tu nie ma błędów?

        center = new Helper.IntVector3(this.transform.position);
        Helper.IntVector3 p1 = NearEdge(center, d);

        abandoned = AbandonedOrGainedPoints(p0, p1, d, 0);
        gained = AbandonedOrGainedPoints(p0, p1, d, 1);

        //test
        /*
        string s1 = "abandoned: ";
        foreach(Helper.IntVector3 p in abandoned)
        {
            s1 += p.ToString() + ", ";
        }
        string s2 = "gained: ";
        foreach (Helper.IntVector3 p in gained)
        {
            s2 += p.ToString() + ", ";
        }
        Debug.Log(s1);
        Debug.Log(s2);
        */

        //test

        Debug.Log("stone pool count:" + stonePool.Count.ToString());



        foreach (Helper.IntVector3 a in abandoned)
        {
            //jeśli w tym opuszczonym punkcie był kamień
            if(stones[a] != null)
            {
                //wyjeb do poola
                if(stonePool.Count < 50)
                {
                    stonePool.Add((GameObject)stones[a]);                    
                }
                else
                {
                    Destroy((GameObject)stones[a]);
                }                
                stones.Remove(a);
            }

            //analogicznie obstacles...
            if (obstacles[a] != null)
            {
                //wyjeb do poola
                if(obstaclePool.Count < 50)
                {
                    obstaclePool.Add((GameObject)obstacles[a]);
                }
                else
                {
                    Destroy((GameObject)obstacles[a]);
                }
                obstacles.Remove(a);
            }
        }

        //test
        int movedDebug = 0;
        int instancedDebug = 0;

        //tu kontynuować debugowanie po 12.04
        foreach (Helper.IntVector3 g in gained)
        {
            //jeśli tam ma być kamień
            if(Flavor(g, _mapLayered2D) == 1)
            {
                //jeśli jest w pool, weż stamtąd
                if(stonePool.Count > 0)
                {
                    stonePool[0].transform.position = g.ToVector3();                    //w tych klamrach coś jest nie tak 12.04
                    //gained.Remove(g);
                    stones.Remove(g);
                    stones.Add(g, stonePool[0]);
                    stonePool.RemoveAt(0);

                    movedDebug++;
                }
                else
                //albo zinstancjonuj
                {
                    SpawnBlock(g);


                    instancedDebug++;
                }                
            }

            //jeśli tam ma być przeszkoda
            if (Flavor(g, _mapLayered2D) == 2)
            {
                //jeśli jest w pool, weż stamtąd
                if (obstaclePool.Count > 0)
                {
                    obstaclePool[0].transform.position = g.ToVector3();
                    //gained.Remove(g);
                    obstacles.Remove(g);
                    obstacles.Add(g, obstaclePool[0]);
                    obstaclePool.RemoveAt(0);
                }
                else
                //albo zinstancjonuj
                {
                    SpawnObstacle(g);
                    //gained.Remove(g);
                }
            }

            

        }

        Debug.Log("moved: " + movedDebug.ToString() + ", instanced: " + instancedDebug.ToString() + ", pool count: " + obstaclePool.Count.ToString());




    }


    //punkt na rogu najbliższym środka układu współrzędnych
    public //do testów
    Helper.IntVector3 NearEdge(Helper.IntVector3 center, int d)
    {
        Helper.IntVector3 r = new Helper.IntVector3(center.x - Mathf.RoundToInt(d / 2.0f), center.y - Mathf.RoundToInt(d / 2.0f), center.y - Mathf.RoundToInt(d / 2.0f));
        return r;
    }

    //punkt na rogu najdalszym od środka układu współrzędnych
    public //do testów
    Helper.IntVector3 FarEdge(Helper.IntVector3 center, int d)
    {
        Helper.IntVector3 p = NearEdge(center, d);
        Helper.IntVector3 r = new Helper.IntVector3(p.x + d, p.y + d, p.z + d);
        return r;
    }

    //listuje punkty opuszczone albo nabyte po ruchu view boxa
    //mode = 0 - abandoned, 1 - gained; d - box size; p0, p1 - bottom left edge, old and new
    public //do testów
        List<Helper.IntVector3> AbandonedOrGainedPoints(Helper.IntVector3 p0, Helper.IntVector3 p1, int d, int mode)
    {
        List<Helper.IntVector3> abandoned = new List<Helper.IntVector3>();
        List<Helper.IntVector3> gained = new List<Helper.IntVector3>();

        if ((p0.x < 0) || (p0.y < 0) || (p0.z < 0) || (p1.x < 0) || (p1.y < 0) || (p1.z < 0))
        {
            Debug.Log("AbandonedPoints(): Nie obsługuję ujemnych współrzędnych");
            return null;
        }

        Helper.IntVector3 pA = new Helper.IntVector3(Mathf.Min(p0.x, p1.x), Mathf.Min(p0.y, p1.y), Mathf.Min(p0.z, p1.z));
        Helper.IntVector3 pB = new Helper.IntVector3(Mathf.Max(p0.x + d, p1.x + d), Mathf.Max(p0.y + d, p1.y + d), Mathf.Max(p0.z + d, p1.z + d));

        bool b0 = true;
        bool b1 = true;

        for (int x = pA.x; x <= pB.x; x++)
        {
            for (int y = pA.y; y <= pB.y; y++)
            {
                for (int z = pA.z; z <= pB.z; z++)
                {
                    //jeżeli punkty należy do kostki 0
                    if ((p0.x <= x) && (x < p0.x + d) && (p0.y <= y) && (y < p0.y + d) && (p0.z <= z) && (z < p0.z + d))
                    {
                        b0 = true;
                    }
                    else
                    {
                        b0 = false;
                    }
                    //jeżeli punkty należy do kostki 1
                    if ((p1.x <= x) && (x < p1.x + d) && (p1.y <= y) && (y < p1.y + d) && (p1.z <= z) && (z < p1.z + d))
                    {
                        b1 = true;
                    }
                    else
                    {
                        b1 = false;
                    }

                    if (b0 & !b1)
                    {
                        abandoned.Add(new Helper.IntVector3(x, y, z));
                    }
                    else
                        if (!b0 & b1)
                    {
                        gained.Add(new Helper.IntVector3(x, y, z));
                    }
                }
            }
        }

        if (mode == 0)
        {
            return abandoned;
        }
        else
        {
            return gained;
        }
    }


    private void Awake()
    {
        center = new Helper.IntVector3(this.transform.position);
        mapLayered2D = EnviroGenerator.DeliverLayered2DMap(8);
        gF = GameObject.FindGameObjectWithTag("Config").GetComponent<GetterFinder>();
    }


    // Use this for initialization
    void Start () {

        


        //SpawnAll();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

