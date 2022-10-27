using UnityEngine;
using Mirror;
using Game_Canvas;
using Unity.Collections.LowLevel.Unsafe;

/// <summary>
/// 服务端的游戏管理器
/// </summary>
public class GameCanvasServerManager : NetworkBehaviour,IGameCanvasPlayerControlConnect
{
    /// <summary>
    /// 网络管理器
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
                new GameCanvasPlayerData(),
                new GameCanvasPlayerData()
            };
            //创建新的游戏进程管理器
            _game_Manager = new GameCanvasGameManager();
        }
        //寻找场景物体管理器
        _canvas_Manager = transform.parent.GetComponent<GameCanvasManager>();
        //移动相机至游戏界面
        _canvas_Manager.Goto_GameCanvas();
        //将新的玩家信息添加到服务端的_playerData
        CmdPlayerChange(isServer, Tool.JsonReader<n_LocalPlayerData.Root>("LocalPlayerData.json"));
        //在各个客户端重新绘制玩家信息
        CmdDrawPlayerInfo();
        //场景初始化
        _canvas_Manager.SceneInit();
        //更新游戏数据显示信息
            CmdUpdateDiceData();
            CmdUpdateGradeData();
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
    /// 当有客户端断开连接时会在服务端调用这个方法
    /// </summary>
    /// <param name="conn">断开的客户端的NetworkConnection</param>
    private void OnServerDisconnect(NetworkConnection conn)
    {
        //服务端已关闭
        //单此条件时会在以下情况同时出现时中出现错误:
        //1.房主断开连接
        if (false == NetworkClient.active) return;
        //断开连接的是房主的客户端
        //单此条件时会在以下情况同时出现时中出现错误:
        //1.有其他玩家客户端连接到房主时
        //2.房主断开连接
        if (conn.connectionId == 0) return;
        //****************************************
        //** condition : 非房主玩家单独退出游戏 **
        //****************************************
        //如果当前正在游戏中需要结束游戏
        if (false == _game_Manager.IsGameEnd)
        {
            //服务端游戏数据初始化
            _game_Manager.Init();
            //更新游戏数据显示信息
            CmdUpdateDiceData();
            CmdUpdateGradeData();
            //显示准备按钮
            RpcSetReadyButtonVisible(true);
            //改变房主的准备状态
            _playerData[0].ChangeReadyState(false);
        }
        //提示玩家离开
        RpcShowTipText($"玩家1\n{_playerData[1].Name}\n离开");
        //清除非房主玩家数据
        _playerData[1].ClearPlayerInfo();
        //更新房主的非房主玩家信息
        RpcDrawPlayerInfo(true, _playerData[1]);
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
    /// 服务端debuglog输出方法
    /// </summary>
    /// <param name="content"></param>
    [Command(requiresAuthority =false)]
    public void CmdDebug(string content)
    {
        Debug.Log(content);
    }

    /// <summary>
    /// 指定玩家准备状态切换
    /// </summary>
    /// <param name="isHost">指定的玩家</param>
    [Server]
    public void PlayerReady(bool isHost)
    {
        //检测是否还在游戏结束状态
        if (true == _game_Manager.IsGameEnd)
        {
            //服务端游戏数据初始化
            _game_Manager.Init();
            //重新绘制服务端骰子和分数的显示
            CmdUpdateDiceData();
            CmdUpdateGradeData();
        }
        //切换服务端中的玩家准备状态
        //切换客户端准备按钮的图片
        if (true == isHost)
        {
            _playerData[0].ChangeReadyState();
            SetPlayerReadyState(true, _playerData[0].IsReady);
        }
        else
        {
            _playerData[1].ChangeReadyState();
            SetPlayerReadyState(false, _playerData[1].IsReady);
        }
        //计算已准备的人数
        int ReadyPlayerNum = 0;
        ReadyPlayerNum += (_playerData[0] is null || false == _playerData[0].IsReady) ? 0 : 1;
        ReadyPlayerNum += (_playerData[1] is null || false == _playerData[1].IsReady) ? 0 : 1;
        //判断是否都准备好了
        if (2 != ReadyPlayerNum)
        {
            //更改提示信息为准备好的人数
            RpcShowTipText($"当前已有\n{ReadyPlayerNum}/2\n个玩家已准备");
        }
        else
        {
            //隐藏准备按钮
            RpcSetReadyButtonVisible(false);
            //切换回合
            _game_Manager.GameStart(true);
            //更改提示信息游戏开始
            RpcShowTipText("游戏开始");
        }
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
            for (int i = 0; i < 18; ++i)
            {
                RpcSetDiceValue(i, _game_Manager.DiceSlots[i]);
            }
            //更新分数信息
            CmdUpdateGradeData();
            //更新下一个骰子
            RpcSetNextDiceValue(_game_Manager.NextPutDiceValue);
            //检测是否结束游戏
            if (true == _game_Manager.IsGameEnd)
            {
                //显示准备按钮
                RpcSetReadyButtonVisible(true);
                //提示文本显示为"{获胜玩家}胜利！"
                RpcShowTipText($"玩家{_game_Manager.GameWin()}胜利！");
                //隐藏下一个骰子的显示
                RpcSetNextDiceValue(0);
            }
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
            RpcSetDiceValue(i,  _game_Manager.DiceSlots[i]);
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
    /// <para/>设置下一个要放置的骰子点数
    /// </summary>
    /// <param name="value">要放置的点数,0为隐藏</param>
    [ClientRpc]
    public void RpcSetNextDiceValue(int value)
    {
        _canvas_Manager.SetNextDiveValue(value);
    }

    /// <summary>
    /// 设置列分数
    /// </summary>
    /// <param name="linePos">设置的列分数位置</param>
    /// <param name="value">设置的分数</param>
    [ClientRpc]
    public void RpcSetDiceLineGrade(int linePos,int value)
    {
        _canvas_Manager.SetDiceLineGrade(linePos, value);
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

    /// <summary>
    /// 客户端设置准备按钮是否可见
    /// </summary>
    /// <param name="isVisible">是否可见</param>
    [ClientRpc]
    public void RpcSetReadyButtonVisible(bool isVisible)
    {
        _canvas_Manager.SetReadyButtonVisible(isVisible);
    }

    /// <summary>
    /// 所有客户端展示相同提示文本
    /// </summary>
    /// <param name="content">要展示的内容</param>
    [ClientRpc]
    public void RpcShowTipText(string content)
    {
        _canvas_Manager.ShowTipText(content);
    }

    /// <summary>
    /// 向指定客户端展示提示文本
    /// </summary>
    /// <param name="conn">要展示的内容</param>
    /// <param name="content">指定客户端的连接</param>
    [TargetRpc]
    public void TargetShowTipText(NetworkConnection conn, string content)
    {
        _canvas_Manager.ShowTipText(content);
    }

    /// <summary>
    /// 切换指定玩家的准备按钮
    /// </summary>
    /// <param name="conn">指定的玩家</param>
    /// <param name="isReady">是否准备</param>
    [ClientRpc]
    public void SetPlayerReadyState(bool isHost, bool isReady)
    {
        if (false == (true == isHost ^ true == isServer))
            _canvas_Manager.SetReadyState(isReady);
    }

    /// <summary>
    /// 设置骰子放置事件
    /// </summary>
    public void SetPutDiceDelegate(bool isHost, DiceButtonControlDel del)
    {
        _canvas_Manager.SetPutDiceDelegate(isHost, del);
    }

    /// <summary>
    /// 设置开始按钮事件
    /// </summary>
    public void SetStartButtonDelegate(StartButtonDel del)
    {
        _canvas_Manager.SetStartButtonDelegate(del);
    }

    /// <summary>
    /// 设置退出按钮事件
    /// </summary>
    public void SetExitButtonDelegate(ExitButtonDel del)
    {
        _canvas_Manager.SetExitButtonDelegate(del);
    }
}