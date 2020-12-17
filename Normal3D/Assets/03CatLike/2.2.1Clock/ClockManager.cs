using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    [SerializeField] private Transform hourPivot, minutePivot, secondPivot;

    private const float hoursToDegrees = -30f, secondToDegrees = -6f, minuteToDegrees = -6f;

    private Button Button_Back;
    private void Awake()
    {
        Button_Back = GameObject.Find("Button_Back").GetComponent<Button>();
        BackToMain();
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        var curTime = DateTime.Now.TimeOfDay;
        this.hourPivot.localRotation =Quaternion.Euler(0,   0, hoursToDegrees* (float)curTime.TotalHours); 
        this.minutePivot.localRotation =Quaternion.Euler(0, 0, minuteToDegrees* (float)curTime.TotalMinutes); 
        this.secondPivot.localRotation =Quaternion.Euler(0, 0, secondToDegrees* (float)curTime.TotalSeconds); 
    }
    
    void BackToMain()
    {
        Button_Back.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(0);
        });
    }
}
