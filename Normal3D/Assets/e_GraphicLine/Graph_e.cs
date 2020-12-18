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
    
    // Start is called before the first frame update

    private void Awake()
    {
        float step = 2f/this.resolution;
        var scale = Vector3.one * step;
        var pos = Vector3.zero;
       
        for (int i = 0; i < resolution; i++)
        {
            Transform point = Instantiate(this.pointPrefab);
            pos.x = (i + 0.5f)*step - 1f;;
            pos.y = pos.x * pos.x* pos.x;
            point.position = pos;
            point.localScale = scale;
            point.SetParent(transform,false);
        }
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
