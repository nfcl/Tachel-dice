using UnityEngine;
using Mirror;

public class GameCanvasPlayerManager : NetworkBehaviour
{
    private GameCanvasServerManager _manager;

    public bool IsHost => isServer & isClient;      //此客户端是否为房主

    public override void OnStartClient()
    {
        if (false == isLocalPlayer) return;

        _manager = GameObject.Find("Game_Canvas/ServerManager").GetComponent<GameCanvasServerManager>();
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
        _manager.StopConnectClient(IsHost);
    }

    [Command]
    public void CmdPlayerReady(bool isHost)
    {
        _manager.PlayerReady(isHost);
    }

    [Command]
    public void CmdGameExit(bool isHost)
    {
        //通知服务端客户端停止连接
        _manager.StopConnectServer(isHost);
    }

    /// <summary>
    /// 在指定位置放置骰子
    /// </summary>
    /// <param name="pos">放置骰子的位置</param>
    public void PutDice(int pos)
    {
        if (false == isLocalPlayer) return;
        //向服务端发送在指定位置放置骰子指令
        _manager.CmdPutDice(pos);
    }

    /// <summary>
    /// <para/>玩家控制连接
    /// <para/>将玩家控制方法绑定到场景实体上
    /// </summary>
    public void PlayerControlConnect()
    {
        _manager.SetPutDiceDelegate(PutDice);
        _manager.SetStartButtonDelegate(Button_Start);
        _manager.SetExitButtonDelegate(Button_Exit);
    }
}