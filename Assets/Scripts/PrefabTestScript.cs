using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.parent = this.transform;
        g.transform.position = new Vector3(0, 1, 0);
        GameObject e = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        e.transform.parent = this.transform;
        e.transform.position = new Vector3(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
