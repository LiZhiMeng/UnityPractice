using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    private Button Drive;

    private Button FPS;

    private Button Clock;
    // Start is called before the first frame update
    void Start()
    {
        Drive = GameObject.Find("Drive").GetComponent<Button>();
        this.FPS = GameObject.Find("FPS").GetComponent<Button>();
        this.Clock = GameObject.Find("Clock").GetComponent<Button>();

        InitBtn();
    }

    void InitBtn()
    {
        this.Drive.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(1);
        });
        this.FPS.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(2);
        });
        this.Clock.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(3);
        });
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
