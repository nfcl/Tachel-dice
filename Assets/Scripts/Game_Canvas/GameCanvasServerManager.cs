using UnityEngine;
using Mirror;
using Game_Canvas;
using System;

/// <summary>
/// 服务端的游戏管理器
/// </summary>
public class GameCanvasServerManager : NetworkBehaviour, IGameCanvasPlayerControlConnect
{
    /// <summary>
    /// <para/>网络管理器
    /// </summary>
    [SerializeField]
    private NetworkManager              _networkManager;
    /// <summary>
    /// 场景物体管理器
    /// </summary>
    private GameCanvasManager           _canvas_Manager;
    /// <summary>
    /// <para/>房间中的玩家信息
    /// <para/>下标0是房主
    /// </summary>
    private GameCanvasPlayerData[]      _playerData;
    /// <summary>
    /// 游戏进程管理器
    /// </summary>
    private GameCanvasGameManager       _game_Manager;
    /// <summary>
    /// 客户端连接到服务端
    /// </summary>
    public override void OnStartClient()
    {
        //主机玩家创建新房间初始化
        if (true == isServer)
        {
            //向NetworkServer订阅OnDisconnectedEvent(客户端断开连接)事件
            NetworkServer.OnDisconnectedEvent += OnServerDisconnect;
            //房间内玩家信息初始化
            _playerData = new GameCanvasPlayerData[2]
            {
                null,       //null代表没有玩家
                null,       //null代表没有玩家
            };
            //创建新的游戏进程管理器
            _game_Manager = new GameCanvasGameManager();
        }
        //寻找场景物体管理器
        _canvas_Manager = transform.parent.GetComponent<GameCanvasManager>();
        //移动相机至游戏界面
        _canvas_Manager.Goto_GameCanvas();
        //将新的玩家信息添加到服务端的_playerData
        CmdPlayerChange(isServer, new GameCanvasPlayerData(Tool.JsonReader<n_LocalPlayerData.Root>("LocalPlayerData.json")));
        //在各个客户端重新绘制玩家信息
        CmdDrawPlayerInfo();
        //非主机玩家进入房间时与主机同步
        if (false == isServer)
        {
            CmdUpdateDiceData();
            CmdUpdateGradeData();
        }
    }

    private void OnServerDisconnect(NetworkConnection conn)
    {

    }

    /// <summary>
    /// 客户端与服务端停止连接
    /// </summary>
    public override void OnStopClient()
    {
        //移动相机至游戏界面
        _canvas_Manager.Return_MainCanvas();
    }

    /// <summary>
    /// <para/>当前客户端停止网络连接
    /// <para/>(客户端调用)
    /// <para/>用于客户端停止连接
    /// </summary>
    /// <param name="isHost">是房主调用</param>
    [Client]
    public void StopConnectClient(bool isHost)
    {
        if (true == isHost)
        {
            _networkManager.StopHost();
        }
        else
        {
            _networkManager.StopClient();
        }
    }

    /// <summary>
    /// <para/>当前客户端停止网络连接
    /// <para/>(服务端调用)
    /// <para/>用于提前通知服务端客户端停止连接
    /// </summary>
    /// <param name="isHost">是房主调用</param>
    [Server]
    public void StopConnectServer(bool isHost)
    {
        if(true == isHost)
        {
            _playerData[0] = null;
        }
        else
        {
            _playerData[1] = null;
        }
        //在所有客户端上重新绘制玩家信息
        RpcDrawPlayerInfo(isHost, null);
    }

    /// <summary>
    /// 放置骰子
    /// </summary>
    /// <param name="pos">要放置的位置</param>
    [Command(requiresAuthority = false)]
    public void CmdPutDice(int pos)
    {
        //尝试在指定位置放置骰子
        if (true == _game_Manager.PutDice(pos))
        {
            //更新骰子放置信息
            CmdUpdateDiceData();
            //更新分数信息
            CmdUpdateGradeData();
            //检测是否结束游戏

        }
    }

    /// <summary>
    /// 更新骰子信息命令
    /// </summary>
    [Command(requiresAuthority =false)]
    public void CmdUpdateDiceData()
    {
        //更新双方骰子放置信息
        for (int i = 0; i < 18; ++i)
        {
            RpcSetDiceValue(i, _game_Manager.DiceSlots[i]);
        }
    }

    /// <summary>
    /// 更新分数信息命令
    /// </summary>
    [Command(requiresAuthority =false)]
    public void CmdUpdateGradeData()
    {
        //更新双方列分数信息
        for (int i = 0; i < 6; ++i)
        {
            RpcSetDiceLineGrade(i, _game_Manager.LineGrades[i]);
        }
        //更新双方总分数信息
        RpcSetPlayerGrade(true, _game_Manager.Player1Grade);
        RpcSetPlayerGrade(false, _game_Manager.Player2Grade);
    }

    /// <summary>
    /// 玩家信息更改
    /// </summary>
    /// <param name="isHost">是否是房主</param>
    /// <param name="playerData">要更改为的玩家信息</param>
    [Command(requiresAuthority = false)]
    public void CmdPlayerChange(bool isHost, n_LocalPlayerData.Root playerData)
    {
        _playerData[isHost ? 0 : 1].SetPlayerInfo(playerData);
    }

    /// <summary>
    /// 绘制玩家信息
    /// </summary>
    [Command(requiresAuthority =false)]
    public void CmdDrawPlayerInfo()
    {
        RpcDrawPlayerInfo(true, _playerData[0]);
        RpcDrawPlayerInfo(false, _playerData[1]);
    }

    /// <summary>
    /// 设置骰子点数
    /// </summary>
    /// <param name="pos">设置的骰子位置</param>
    /// <param name="value">设置成的骰子点数</param>
    [ClientRpc]
    public void RpcSetDiceValue(int pos, int value)
    {
        _canvas_Manager.SetDiceValue(pos, value);
    }

    /// <summary>
    /// 设置列分数
    /// </summary>
    /// <param name="linePos">设置的列分数位置</param>
    /// <param name="value">设置的分数</param>
    [ClientRpc]
    public void RpcSetDiceLineGrade(int linePos,int value)
    {
        _canvas_Manager.SetDiceValue(linePos, value);
    }

    /// <summary>
    /// 设置玩家分数
    /// </summary>
    /// <param name="isHost">是房主</param>
    /// <param name="value">玩家分数</param>
    [ClientRpc]
    public void RpcSetPlayerGrade(bool isHost, int value)
    {
        _canvas_Manager.SetPlayerGrade(isHost, value);
    }

    /// <summary>
    /// 客户端绘制玩家信息
    /// </summary>
    [ClientRpc]
    public void RpcDrawPlayerInfo(bool IsHost, GameCanvasPlayerData data)
    {
        _canvas_Manager.SetPlayerData(IsHost, data);
    }

    [ClientRpc]
    public void RpcShowTipText(string content)
    {
        _canvas_Manager.ShowTipText(content);
    }

    [ClientRpc]
    public void RpcGameStart()
    {

    }

    [ClientRpc]
    public void RpcGameEnd()
    {

    }

    [ClientRpc]
    public void RpcChangeControlButton(GameCanvasState state)
    {
        switch (state)
        {
            case GameCanvasState.WaitForReady:
                {
                    break;
                }
            case GameCanvasState.Gaming:
                {
                    break;
                }
        }
    }

    /// <summary>
    /// 骰子放置按钮组点击委托设置
    /// </summary>
    /// <param name="del">要设置的委托</param>
    public void SetPutDiceDelegate(DiceButtonControlDel del)
    {
        _canvas_Manager.SetPutDiceDelegate(del);
    }

    /// <summary>
    /// 开始按钮点击委托设置
    /// </summary>
    /// <param name="del">要设置的委托</param>
    public void SetStartButtonDelegate(StartButtonDel del)
    {
        _canvas_Manager.SetStartButtonDelegate(del);
    }

    /// <summary>
    /// 退出按钮点击委托设置
    /// </summary>
    /// <param name="del">要设置的委托</param>
    public void SetExitButtonDelegate(ExitButtonDel del)
    {
        _canvas_Manager.SetExitButtonDelegate(del);
    }
}