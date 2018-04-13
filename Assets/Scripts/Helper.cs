using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper {

    //public 

    public struct IntVector3
    {
        public int x;
        public int y;
        public int z;
        public IntVector3(Vector3 v)
        {
            x = Mathf.RoundToInt(v.x);
            y = Mathf.RoundToInt(v.y);
            z = Mathf.RoundToInt(v.z);
        }
        public IntVector3(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
        public override string ToString()
        {
            string r = x.ToString() + "," + y.ToString() + "," + z.ToString();
            return r;
        }
    }

    

}
