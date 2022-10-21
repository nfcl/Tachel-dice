using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Start_Canvas_Manager : MonoBehaviour
{
    public static Start_Canvas_Manager instance;

    private Start_Canvas_State           _state;             //当前场景状态
    private Start_Canvas_PannelChange    _pannelControl;     //UIPannel切换控制

    private void Start()
    {
        _state = Start_Canvas_State.Main;
        _pannelControl = new Start_Canvas_PannelChange();
        UpdateLocalPlayerData();
    }

    public void Button_SinglePlayerMode()   => Single_PlayerMode_Select();  //点击单人模式按钮
    public void Button_DoublePlayerMode()   => Double_PlayerMode_Select();  //点击多人模式按钮
    public void Button_Return()             => Return();                    //点击返回按钮
    public void Button_Exit()               => Exit();                      //点击退出按钮

    /// <summary>
    /// 选择单人模式
    /// </summary>
    private void Single_PlayerMode_Select()
    {

    }

    ///jsonData缓冲区
    ///{
            n_LocalPlayerData.Root _UpdateLocalPlayerData_data;
    ///}
    /// <summary>
    /// 根据json数据更新场景中的玩家数据
    /// </summary>
    public void UpdateLocalPlayerData()
    {
        _UpdateLocalPlayerData_data = Tool.JsonReader<n_LocalPlayerData.Root>("LocalPlayerData.json");
        GameObject.Find("Start_Canvas/Avatar/Content").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Avatar/{_UpdateLocalPlayerData_data.LocalPlayerData.Avatar}");
        GameObject.Find("Start_Canvas/Name/Text").GetComponent<Text>().text = _UpdateLocalPlayerData_data.LocalPlayerData.Name;
    }

    /// <summary>
    /// 选择多人模式
    /// </summary>
    private void Double_PlayerMode_Select()
    {
        _state = Start_Canvas_State.DoubleModeSelect;
        _pannelControl.Change(_state);
    }

    /// <summary>
    /// 返回
    /// 根据_state进行判断
    /// </summary>
    private void Return()
    {
        switch (_state)
        {
            case Start_Canvas_State.DoubleModeSelect:
                {
                    _state = Start_Canvas_State.Main;
                    break;
                }
            default:
                {
                    break;
                }
        }
        _pannelControl.Change(_state);
    }

    /// <summary>
    /// 退出
    /// </summary>
    private void Exit()
    {
#if UNITY_EDITOR
        //编辑器模式下
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //发布后的应用运行下
        Application.Quit();
#endif
    }

    private void Awake()
    {
        if(instance is null)
        {
            instance = this;
        }
    }
}
