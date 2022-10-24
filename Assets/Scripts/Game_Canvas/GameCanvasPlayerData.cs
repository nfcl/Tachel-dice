using UnityEngine;

namespace Game_Canvas
{
    public class GameCanvasPlayerData
    {
        /// <summary>
        /// 昵称
        /// </summary>
        private string  _name;
        /// <summary>
        /// 头像序号
        /// </summary>
        private int     _avatar;
        /// <summary>
        /// 玩家是否准备
        /// </summary>
        private bool    _isReady;
        /// <summary>
        /// 当前存储的玩家信息有效
        /// </summary>
        private bool    _isPlayerExit;
        
        public string   Name            =>  _name;
        public int      Avatar          =>  _avatar;
        public bool     IsReady         =>  _isReady;
        public bool     IsPlayerExit    =>  _isPlayerExit;

        public GameCanvasPlayerData()
        {
            _name           = "null";
            _avatar         = 1;
            _isReady        = false;
            _isPlayerExit   = false;
        }

        /// <summary>
        /// 服务端与客户端传输数据用构造
        /// </summary>
        /// <param name="name">玩家姓名</param>
        /// <param name="avatar">玩家头像</param>
        /// <param name="isready">是否已准备</param>
        /// <param name="isPlayerExit">玩家是否有效</param>
        public GameCanvasPlayerData(string name, int avatar, bool isready = false, bool isPlayerExit = false)
        {
            _name           = name;
            _avatar         = avatar;
            _isReady        = isready;
            _isPlayerExit   = isPlayerExit;
        }

        /// <summary>
        /// 清除玩家信息
        /// </summary>
        public void ClearPlayerInfo()
        {
            _name           = "null";
            _avatar         = 1;
            _isReady        = false;
            _isPlayerExit   = false;
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        /// <param name="data">玩家信息</param>
        public void SetPlayerInfo(n_LocalPlayerData.Root data)
        {
            _name           = data.LocalPlayerData.Name;
            _avatar         = data.LocalPlayerData.Avatar;
            _isReady        = false;
            _isPlayerExit   = true;
        }

        /// <summary>
        /// <para/>切换准备状态
        /// <para/>不带参,切换为反状态
        /// </summary>
        public void ChangeReadyState()
        {
            _isReady = !_isReady;
        }
        /// <summary>
        /// <para/>切换准备状态
        /// <para/>带参,切换为参数状态
        /// </summary>
        /// <param name="isReady">是否准备</param>
        public void ChangeReadyState(bool isReady)
        {
            _isReady = isReady;
        }
    }
}

/// <summary>
/// GameCanvasPlayerData的序列化和反序列化类
/// </summary>
public static class GameCanvasPlayerDataReaderWriter
{
    public static void Write_GameCanvasPlayerData(this Mirror.NetworkWriter writer, Game_Canvas.GameCanvasPlayerData source)
    {
        if (source is null)
        {
            writer.Write("null");
            writer.Write(1);
            writer.Write(false);
            writer.Write(false);
        }
        else
        {
            writer.Write(source.Name);
            writer.Write(source.Avatar);
            writer.Write(source.IsReady);
            writer.Write(source.IsPlayerExit);
        }
    }

    public static Game_Canvas.GameCanvasPlayerData Read_GameCanvasPlayerData(this Mirror.NetworkReader reader)
    {
        return new Game_Canvas.GameCanvasPlayerData(reader.Read<string>(), reader.Read<int>(), reader.Read<bool>(), reader.Read<bool>());
    }
}