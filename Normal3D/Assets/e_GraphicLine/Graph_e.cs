using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph_e : MonoBehaviour
{
    [SerializeField]
    private Transform pointPrefab;
    
    [SerializeField,Range(10,100)]
    private int resolution = 10;

    [SerializeField] private FunctionLibrary.FunctionName functionName = default;
    // Start is called before the first frame update
    private Transform[] points;
    private void Awake()
    {
        float step = 2f/this.resolution;
        var scale = Vector3.one * step;
        // var pos = Vector3.zero;
         this.points = new Transform [resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {
            // if (x == this.resolution)
            // {
            //     x = 0;
            //     z += 1;
            // }
            Transform point = Instantiate(this.pointPrefab);
            // pos.x = (x + 0.5f)*step - 1f;;
            // pos.z = (z + 0.5f)*step - 1f;;
            // //pos.y = pos.x * pos.x* pos.x;
            // point.position = pos;
            point.localScale = scale;
            point.SetParent(transform,false);
            points[i] = point;
        }
    }

    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        float _time = Time.time;
        float step = 2f/resolution;
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(this.functionName);
        float v = 0.5f * step - 1f;
        for (int i = 0,x=0,z=0; i < this.points.Length; i++,x++)
        {
            if (x == this.resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, _time);

        }
    }
}
