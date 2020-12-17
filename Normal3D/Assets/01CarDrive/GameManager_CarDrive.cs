using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// 前后左右行驶 
// 左右转向
// 加分道具 + 速度
//掉下悬崖+时间 重置位置
//障碍物随机生成
//到达结算 游戏停止,显示 当前得分，排行榜，重玩按钮  
//分数UI显示
//吃道具后闪屏幕
//速度等级ui显示
//重开ui功能
//shift加速
//排行榜功能  按名次滚动显示时间 
//记录胜利的时间
//游戏开始时渐变显示出界面
//时间只记录两位小数
//增加食物的特效显示,车尾特效
//增加碰撞音效、胜利音效、速度音效、ui点击音效
//todo 增加移动端控制
//todo bug 按speed时，速度增加

public class GameManager_CarDrive : MonoBehaviour
{
    private static GameManager_CarDrive _instance;

    public static GameManager_CarDrive Instance()
    {
        return _instance;
    }
    

    [HideInInspector]
    public float btnH, btnV=0f;
    
    private Transform   _carTransform;
    private Transform   _rotateGo;
    private float       _h,_v;
    public  float       runSpeed      = 1.0f;
    public  float       rotationSpeed = 1.0f;
    [HideInInspector]
    public bool isSpeedOut = false;
    public  int         shiftSpeedOut = 2;
    private bool        _isGameOver   = false;
    private bool        _isShiftHolder;
    private float       _shiftSpeed = 1.0f;
    private Transform   _cameraTransform;
    private Vector3     _offsetCarCamera;
    private Transform   _originalBlock;
    private Transform   _groundTransform;
    private Text _timeText, _tipText, _currentSpeedText, _levelText;
    private float       _addTime;
    private Button      _reload,_rank;
    private Rigidbody   _carRigidbody;
    private Material    _foodMaterial;
    public  Image       _foodGetImage;
    private Color       _foodColor;
    private bool        _showGameWorldOver = false;
    private Transform   _sVContent;
    public  Transform   _carScrollView;
    private Transform   _templetText;
    private Button      _clearDataBtn;
    public  bool        ToSuccess = false;
    public  AudioClip   _audioClick,_audioBling;
    private AudioSource _mainAudioSource;
    public  Transform   _successText;

    public Button Button_Back;

    public Transform LeftButton, RightButton, SpeedButton, UpButton, DownButton;

    void Awake()
    {
        _instance = this;
        _foodGetImage.gameObject.SetActive(true);
        _carTransform = GameObject.Find("Car").transform;
        _rotateGo = GameObject.Find("rotateGo").transform;
        _cameraTransform = GameObject.Find("Main Camera").transform;
        _originalBlock = GameObject.Find("originalBlock").transform;
        _groundTransform = GameObject.Find("ground").transform;
        _timeText = GameObject.Find("TimeText").transform.GetComponent<Text>();
        _levelText = GameObject.Find("LevelText").transform.GetComponent<Text>();
        _reload = GameObject.Find("reload").transform.GetComponent<Button>();
        _rank = GameObject.Find("rank").transform.GetComponent<Button>();
        Button_Back = GameObject.Find("Button_Back").transform.GetComponent<Button>();
 
        _carRigidbody = _carTransform.GetComponent<Rigidbody>();
        _carScrollView.gameObject.SetActive(true);
        _templetText = GameObject.Find("TempletText").GetComponent<Transform>();
        _sVContent = GameObject.Find("Content").GetComponent<Transform>();
        _clearDataBtn = GameObject.Find("clearDataBtn").GetComponent<Button>();
        _mainAudioSource = _carTransform.GetComponent<AudioSource>();

        StartToInitValue();
        ReloadGame();    //重开游戏
        ClickRank();     //排行榜
        GeneralBlock();  //生成障碍物
        ClickClearRankData(); //清空排行数据
        BackToMain();
        InitPlatformBtn();
    }

    private void Start()
    {
        _carScrollView.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!_isGameOver)
        {
            UpdateTime();
            UpdateOutRoad();
            ShowGameWorld();
        }
        
    }

    private void FixedUpdate()
    {
        if (!this._isGameOver)
        {
            UpdateControl();    
        }
        
    }

    void StartToInitValue()
    {
        _offsetCarCamera = _cameraTransform.position - _carTransform.position;
        _timeText.text = "0";
        _foodColor = GetRandomeColor();
        _successText.gameObject.SetActive(false);
    }

    void ReloadGame()
    {
        UnityAction reload;
        reload = () =>
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            _mainAudioSource.clip = _audioClick;
            _mainAudioSource.Play();
        };
        _reload.onClick.AddListener(reload);
    }

    void ClickRank()
    {
        UnityAction rankFun;
        rankFun = () =>
        {
            _mainAudioSource.clip = _audioClick;
            _mainAudioSource.Play();
            if (_carScrollView.transform.gameObject.activeSelf)
            {
                Text[] textList = _sVContent.GetComponentsInChildren<Text>();
                for (int i = 0; i < textList.Length; i++)
                {
                    Destroy(textList[i].gameObject);
                }
                _carScrollView.gameObject.SetActive(false);
            }
            else
            {
                _templetText.gameObject.SetActive(true);
                _carScrollView.gameObject.SetActive(true);
                string rankData = PlayerPrefs.GetString("CarDrive");
                if (rankData == "")
                {
                    _templetText.gameObject.SetActive(false);
                    return;
                }

                string[]    rankSplit = rankData.Split(',');
                List<float> rankFloat = new List<float>();
                for (int i = 0; i < rankSplit.Length; i++)
                {
                    rankFloat.Add(float.Parse(rankSplit[i]));
                }

                RankFloat(ref rankFloat);
                for (int i = 0; i < rankFloat.Count; i++)
                {
                    Transform cur_TextGo = Instantiate(_templetText, _sVContent);
                    Text      curText    = cur_TextGo.GetComponent<Text>();
                    curText.text = "第" + (i + 1) + "名: " + rankFloat[i];
                }

                _templetText.gameObject.SetActive(false);
            }
        };
        _rank.onClick.AddListener(rankFun);
    }

    void GeneralBlock()
    {
        Transform blockParent;
        blockParent = _originalBlock.parent;
        Vector3 currentPos = Vector3.zero;
        float   _xRange    = 0;

        List<int> foodList = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            int i_food = UnityEngine.Random.Range(1, 40);
            while (foodList.Contains(i_food))
            {
                i_food = UnityEngine.Random.Range(1, 40);
            }

            foodList.Add(i_food);
        }

        int blockCount = Int32.Parse(_groundTransform.localScale.z.ToString());
        for (int i = 2; i < blockCount / 4; i++)
        {
            _xRange = UnityEngine.Random.Range(-4.5f, 4.5f);
            currentPos = new Vector3(_xRange, _originalBlock.position.y, i * 2);
            //生成障碍物
            Transform _block = GameObject.Instantiate(_originalBlock, currentPos, Quaternion.identity, blockParent);
            //生成道具
            if (foodList.Contains(i))
            {
                _foodMaterial = _block.GetComponent<MeshRenderer>().material;
                _foodMaterial.color = _foodColor;
                _block.tag = "food";
                _block.GetComponent<BoxCollider>().isTrigger = true;
            }
        }
    }

    void ClickClearRankData()
    {
        _clearDataBtn.onClick.AddListener(() =>
        {
            _mainAudioSource.clip = _audioClick;
            _mainAudioSource.Play();
            PlayerPrefs.SetString("CarDrive", "");
            Text[] texts = _sVContent.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].text = "";
            }
        });
    }

    void BackToMain()
    {
        Button_Back.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(0);
        });
    }

    void InitPlatformBtn()
    {
        
    #if !UNITY_EDITOR_WIN
            LeftButton.gameObject.SetActive(true);
            RightButton.gameObject.SetActive(true);
            UpButton.gameObject.SetActive(true);
            LeftButton.gameObject.SetActive(true);
            DownButton.gameObject.SetActive(true);
    
        #endif
        
    }



    void ShowGameWorld()
    {
        if (!_showGameWorldOver)
        {
            if (_foodGetImage.color.a < 0.1f)
            {
                _showGameWorldOver = true;
                _foodGetImage.gameObject.SetActive(false);
            }

            _foodGetImage.color =
                Color.Lerp(_foodColor, new Color(_foodColor.r, _foodColor.g, _foodColor.b, 0), Time.time);
        }
    }
    void UpdateControl()
    {
        #if UNITY_EDITOR_WIN
            _h = Input.GetAxis("Horizontal"); //获取玩家的操作
            _v = Input.GetAxis("Vertical");
        #else
           _h = btnH; 
            _v = btnV;
        #endif

        Vector3 carVelocity    = new Vector3(_h, 0, _v);
        Vector3 carVectorSpeed = _carTransform.forward * _v * runSpeed;
        if (_carRigidbody)
        {
            _carRigidbody.velocity = carVectorSpeed;
        }
        _cameraTransform.position = _carTransform.position + _offsetCarCamera;        //视角控制
        _carTransform.Rotate(new Vector3(0, _h, 0) * rotationSpeed, Space.Self);      //四驱车，原地转
        #if UNITY_EDITOR_WIN
            _shiftSpeed = Input.GetKey(KeyCode.LeftShift) == true ? shiftSpeedOut : 1.0f; //shift加速
        #else
            _shiftSpeed = this.isSpeedOut == true ? shiftSpeedOut : 1.0f; //shift加速    
        #endif
       
        if (_carRigidbody)
        {
            _carRigidbody.velocity = carVectorSpeed * _shiftSpeed; //动力    
        }
        
        
    }
    
    

    void UpdateTime()
    {
        _timeText.text = (Time.timeSinceLevelLoad + _addTime).ToString("f2");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("food"))
        {
            _mainAudioSource.clip = _audioBling;
            _mainAudioSource.Play();
            other.gameObject.SetActive(false);
            shiftSpeedOut++;
            if (shiftSpeedOut != 0)
            {
                _levelText.text = "当前等级:" + (shiftSpeedOut - 2);
            }

            StartCoroutine(ImageFlash());
        }

        if (other.CompareTag("VectoryGo"))
        {
            _isGameOver = true;
            _successText.gameObject.SetActive(true);
            string carRank = PlayerPrefs.GetString("CarDrive");
            string newRank = "";
            if (string.IsNullOrEmpty(carRank))
            {
                newRank = _timeText.text;
                Debug.Log("newRank:" + newRank);
            }
            else
            {
                newRank = carRank + "," + _timeText.text;
            }

            PlayerPrefs.SetString("CarDrive", newRank);
        }
    }

    void UpdateOutRoad()
    {
        if (_carTransform.position.y < 0.3f)
        {
            _addTime = _addTime + 10f;
            Vector3 pos = _carTransform.position;
            pos.y = 1.5f;
            pos.x = 1;
            _carTransform.SetPositionAndRotation(pos, Quaternion.identity);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("block"))
        {
        }
    }

    IEnumerator ImageFlash()
    {
        _foodGetImage.color = GetRandomeColor();
        _foodGetImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        _foodGetImage.gameObject.SetActive(false);
    }



    /// <summary>
    /// 对排行数据排序
    /// </summary>
    /// <param name="floatList"></param>
    /// <returns></returns>
    List<float> RankFloat(ref List<float> floatList)
    {
        for (int i = 0; i < floatList.Count; i++)
        {
            for (int j = i + 1; j < floatList.Count; j++)
            {
                float temp = 0;
                if (floatList[i] > floatList[j])
                {
                    temp = floatList[i];
                    floatList[i] = floatList[j];
                    floatList[j] = temp;
                }
            }
        }

        return floatList;
    }
    
    Color GetRandomeColor()
    {
        return new Color(UnityEngine.Random.Range(0f, 255f), UnityEngine.Random.Range(0f, 255f),
            UnityEngine.Random.Range(0f,              255f));
    }

    private void Test() //备用代码
    {
        if (ToSuccess)
        {
            _isGameOver = true;
        }
    }


}