using UnityEngine;

public class GameCanvasDiceBoardProcess
{
    private int     _nextRandomDiceValue;       //下一个骰子       
    private int[]   _diceSlots;                 //骰子槽位的值,0表示没有骰子  
    private int[]   _lineGrades;                //列分数

    public int[]    DiceSlots               => _diceSlots;
    public int[]    LineGrades              => _lineGrades;
    public int      NextRandomDiceValue     => _nextRandomDiceValue;

    public GameCanvasDiceBoardProcess()
    {
        DrawNextDice();
        _diceSlots = new int[18];
        _lineGrades = new int[6];
    }

    /// <summary>
    /// 外部调用在指定位置放置骰子
    /// </summary>
    /// <param name="slotPos">指定位置</param>
    /// <returns></returns>
    public bool PutDice(int slotPos)
    {
        if(PutDice(slotPos, _nextRandomDiceValue))
        {//放置成功
            DrawNextDice();
            return true;
        }
        else
        {//放置失败
            return false;
        }
    }

    public void Init()
    {
        for(int i = 0; i < 18; ++i)
        {
            _diceSlots[i] = 0;
        }
        for(int i = 0; i < 6; ++i)
        {
            _lineGrades[i] = 0;
        }
    }

    private bool PutDice(int slotPos,int value)
    {
        if (slotPos < 0 || slotPos > 17) return false;
        if (value < 1 || value > 6) return false;
        if (_diceSlots[slotPos] != 0) return false;

        _diceSlots[slotPos] = _nextRandomDiceValue;

        EraseSame(slotPos % 3 + (slotPos < 9 ? 3 : 0), _nextRandomDiceValue);

        Calculate_lineGrades(slotPos % 3);
        Calculate_lineGrades(slotPos % 3 + 3);

        return true;
    }

    private void EraseSame(int linePos,int value)
    {
        if (linePos < 0 || linePos > 5) return;

        if (linePos < 3)
        {
            for (int __i = linePos; __i < 9; __i += 3)
            {
                if(_diceSlots[__i] == value)
                {
                    _diceSlots[__i] = 0;
                }
            }
        }
        else
        {
            for (int __i = linePos + 6; __i < 18; __i += 3)
            {
                if (_diceSlots[__i] == value)
                {
                    _diceSlots[__i] = 0;
                }
            }
        }
    }

    private static int[] _Calculate_lineGrades_ValueNum = new int[7];

    private void Calculate_lineGrades(int linePos)
    {
        if (linePos < 0 || linePos > 5) return;

        for (int __i = 0; __i < 7; ++__i)
        {
            _Calculate_lineGrades_ValueNum[__i] = 0;
        }

        if (linePos < 3)
        {
            for (int __i = linePos; __i < 9; __i += 3)
            {
                _Calculate_lineGrades_ValueNum[_diceSlots[__i]] += 1;
            }
        }
        else
        {
            for (int __i = linePos + 6; __i < 18; __i += 3)
            {
                _Calculate_lineGrades_ValueNum[_diceSlots[__i]] += 1;
            }
        }

        _lineGrades[linePos] = 0;

        for(int i = 0; i < 7; ++i)
        {
            _lineGrades[linePos] += (i + i) * _Calculate_lineGrades_ValueNum[i];
        }
    }

    private void DrawNextDice()
    {
        _nextRandomDiceValue = Random.Range(1, 7);
    }
}