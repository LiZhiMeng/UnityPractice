using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class CarDrive_BtnControl : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler,IMoveHandler
{
    

    private string[] ControlType = {"LeftButton", "RightButton", "UpButton", "DownButton"};
    enum ControlTypeEnum 
    {
        Left,
        Right,
        Up,
        Down
    };
    
    // Start is called before the first frame update
    void Start()
    {
        #if !UNITY_EDITOR_WIN
            gameObject.SetActive(true);
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.pointerCurrentRaycast.gameObject.name)
        {
            case "LeftButton":  GameManager_CarDrive.Instance().btnH = -1f  ; 
                
                break;
            case  "RightButton": GameManager_CarDrive.Instance().btnH = 1f  ;
                
                break;
            case  "UpButton":
                   GameManager_CarDrive.Instance().btnV = 1f  ;
                   break;
               case "DownButton": GameManager_CarDrive.Instance().btnV = -1f  ; 
                   break;
            case  "SpeedButton":
                GameManager_CarDrive.Instance().isSpeedOut = true;
                   break;
        }
    }
    
    

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (eventData.pointerCurrentRaycast.gameObject.name)
        {
            case "LeftButton":
            case   "RightButton":
                GameManager_CarDrive.Instance().btnH = 0f;
                break;
            case  "UpButton":
            case "DownButton":
                GameManager_CarDrive.Instance().btnV = 0f;
                break;
            case  "SpeedButton":
                GameManager_CarDrive.Instance().isSpeedOut = false;
                break;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        // switch (eventData .gameObject.name)
        // {
        //     case "LeftButton":
        //     case   "RightButton":
        //         GameManager_CarDrive.Instance().btnH = 0f;
        //         break;
        //     case  "UpButton":
        //     case "DownButton":
        //         GameManager_CarDrive.Instance().btnV = 0f;
        //         break;
        //     case  "SpeedButton": 
        //         GameManager_CarDrive.Instance().tempSpeed = GameManager_CarDrive.Instance().shiftSpeedOut;
        //         GameManager_CarDrive.Instance().shiftSpeedOut = 2;
        //         break;
        // }
    }

    public void OnMove(AxisEventData eventData)
    {
        Debug.Log("aa:"+eventData.moveVector);
    }
}
