using UnityEngine;
using Mirror;

public class GameCanvasPlayerManager : NetworkBehaviour
{
    private GameCanvasServerManager  _serverManager;

    public bool IsHost => isServer & isClient;      //此客户端是否为房主

    public override void OnStartClient()
    {

        _serverManager = GameObject.Find("Game_Canvas/ServerManager").GetComponent<GameCanvasServerManager>();

        if (false == isLocalPlayer) return;
        //玩家控制和场景物体进行连接
        PlayerControlConnect();
    }

    public void Button_Start()
    {
        if (false == isLocalPlayer) return;
        //向服务端发送玩家准备指令
        CmdPlayerReady(IsHost);
    }

    public void Button_Exit()
    {
        if (false == isLocalPlayer) return;
        //向服务端发送玩家退出游戏指令
        CmdGameExit(IsHost);
        //客户端停止连接
        _serverManager.StopConnectClient(IsHost);
    }

    [Command]
    public void CmdPlayerReady(bool isHost)
    {
        _serverManager.PlayerReady(isHost);
    }

    [Command]
    public void CmdGameExit(bool isHost)
    {
        //通知服务端客户端停止连接
        _serverManager.StopConnectServer(isHost);
    }

    /// <summary>
    /// 在指定位置放置骰子
    /// </summary>
    /// <param name="pos">放置骰子的位置</param>
    public void PutDice(int pos)
    {
        //向服务端发送在指定位置放置骰子指令
        _serverManager.CmdPutDice(pos);
    }

    /// <summary>
    /// <para/>玩家控制连接
    /// <para/>将玩家控制方法绑定到场景实体上
    /// </summary>
    public void PlayerControlConnect()
    {
        _serverManager.SetPutDiceDelegate(IsHost, PutDice);
        _serverManager.SetStartButtonDelegate(Button_Start);
        _serverManager.SetExitButtonDelegate(Button_Exit);
    }
}