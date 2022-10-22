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

        public string   Name        =>  _name;
        public int      Avatar      =>  _avatar;
        public bool     IsReady     =>  _isReady;

        public GameCanvasPlayerData(n_LocalPlayerData.Root source, bool isready = false)
        {
            _name       = source.LocalPlayerData.Name;
            _avatar     = source.LocalPlayerData.Avatar;
            _isReady    = isready;
        }

        public GameCanvasPlayerData(string name, int avatar, bool isready = false)
        {
            _name       = name;
            _avatar     = avatar;
            _isReady    = isready;
        }

        /// <summary>
        /// 玩家准备
        /// </summary>
        public void PlayerReady()       => _isReady = true;
        /// <summary>
        /// 玩家取消准备
        /// </summary>
        public void PlayerDisReady()    => _isReady = false;
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
        }
        else
        {
            writer.Write(source.Name);
            writer.Write(source.Avatar);
            writer.Write(source.IsReady);
        }
    }

    public static Game_Canvas.GameCanvasPlayerData Read_GameCanvasPlayerData(this Mirror.NetworkReader reader)
    {
        return new Game_Canvas.GameCanvasPlayerData(reader.Read<string>(), reader.Read<int>(),reader.Read<bool>());
    }
}