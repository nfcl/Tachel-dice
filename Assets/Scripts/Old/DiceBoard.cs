using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// 骰子盘
/// </summary>
public class DiceBoard
{
    private int         _randomDiceValue;       //随机骰子点数
    private int[]       _slots;                 //场上的18个骰子槽位
    private int[]       _lineGrades;            //每列的骰子分数
    private GameState   _state;                 //游戏状态

    public int          RandomDiceValueNow      => _randomDiceValue;
    public int[]        Slots                   => _slots;
    public int[]        LineGrades              => _lineGrades;
    public GameState    State                   => _state;

    /// <summary>
    /// 游戏的状态
    /// </summary>
    public enum GameState
    {
        WaitForPlayer,      //等待玩家至2个
        Player1,            //玩家1的回合
        Player2,            //玩家2的回合
        End                 //有一方的骰子数打到9个结束游戏
    }

    public DiceBoard()
    { 
        _state = GameState.WaitForPlayer;

        _slots = new int[18] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        _lineGrades = new int[6] { 0, 0, 0, 0, 0, 0 };

        ChangeRandomDice();
    }

    public DiceBoard(int[] slots, int[] lineGrades,int randomValue)
    {
        _slots = slots;
        _lineGrades = lineGrades;
        _randomDiceValue = randomValue;
    }

    /// <summary>
    /// 游戏开始初始化
    /// </summary>
    public void StartGame()
    {
        //更改状态
        _state = GameState.Player1;
        //初始化场上的骰子数值
        for (int i = 0; i < 18; ++i) _slots[i] = 0;
        //初始化列分数
        for (int i = 0; i < 6; ++i) _lineGrades[i] = 0;
        //初始化随机骰子
        ChangeRandomDice();
    }

    /// <summary>
    /// 检查是否有一方填满骰子
    /// </summary>
    /// <returns>返回游戏结束条件是否成立</returns>
    public bool CheckGameEnd()
    {
        int __num1, __num2;

        __num1 = 0;
        __num2 = 0;

        for (int i = 0; i < 9; ++i)
        {
            __num1 += Slots[i] != 0 ? 1 : 0;
            __num2 += Slots[i + 9] != 0 ? 1 : 0;
        }

        return __num1 == 9 || __num2 == 9;
    }

    /// <summary>
    /// 摇随机骰子
    /// </summary>
    public void ChangeRandomDice()
    {
        _randomDiceValue = Random.Range(1, 7);
    }

    /// <summary>
    /// 在指定位置放入摇出的随机骰子
    /// </summary>
    /// <param name="pos">放置骰子的位置</param>
    /// <returns>返回放置是否成功</returns>
    public bool PutDice(int pos)
    {
        //检测当前槽位是否已放置骰子
        if (_slots[pos] != 0) return false;
        //将随机骰子放入
        _slots[pos] = _randomDiceValue;
        //擦除对方相应列的相同点数骰子
        EraseSame(_randomDiceValue, pos > 8 ? pos % 3 : pos % 3 + 3);
        //计算双方分数
        CalculateGrade(pos % 3 + 3);
        CalculateGrade(pos % 3);

        return true;
    }

    /// <summary>
    /// 回合转换
    /// </summary>
    public void ChangeTurn()
    {
        if(State == GameState.Player1)
        {
            _state = GameState.Player2;
        }
        else if(State == GameState.Player2)
        {
            _state = GameState.Player1;
        }
    }

    /// <summary>
    /// 擦除某列和输入点数相同的骰子
    /// </summary>
    /// <param name="value">放置的骰子点数</param>
    /// <param name="pos">放置的位子0-17</param>
    private void EraseSame(int value, int linePos)
    {
        if (linePos < 3)
        {
            for (int i = linePos; i < 9; i += 3)
            {
                if (_slots[i] == value)
                {
                    _slots[i] = 0;
                }
            }
        }
        else
        {
            for (int i = linePos + 6; i < 18; i += 3)
            {
                if (_slots[i] == value)
                {
                    _slots[i] = 0;
                }
            }
        }
    }

    /// <summary>
    /// 计算某列当前分数
    /// </summary>
    /// <param name="linePos">列坐标</param>
    private void CalculateGrade(int linePos)
    {
        //统计一列出现的骰子点数个数
        int[] valueNum = new int[] { 0, 0, 0, 0, 0, 0, 0 };

        if (linePos < 3)
        {
            for(int i = linePos; i < 9; i += 3)
            {
                valueNum[_slots[i]] += 1;
            }
        }
        else
        {
            for (int i = linePos + 6; i < 18; i += 3)
            {
                valueNum[_slots[i]] += 1;
            }
        }

        //清零
        _lineGrades[linePos] = 0;
        for (int i = 1; i <= 6; ++i)
        {
            if (valueNum[i] > 1)
            {//出现多个相同点数的骰子时进行翻倍
                _lineGrades[linePos] += (i + i) * valueNum[i];
            }
            else if (valueNum[i] == 1)
            {//单个直接相加
                _lineGrades[linePos] += i;
            }
        }
    }
}

public static class DiceBoardReaderWriter
{
    public static void WriteDiceBoard(this NetworkWriter writer, DiceBoard source)
    {
        writer.WriteArray<int>(source.Slots);
        writer.WriteArray<int>(source.LineGrades);
        writer.WriteInt(source.RandomDiceValueNow);
    }

    public static DiceBoard ReadDiceBoard(this NetworkReader reader)
    {
        return new DiceBoard(reader.ReadArray<int>(), reader.ReadArray<int>(), reader.ReadInt());
    }
}