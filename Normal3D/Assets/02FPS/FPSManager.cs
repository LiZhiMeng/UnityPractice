using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FPSManager : MonoBehaviour
{   
//Actions（行动）
// Rules（规则）
// Goals（游戏目标）
// Objects（游戏对象）
// Playspace（游戏空间）
// Players（玩家）

// 人物行走
//todo 鼠标转向
//todo 人物射击
//todo 人物击中
//todo 操作说明
//todo 敌人生成
//todo 失败倒计时
//todo 难度递增
//todo 游戏目标
//todo 游戏空间
//todo 游戏玩家
//todo ui,得分显示，重开按钮等
// 相机控制
    
    private Transform _transformPlayer;
    private Rigidbody _playerRigidbody;
    private Transform _transformCamera;
    private Vector3 _offsetCamera;
    private float v = 0f;
    private float h = 0f;
    public  float   speed         = 6.0F;
    public  float   jumpSpeed     = 8.0F;
    public  float   gravity       = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private Transform _enemyTransform;
    private Transform _enemysWid;
    public Transform _enemyParent;

    private Button Button_Back;
    private void Awake()
    {
        InitObjs();
        BackToMain();
    }

    void Start()
    {
        GenerateEnemys();
    }
    

    void Update()
    {

       
    }

    private void FixedUpdate()
    {
        CameraControl();
        PlayerControl();
    }

    void InitObjs()
    {
        _transformPlayer = GameObject.Find("FPS_Player").GetComponent<Transform>();
        _playerRigidbody = _transformPlayer.GetComponent<Rigidbody>();
        
        _transformCamera = GameObject.Find("Main Camera").transform;
        _offsetCamera = _transformCamera.position - _transformPlayer.position;
        _enemyTransform = GameObject.Find("FPS_Enemy").GetComponent<Transform>();
        _enemyParent = GameObject.Find("enemyParent").GetComponent<Transform>();
        Button_Back = GameObject.Find("Button_Back").GetComponent<Button>();
    }
    
    void GenerateEnemys()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 generatePos = Vector3.zero;
            generatePos = new Vector3(Random.Range(-30,30),1,Random.Range(-30,30));
            GameObject.Instantiate(_enemyTransform, generatePos, Quaternion.identity, _enemyParent);    
        }
        
    }

    void CameraControl()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        // this._transformPlayer .Rotate(Quaternion.Euler(-mouseY, mouseX, 0).eulerAngles,Space.Self);
        //this._transformCamera.RotateAround(this._transformPlayer.position, new Vector3(mouseY, mouseX, 0), 1f);
        Debug.Log("x"+ mouseX);
        _transformCamera.RotateAround(_transformPlayer.transform.position, Vector3.up, mouseX*5);
        _transformCamera.RotateAround(_transformPlayer.transform.position, _transformCamera.right, mouseY*5);
        //this._transformCamera.RotateAround(this._transformPlayer.position, new Vector3(0, mouseX, 0), 1f);
        // this._transformCamera.Rotate(Quaternion.Euler(-mouseY, mouseX, 0).eulerAngles, Space.Self);
        if (mouseX>0)
        {
            Debug.Log("mouseX"+mouseX);
        }
    }

    void PlayerControl()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void BackToMain()
    {
        Button_Back.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(0);
        });
    }
}
