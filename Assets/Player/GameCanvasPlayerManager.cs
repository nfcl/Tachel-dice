using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GameCanvasPlayerManager : NetworkBehaviour
{
    private GameCanvasServerManager _manager; //

    public bool IsHost => isServer & isClient;     //当前玩家为房主

    public override void OnStartClient()
    {
        if (false == isLocalPlayer) return;

        _manager = GameObject.Find("Game_Canvas/ServerManager").GetComponent<GameCanvasServerManager>();
        //玩家控制连接
        PlayerControlConnect();

    }

    public void Button_Start()
    {
        if (false == isLocalPlayer) return;

        CmdGameStart();
    }

    public void Button_Exit()
    {
        if (false == isLocalPlayer) return;
        //
        CmdGameExit(IsHost);
        //客户端停止连接
        _manager.StopConnectClient(IsHost);
    }

    [Command]
    public void CmdGameStart()
    {

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