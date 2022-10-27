using UnityEngine;
using Mirror;

public class GameCanvasPlayerManager : NetworkBehaviour
{
    private GameCanvasServerManager  _serverManager;    //服务端管理器

    public bool IsHost => isServer & isClient;          //此客户端是否为房主

    public override void OnStartClient()
    {
        //寻找场景中的服务端管理器
        _serverManager = GameObject.Find("Game_Canvas/ServerManager").GetComponent<GameCanvasServerManager>();
        //如果不是本地玩家则不需要执行以下代码
        if (false == isLocalPlayer) return;
        //玩家控制和场景物体进行连接
        PlayerControlConnect();
    }

    /// <summary>
    /// <para/>用于绑定到场景按钮上的回调
    /// <para/>玩家切换准备状态
    /// </summary>
    public void Button_Ready()
    {
        if (false == isLocalPlayer) return;
        //向服务端发送玩家准备指令
        CmdPlayerReady(IsHost);
    }

    /// <summary>
    /// <para/>用于绑定到场景按钮上的回调
    /// <para/>玩家退出游戏
    /// </summary>
    public void Button_Exit()
    {
        if (false == isLocalPlayer) return;
        //客户端停止连接
        _serverManager.StopConnectClient(IsHost);
    }

    /// <summary>
    /// <para/>用于绑定到场景按钮上的回调
    /// <para/>在指定位置放置骰子
    /// </summary>
    /// <param name="pos">放置骰子的位置</param>
    public void PutDice(int pos)
    {
        //向服务端发送在指定位置放置骰子指令
        _serverManager.CmdPutDice(pos);
    }

    /// <summary>
    /// 将玩家控制方法绑定到场景实体上
    /// </summary>
    public void PlayerControlConnect()
    {
        _serverManager.SetPutDiceDelegate(IsHost, PutDice);
        _serverManager.SetStartButtonDelegate(Button_Ready);
        _serverManager.SetExitButtonDelegate(Button_Exit);
    }

    /// <summary>
    /// 玩家想服务端发出切换准备状态的命令
    /// </summary>
    /// <param name="isHost">是否是房主</param>
    [Command]
    public void CmdPlayerReady(bool isHost)
    {
        _serverManager.PlayerReady(isHost);
    }
}