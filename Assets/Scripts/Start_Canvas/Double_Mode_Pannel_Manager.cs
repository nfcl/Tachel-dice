using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// <para/>联机游戏面板管理器
/// <para/>用于管理房间生成和加入
/// </summary>
public class Double_Mode_Pannel_Manager : MonoBehaviour
{
    [SerializeField]
    private NetworkManager _networkManager;         //网络管理器
    private InputField _ip_Input;                   //房间ip输入框

    private void Start()
    {
        _ip_Input = GameObject.Find("Start_Canvas/Double_Mode_Pannel/Button_Join_Room/InputField").GetComponent<InputField>();
    }

    //创建房间按钮
    public void Button_CreaterNewRoom()     => CreateNewRoom();
    //加入房间按钮
    public void Button_JoinRoom()           => JoinRoom();

    /// <summary>
    /// <para/>创建一个新房间
    /// <para/>自己作为房主
    /// <para/>Serve+Client
    /// </summary>
    private void CreateNewRoom()
    {
        _networkManager.StartHost();
    }

    /// <summary>
    /// <para/>加入一个房间
    /// <para/>以_ip_Input中输入的ip为目标房间
    /// <para/>Client
    /// </summary>
    private void JoinRoom()
    {
        _networkManager.networkAddress = _ip_Input.text;
        _networkManager.StartClient();
    }
}
