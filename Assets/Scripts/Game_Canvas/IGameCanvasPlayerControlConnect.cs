namespace Game_Canvas
{
    /// <summary>
    /// 骰子放置按钮点击委托
    /// </summary>
    /// <param name="pos">放置的位置</param>
    public delegate void DiceButtonControlDel(int pos);
    public delegate void StartButtonDel();
    public delegate void ExitButtonDel();
    /// <summary>
    /// 玩家控制连接接口
    /// 用于将player的操作连接到场景实体
    /// </summary>
    public interface IGameCanvasPlayerControlConnect
    {
        /// <summary>
        /// 设置骰子放置事件
        /// </summary>
        void SetPutDiceDelegate(DiceButtonControlDel del);
        /// <summary>
        /// 设置开始按钮事件
        /// </summary>
        void SetStartButtonDelegate(StartButtonDel del);
        /// <summary>
        /// 设置退出按钮事件
        /// </summary>
        void SetExitButtonDelegate(ExitButtonDel del);
    }
}

