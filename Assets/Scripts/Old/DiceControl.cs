using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceControl
{
    //放置在场上的骰子
    private         GameObject[]    _putDice;
    //随机骰子
    private         GameObject      _randomDice;    
    //骰子的值和实体旋转角度的映射
    private static  List<Vector3>   DiceValue_localEulerAngles = new List<Vector3>() {
        new Vector3(0   ,0   ,0   ), 
        new Vector3(-90 ,0   ,0   ),
        new Vector3(0   ,0   ,180 ), 
        new Vector3(0   ,0   ,-90 ), 
        new Vector3(0   ,0   ,0   ),
        new Vector3(0   ,0   ,90  ),
        new Vector3(90  ,0   ,0   )
    };

    public DiceControl(GameObject[] putDice, GameObject randomDice)
    {
        _putDice = putDice;
        _randomDice = randomDice;
    }

    ~DiceControl()
    {
        _putDice = null;
        _randomDice = null;
    }

    /// <summary>
    /// 根据值更改场上的骰子实体
    /// </summary>
    /// <param name="pos">更改的骰子位置</param>
    /// <param name="value">要更改为的值</param>
    public void ChangePutDiceValue(int pos, int value)
    {
        if (pos < 0 || pos > 17) return;
        if (value < 0 || value > 6) return;
        if (value == 0)
        {
            _putDice[pos].SetActive(false);
        }
        else
        {
            _putDice[pos].SetActive(true);
            _putDice[pos].transform.localEulerAngles = DiceValue_localEulerAngles[value];
        }
    }

    /// <summary>
    /// 根据值更改随机骰子的值
    /// </summary>
    /// <param name="value">要更改为的值</param>
    public void ChangeRandomDiceValue(int value)
    {
        if (value < 0 || value > 6) return;
        if (value == 0)
        {
            _randomDice.SetActive(false);
        }
        else
        {
            _randomDice.SetActive(true);
            _randomDice.transform.localEulerAngles = DiceValue_localEulerAngles[value];
        }
    }
}
