﻿using Game_Canvas;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于管理Game_Canvas中的所有实体
/// 方便实施服务端的命令
/// </summary>
public class GameCanvasManager : MonoBehaviour, IGameCanvasPlayerControlConnect
{
    private PlayerData[]        _playerData;            //玩家数据显示实体
    private GameInfoDisplay    _diceBoardControl;      //骰子显示实体
    private ControlButtons      _controlButtons;        //游戏进程控制按钮
    private GameCanvasState   _state;                 //场景状态
    private Camera              _mainCamera;            //场景主相机

    public GameCanvasState    State => _state;

    private void Start()
    {
        _playerData         = new PlayerData[] { new PlayerData(transform.Find("Player1")), new PlayerData(transform.Find("Player2")) };
        _diceBoardControl   = new GameInfoDisplay(transform.Find("Plane"));
        _controlButtons     = new ControlButtons(transform.Find("Control"));
        _state              = GameCanvasState.WaitForReady;
        _mainCamera         = Camera.main;
    }

    /// <summary>
    /// 进入游戏场景
    /// </summary>
    public void Goto_GameCanvas()
    {
        //移动相机至游戏界面
        if (_mainCamera.transform.position.x != 3000)
        {
            _mainCamera.transform.position = new Vector3(3000, 0, 0);
        }
    }

    /// <summary>
    /// 回到主场景
    /// </summary>
    public void Return_MainCanvas()
    {
        //移动相机至游戏界面
        if (_mainCamera.transform.position.x != 0)
        {
            _mainCamera.transform.position = new Vector3(0, 0, 0);
        }
    }

    /// <summary>
    /// 设置指定位置的骰子值
    /// </summary>
    /// <param name="pos">指定位置</param>
    /// <param name="value">要设定的骰子点数</param>
    public void SetDiceValue(int pos, int value)
    {
        _diceBoardControl.SetDiceValue(pos, value);
    }

    /// <summary>
    /// 设定指定列的列分数
    /// </summary>
    /// <param name="linePos">指定列位置</param>
    /// <param name="value">要设置的分数</param>
    public void SetDiceLineGrade(int linePos,int value)
    {
        _diceBoardControl.SetLineGrade(linePos, value);
    }

    /// <summary>
    /// 设置玩家分数
    /// </summary>
    /// <param name="isHost">是房主</param>
    /// <param name="value">玩家分数</param>
    public void SetPlayerGrade(bool isHost,int value)
    {
        _diceBoardControl.SetGrade(isHost, value);
    }

    /// <summary>
    /// 玩家信息设置
    /// </summary>
    /// <param name="IsHost">是否是房主</param>
    /// <param name="data">传入的玩家信息</param>
    public void SetPlayerData(bool IsHost, GameCanvasPlayerData data)
    {
        if (data is null)
        {
            _playerData[IsHost ? 0 : 1].Init();
            return;
        }
        if (IsHost)
        {
            _playerData[0].SetAvatar(data.Avatar);
            _playerData[0].SetName(data.Name);
        }
        else
        {
            _playerData[1].SetAvatar(data.Avatar);
            _playerData[1].SetName(data.Name);
        }
    }

    /// <summary>
    /// 骰子放置按钮组点击委托设置
    /// </summary>
    /// <param name="del">要设置的委托</param>
    public void SetPutDiceDelegate(DiceButtonControlDel del)
    {
        _diceBoardControl.SetPutDiceDelegate(del);
    }

    /// <summary>
    /// 开始按钮点击委托设置
    /// </summary>
    /// <param name="del">要设置的委托</param>
    public void SetStartButtonDelegate(StartButtonDel del)
    {
        _controlButtons.SetStartButtonDelegate(del);
    }

    /// <summary>
    /// 退出按钮点击委托设置
    /// </summary>
    /// <param name="del">要设置的委托</param>
    public void SetExitButtonDelegate(ExitButtonDel del)
    {
        _controlButtons.SetExitButtonDelegate(del);
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    public void GameStart()
    {
        _state = GameCanvasState.Gaming;
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameEnd()
    {
        _state = GameCanvasState.WaitForReady;
    }

    /// <summary>
    /// 设置提示文本
    /// </summary>
    /// <param name="content">显示内容</param>
    public void ShowTipText(string content)
    {
        _controlButtons.Tip_Text.text = content;
    }

    /// <summary>
    /// 控制游戏进程的按钮组类
    /// </summary>
    private class ControlButtons
    {
        private Button  _start_Button;      //开始按钮
        private Button  _exit_Button;       //退出按钮
        private Text    _tip_Text;          //提示文本

        public Button   Start_Button    => _start_Button;
        public Button   Exit_Button     => _exit_Button;
        public Text     Tip_Text        => _tip_Text;

        public ControlButtons(Transform source)
        {
            _start_Button   = source.Find("Start").GetComponent<Button>();
            _exit_Button    = source.Find("Exit").GetComponent<Button>();
            _tip_Text       = source.Find("Tip").GetComponent<Text>();
        }

        public void SetStartButtonDelegate(StartButtonDel del)
        {
            _start_Button.onClick.RemoveAllListeners();
            _start_Button.onClick.AddListener(() => del());
        }

        public void SetExitButtonDelegate(ExitButtonDel del)
        {
            _exit_Button.onClick.RemoveAllListeners();
            _exit_Button.onClick.AddListener(() => del());
        }
    }

    /// <summary>
    /// 玩家信息显示类
    /// </summary>
    private class PlayerData
    {
        /// <summary>
        /// 头像图片
        /// </summary>
        private Image _avatar;
        /// <summary>
        /// 名称文本框
        /// </summary>
        private Text _name;

        public PlayerData(Transform source)
        {

            _avatar = source.Find("Avatar/Content").GetComponent<Image>();

            _name = source.Find("Name/Text").GetComponent<Text>();
        }

        /// <summary>
        /// 设置头像
        /// </summary>
        /// <param name="id"></param>
        public void SetAvatar(int id)
        {
            //检测输入的头像id合法性
            if (id < 1 || id > Start_Canvas_SettingPannel_Manager.AVATARNUM) return;
            //从本地读取头像图片
            _avatar.sprite = Resources.Load<Sprite>($"Avatar/{id}");
        }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            _name.text = name;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            _avatar.sprite = Resources.Load<Sprite>($"Avatar/{1}");
            _name.text = "null";
        }
    }

    /// <summary>
    /// 游戏相关信息显示类
    /// </summary>
    private class GameInfoDisplay
    {
        /// <summary>
        /// 用于显示对应位置骰子放置情况的图片
        /// </summary>
        private Image[] _dice_Image;
        /// <summary>
        /// 用于接受放置骰子事件的按钮
        /// </summary>
        private Button[] _dice_Button;
        /// <summary>
        /// 列分数显示文本
        /// </summary>
        private Text[] _lineGrade_Text;
        /// <summary>
        /// 骰子值对应的图片
        /// 0是没有放置骰子的底图
        /// 1-6是对应的骰子面
        /// </summary>
        private Sprite[] _dice_value2Sprite;
        /// <summary>
        /// 玩家总分数
        /// </summary>
        private Text[] _grade;

        public GameInfoDisplay(Transform source)
        {
            _dice_Image = new Image[18];
            _dice_Button = new Button[18];
            for (int i = 1; i <= 9; ++i)
            {
                _dice_Image[i - 1] = source.Find($"Player1/{i}").GetComponent<Image>();
                _dice_Button[i - 1] = _dice_Image[i - 1].gameObject.GetComponent<Button>();
            }
            for (int i = 1; i <= 9; ++i)
            {
                _dice_Image[i + 8] = source.Find($"Player2/{i}").GetComponent<Image>();
                _dice_Button[i + 8] = _dice_Image[i + 8].gameObject.GetComponent<Button>();
            }
            _dice_value2Sprite = new Sprite[7];
            for (int i = 0; i < 7; ++i)
            {
                _dice_value2Sprite[i] = Resources.Load<Sprite>($"Game_Scene/sprite/Dice_{i}");
            }
            _lineGrade_Text = new Text[6];
            for (int i = 0; i < 6; ++i)
            {
                _lineGrade_Text[i] = source.Find($"").GetComponent<Text>();
            }
            _grade = new Text[2]
            {
                source.Find("Player1/Grade/Text").GetComponent<Text>(),
                source.Find("Player2/Grade/Text").GetComponent<Text>(),
            };
        }

        /// <summary>
        /// 设置骰子放置按钮点击事件
        /// </summary>
        /// <param name="del">点击委托</param>
        /// <param name="pos">设置的点击</param>
        public void SetPutDiceDelegate(DiceButtonControlDel del)
        {
            for (int i = 0; i < 17; ++i)
            {
                __setPutDiceDelegate(del, i);
            }
        }

        /// <summary>
        /// 用于SetPutDiceDelegate绑定带参delegate
        /// </summary>
        /// <param name="del">需要绑定的委托</param>
        /// <param name="pos">参数</param>
        private void __setPutDiceDelegate(DiceButtonControlDel del,int pos)
        {
            //移除之前绑定的所有委托
            _dice_Button[pos].onClick.RemoveAllListeners();
            //绑定新的委托
            _dice_Button[pos].onClick.AddListener(() => del(pos));
        }

        /// <summary>
        /// 设置指定位置骰子的点数
        /// </summary>
        /// <param name="pos">指定的位置</param>
        /// <param name="value">设置的骰子点数</param>
        public void SetDiceValue(int pos, int value)
        {
            //位置合法性检查
            if (pos < 0 || pos > 17) return;
            //点数合法性检查
            if (value < 0 || value > 6) return;
            //设定点数对应的sprite
            _dice_Image[pos].sprite = _dice_value2Sprite[value];
        }

        /// <summary>
        /// 设置指定列的分数
        /// </summary>
        /// <param name="linePos">指定的列位置</param>
        /// <param name="value">设置的分数</param>
        public void SetLineGrade(int linePos, int value)
        {
            if (linePos < 0 || linePos > 5) return;

            _lineGrade_Text[linePos].text = value.ToString();
        }

        /// <summary>
        /// 设置玩家分数
        /// </summary>
        /// <param name="isPlayer1">是玩家1</param>
        /// <param name="value">要设置的分数</param>
        public void SetGrade(bool isPlayer1,int value)
        {
            _grade[true == isPlayer1 ? 0 : 1].text = $"得分：{value}";
        }
    }
}