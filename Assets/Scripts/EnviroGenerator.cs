using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//ma wygenerować mapę opisaną liczbami i wystawić jako tablicę map3d

public class EnviroGenerator : MonoBehaviour {

    GetterFinder gF;

    //short[,,] map; //mapa taka jak w excellu [arkusz, x, y], wartość to wysokość, minus oznacza przeszkodę, zero to null
    public const int size = 1000; //1500; //przy 100 ogromnie zamula; spróbować combine mesh
    //public short[,,] map3d; //mapa jak na scenie [x, y, z], gdzie y to wysokość, wartość mówi co tam jest: 1 blok, 2 przeszkoda

    //zastępuje map3d
    //public MapLayered2DItem[,] mapLayered2D;    

    public struct MapLayered2DItem
    {
        public short h; //wysokość nad poziomem morza
        public byte flavor; //rodzaj bloka: 0 - brak, 1 - kamień
        //public bool instanced; //czy ma przypisany game object blok //może zastąpić tym, że wszystko w view box ma być instanced, a poza nie instanced
        public byte depth; //ilość bloków pod spodem, aby nie było dziur
        public MapLayered2DItem(short _h, byte _flavor,/*, bool _instanced*/ byte _depth)
        {
            h = _h;
            flavor = _flavor;
            //instanced = _instanced;
            depth = _depth;
        }
    }

    public static MapLayered2DItem[,] DeliverLayered2DMap(int sheet)
    {
        return MakeMapLayered2D(MakeMap(), sheet);
    }

    static MapLayered2DItem[,] MakeMapLayered2D(short[,,] m, int sheet)
    {
        //07.04 11:40 przepisanie wszystkiego do nowej struktury danych bez żadnych zmian funkcjonalności
        MapLayered2DItem[,] mapLayered2D = new MapLayered2DItem[size, size];

        int y = 0;
        for(int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                //map layered 2 d ma współrzędne jak scena, y to wysokość; map ma współrzędne jak excell, jej y odpowiada z sceny
                y = z;
                //jeżeli ma tam nic nie być
                if (m[sheet, x, y] == 0)
                {
                    mapLayered2D[x, z] = new MapLayered2DItem(0, 0, 0);
                }
                //ścieżka - warstwa terenu grubości 2
                if (m[sheet, x, y] > 0)
                {
                    mapLayered2D[x, z] = new MapLayered2DItem((short)(m[sheet, x, y] - 1), 1, 4);
                }
                //przeszkoda stojąca na obniżonym o 1, równym lub podniesionym o 1
                if (m[sheet, x, y] < 0)
                {
                    int r = 0;
                    r = UnityEngine.Random.Range(0, 2);

                    mapLayered2D[x, z] = new MapLayered2DItem((short)Mathf.Abs(m[sheet, x, y] - 2 + r), 2, 4);
                }
            }
        }

        //zabezpieczanie indeksów
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if(mapLayered2D[x, z].h >= size)
                {
                    mapLayered2D[x, z].h = size - 1;
                }
                if(mapLayered2D[x, z].h + 1 - mapLayered2D[x, z].depth < 0)
                {
                    mapLayered2D[x, z].depth = (byte)(mapLayered2D[x, z].h + 1);
                }
            }
        }

        return mapLayered2D;
    }



    static short[,,] MakeMap()
    {
        short[,,] map = new short[9, size, size];

        int eX;
        int eY;

        //sheet1
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                //różnice w indeksacji c# a excell
                eX = x + 1;
                eY = y + 1;
                if (eX + eY < 7) { map[0, x, y] = 1; } else { map[0, x, y] = (short)(eX + eY - 5); }
            }
        }

        //sheet2 z generatora - wyznacza korytarz po przekątnej
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                eX = x + 1;
                eY = y + 1;
                if ((eX - eY < 12) && (eX - eY) > -12) { map[1, x, y] = 1; } else { map[1, x, y] = 0; }
            }
        }

        short v1 = 0;
        short v2 = 0;

        //sheet3
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                eX = x + 1;
                eY = y + 1;
                if ((eX - eY >= 7) && (eX - eY < 12)) { v1 = -1; } else { v1 = 1; }
                if ((eX - eY <= -7) && (eX - eY > -12)) { v2 = -1; } else { v2 = 1; }
                map[2, x, y] = (short)(v1 * v2);
            }
        }

        //sheet2a
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                eX = x + 1;
                eY = y + 1;
                if ((eX - eY < 5) && (eX - eY) > -5) { map[3, x, y] = 1; } else { map[3, x, y] = 0; }
            }
        }

        //uwaga na błędy wynikające z indeksacji - w excellu od 1

        //sheet5 - generowanie ścieżki "1"
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                eX = x + 1;
                eY = y + 1;

                //puste miejsca i jedynki
                if ((eX <= 2) || (eY <= 3))
                {
                    if (eX + eY < 7) { map[4, x, y] = 1; }
                    else { map[4, x, y] = 0; }
                }
                else
                //komórka wyjściowa
                if ((eX == 3) && (eY == 4)) { map[4, x, y] = (short)UnityEngine.Random.Range(1, 3); }
                else
                //reszta górnego wiersza
                if ((eX > 3) && (eY == 4))
                {
                    //d4 = IF(AND(C4=1; Sheet2a!D4>0);RANDBETWEEN(1; 2); 0)
                    if (map[4, x - 1, y] == 1)
                    {
                        if (map[3, x, y] > 0)
                        {
                            map[4, x, y] = (short)UnityEngine.Random.Range(1, 3);
                        }
                        else { map[4, x, y] = 2; }
                    }
                    else
                    { map[4, x, y] = 0; }
                }
                else
                //reszta trzeciej kolumny
                if ((eX == 3) && (eY > 4))
                {
                    if (map[4, x, y - 1] == 2)
                    {
                        if (map[3, x, y] > 0)
                        {
                            map[4, x, y] = (short)UnityEngine.Random.Range(1, 3);
                        }
                        else { map[4, x, y] = 1; }
                    }
                    else
                    { map[4, x, y] = 0; }
                }
                else
                //właściwe mięcho eX > 3 && eY > 4
                {
                    //d5 = IF(C5 = 1; IF(Sheet2a!D5 > 0; RANDBETWEEN(1; 2); 2); 0) +IF(D4 = 2; IF(Sheet2a!D5 > 0; RANDBETWEEN(1; 2); 1); 0)

                    if (map[4, x - 1, y] == 1)
                    {
                        if (map[3, x, y] > 0) { v1 = (short)UnityEngine.Random.Range(1, 3); }
                        else
                        { v1 = 2; }
                    }
                    else
                    { v1 = 0; }

                    if (map[4, x, y - 1] == 2)
                    {
                        if (map[3, x, y] > 0) { v2 = (short)UnityEngine.Random.Range(1, 3); }
                        else
                        { v2 = 1; }
                    }
                    else
                    { v2 = 0; }

                    map[4, x, y] = (short)(v1 + v2);
                }

            }
        }

        //sheet6 - generowanie ścieżki "2"
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                eX = x + 1;
                eY = y + 1;

                //puste miejsca i jedynki
                if ((eX <= 3) || (eY <= 2))
                {
                    if (eX + eY < 7) { map[5, x, y] = 1; }
                    else { map[5, x, y] = 0; }
                }
                else
                //komórka wyjściowa
                if ((eX == 4) && (eY == 3)) { map[5, x, y] = (short)UnityEngine.Random.Range(1, 3); }
                else
                //reszta górnego wiersza
                if ((eX > 4) && (eY == 3))
                {
                    //d4 = IF(AND(C4=1; Sheet2a!D4>0);RANDBETWEEN(1; 2); 0)
                    if (map[5, x - 1, y] == 1)
                    {
                        if (map[3, x, y] > 0)
                        {
                            map[5, x, y] = (short)UnityEngine.Random.Range(1, 3);
                        }
                        else { map[5, x, y] = 2; }
                    }
                    else
                    { map[5, x, y] = 0; }
                }
                else
                //reszta trzeciej kolumny
                if ((eX == 4) && (eY > 3))
                {
                    if (map[5, x, y - 1] == 2)
                    {
                        if (map[3, x, y] > 0)
                        {
                            map[5, x, y] = (short)UnityEngine.Random.Range(1, 3);
                        }
                        else { map[5, x, y] = 1; }
                    }
                    else
                    { map[5, x, y] = 0; }
                }
                else
                //właściwe mięcho
                {
                    //d5 = IF(C5 = 1; IF(Sheet2a!D5 > 0; RANDBETWEEN(1; 2); 2); 0) +IF(D4 = 2; IF(Sheet2a!D5 > 0; RANDBETWEEN(1; 2); 1); 0)

                    if (map[5, x - 1, y] == 1)
                    {
                        if (map[3, x, y] > 0) { v1 = (short)UnityEngine.Random.Range(1, 3); }
                        else
                        { v1 = 2; } //zmień kierunek
                    }
                    else
                    { v1 = 0; }

                    if (map[5, x, y - 1] == 2)
                    {
                        if (map[3, x, y] > 0) { v2 = (short)UnityEngine.Random.Range(1, 3); }
                        else
                        { v2 = 1; } //zmień kierunek
                    }
                    else
                    { v2 = 0; }

                    map[5, x, y] = (short)(v1 + v2);
                }

            }
        }

        //sheet 7
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (map[4, x, y] + map[5, x, y] > 0) { map[6, x, y] = 1; } else { map[6, x, y] = -1; }
            }
        }

        //preFinal1
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if ((map[2, x, y] < 0) && (map[6, x, y] < 0))
                {
                    v2 = (short)(-1 * map[2, x, y] * map[6, x, y]);
                }
                else { v2 = (short)(map[2, x, y] * map[6, x, y]); }

                v1 = (short)(map[0, x, y] * map[1, x, y]);

                map[7, x, y] = (short)(v1 * v2);
            }
        }

        //Final
        bool b;
        int p;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {


                if (map[7, x, y] < 0)
                {
                    b = false;
                    //czy ma choć jednego dodatniego sąsiada
                    if (x > 0) { if (map[7, x - 1, y] > 0) { b = true; } }
                    if (y > 0) { if (map[7, x, y - 1] > 0) { b = true; } }
                    if (x < size - 1) { if (map[7, x + 1, y] > 0) { b = true; } }
                    if (y < size - 1) { if (map[7, x, y + 1] > 0) { b = true; } }

                    //daj mu szansę zmienić znak
                    if (b)
                    {
                        //tym mniejszą, im mniejsza wysokość
                        p = UnityEngine.Random.Range(0, Mathf.Abs(map[7, x, y]) + 500);
                        if (p < 250) { map[8, x, y] = (short)Mathf.Abs(map[7, x, y]); }
                        else { map[8, x, y] = map[7, x, y]; }
                    }
                    else
                    { map[8, x, y] = map[7, x, y]; }

                }
                else { map[8, x, y] = map[7, x, y]; }
            }
        }

        //dodatkowa korekta 07.04.2018: niech wszystko będzie o 5 wyżej
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if(map[8, x, y] < 0)
                {
                    map[8, x, y] -= 5;
                }
                else
                    if(map[8, x, y] > 0)
                {
                    map[8, x, y] += 5;
                }
            }
        }

        return map;
    }
    
    
    
    void Awake()
    {
        
        //MakeMap3d();
        //MakeMapLayered2D(MakeMap(), 8);

        /*
        string s = "";
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                s += "P(" + x.ToString() + "," + y.ToString() + ")=" + map[8, x, y].ToString() + "; ";
            }
            s += Environment.NewLine;
        }
        //Debug.Log(s);
        */
        

    }

    // Update is called once per frame
    void Update () {
		
	}
}
