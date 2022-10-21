/// <summary>
/// <para/>游戏进程管理器
/// <para/>用于处理游戏数据
/// </summary>
public class GameCanvasGameManager
{
    private int                         _nextPutDiceValue;          //下一个放置的骰子       
    private int[]                       _diceSlots;                 //骰子槽位的值,0表示没有骰子  
    private int[]                       _lineGrades;                //列分数
    private GameCanvasGameState         _state;                     //游戏状态

    public int[]                        DiceSlots               => _diceSlots;
    public int[]                        LineGrades              => _lineGrades;
    public int                          NextPutDiceValue        => _nextPutDiceValue;
    public int                          Player1Grade            => _lineGrades[0] + _lineGrades[1] + _lineGrades[2]; 
    public int                          Player2Grade            => _lineGrades[3] + _lineGrades[4] + _lineGrades[5];

    public GameCanvasGameManager()
    {
        //初始化骰子槽位
        _diceSlots = new int[18];
        //初始化列分数
        _lineGrades = new int[6];
        //初始化数据
        Init();
        //设置游戏结束callBack
    }

    /// <summary>
    /// 游戏重置
    /// </summary>
    public void Init()
    {
        _state = GameCanvasGameState.Null;
        for (int i = 0; i < 18; ++i) _diceSlots [i] = 0;
        for (int i = 0; i <  6; ++i) _lineGrades[i] = 0;
        DrawNextDice();
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="Player1Start">是玩家1先手</param>
    public void GameStart(bool Player1Start)
    {
        if (Player1Start)
        {
            _state = GameCanvasGameState.Player1Turn;
        }
        else
        {
            _state = GameCanvasGameState.Player2Turn;
        }
    }

    public void ChangeTurn()
    {
        if(_state == GameCanvasGameState.Player1Turn)
        {
            _state = GameCanvasGameState.Player2Turn;
        }
        else if(_state == GameCanvasGameState.Player2Turn)
        {
            _state = GameCanvasGameState.Player1Turn;
        }
    }

    /// <summary>
    /// 外部调用在指定位置放置骰子
    /// </summary>
    /// <param name="slotPos">指定位置</param>
    /// <returns>返回是否放置成功</returns>
    public bool PutDice(int slotPos)
    {
        if (slotPos < 9)
            if (_state != GameCanvasGameState.Player1Turn) return false;
        else
            if (_state != GameCanvasGameState.Player2Turn) return false;

        if (PutDice(slotPos, _nextPutDiceValue))
        {//放置成功
            DrawNextDice();

            if(true == CheckFull())
            {
                _state = Player1Grade > Player2Grade ? GameCanvasGameState.Player1Win : GameCanvasGameState.Player2Win;
            }

            return true;
        }
        else
        {//放置失败
            return false;
        }

    }

    /// <summary>
    /// 放置骰子
    /// </summary>
    /// <param name="slotPos">放置的位置</param>
    /// <param name="value">放置的骰子点数</param>
    /// <returns>返回是否放置成功</returns>
    private bool PutDice(int slotPos, int value)
    {
        if (slotPos < 0 || slotPos > 17) return false;
        if (value < 1 || value > 6) return false;
        if (_diceSlots[slotPos] != 0) return false;

        _diceSlots[slotPos] = _nextPutDiceValue;

        EraseSame(slotPos % 3 + (slotPos < 9 ? 3 : 0), _nextPutDiceValue);

        Calculate_lineGrades(slotPos % 3);
        Calculate_lineGrades(slotPos % 3 + 3);

        return true;
    }

    /// <summary>
    /// 擦除一列中具有相同点数的骰子
    /// </summary>
    /// <param name="linePos">需要擦除的列</param>
    /// <param name="value">需要擦除的点数</param>
    private void EraseSame(int linePos, int value)
    {
        //检测列位置合法性
        if (linePos < 0 || linePos > 5) return;
        //循环变量定义
        int __CircArg_i;
        //遍历对应列的所有骰子并将相同点数的骰子置0
        if (linePos < 3)
        {
            for (__CircArg_i = linePos; __CircArg_i < 9; __CircArg_i += 3)
            {
                if (_diceSlots[__CircArg_i] == value)
                {
                    _diceSlots[__CircArg_i] = 0;
                }
            }
        }
        else
        {
            for (__CircArg_i = linePos + 6; __CircArg_i < 18; __CircArg_i += 3)
            {
                if (_diceSlots[__CircArg_i] == value)
                {
                    _diceSlots[__CircArg_i] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Calculate_lineGrades用于统计每列骰子对应点数数量的数组
    /// </summary>
    private static int[] _Calculate_lineGrades_ValueNum = new int[7];

    /// <summary>
    /// 计算一列的分数
    /// </summary>
    /// <param name="linePos">需要计算的列</param>
    private void Calculate_lineGrades(int linePos)
    {
        //检测列位置合法性
        if (linePos < 0 || linePos > 5) return;
        //循环变量定义
        int __CircArg_i;
        //重置骰子计数数组
        for (__CircArg_i = 0; __CircArg_i < 7; ++__CircArg_i)
        {
            _Calculate_lineGrades_ValueNum[__CircArg_i] = 0;
        }
        //骰子计数
        if (linePos < 3)
        {
            for (__CircArg_i = linePos; __CircArg_i < 9; __CircArg_i += 3)
            {
                _Calculate_lineGrades_ValueNum[_diceSlots[__CircArg_i]] += 1;
            }
        }
        else
        {
            for (__CircArg_i = linePos + 6; __CircArg_i < 18; __CircArg_i += 3)
            {
                _Calculate_lineGrades_ValueNum[_diceSlots[__CircArg_i]] += 1;
            }
        }
        //重置对应列分数
        _lineGrades[linePos] = 0;
        //计算列分数
        for (__CircArg_i = 0; __CircArg_i < 7; ++__CircArg_i)
        {
            _lineGrades[linePos] += (__CircArg_i + __CircArg_i) * _Calculate_lineGrades_ValueNum[__CircArg_i];
        }
    }

    /// <summary>
    /// 投掷下一个骰子
    /// </summary>
    private void DrawNextDice()
    {
        //投掷范围1~6,平均分布
        _nextPutDiceValue = UnityEngine.Random.Range(1, 7);
    }

    /// <summary>
    /// 检测是否有一方放满了
    /// </summary>
    /// <returns>返回是否放满了</returns>
    private bool CheckFull()
    {
        int __CircArg_i;

        for (__CircArg_i = 0; __CircArg_i < 9; ++__CircArg_i)
        {
            if (_diceSlots[__CircArg_i] == 0)
                goto _GOTOTAG_CHECKFULL_ELSE_;
        }
        return true;

    _GOTOTAG_CHECKFULL_ELSE_:

        for (__CircArg_i = 9; __CircArg_i < 18; ++__CircArg_i)
        {
            if (_diceSlots[__CircArg_i] == 0)
                return false;
        }

        return true;
    }
}
