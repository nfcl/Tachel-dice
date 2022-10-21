/// <summary>
/// Start_Canvas的状态
/// 用于界面Pannel的判断和转换
/// </summary>
public enum Start_Canvas_State
{
    Main                = 0b01,    //主菜单          选择模式和退出的地方
    DoubleModeSelect    = 0b10     //双人模式选择    选择作为房主还是加入其他人的游戏
}