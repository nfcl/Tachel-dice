/// <summary>
/// Game_Canvas中游戏的状态
/// </summary>
public enum GameCanvasGameState
{
    Null        = 0b0000,       //初始化
    Player1Turn = 0b0001,       //玩家1的回合
    Player2Turn = 0b0010,       //玩家2的回合
    Player1Win  = 0b0100,       //玩家1获得胜利
    Player2Win  = 0b1000        //玩家2获得胜利
}