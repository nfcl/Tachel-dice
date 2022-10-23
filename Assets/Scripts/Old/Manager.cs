using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Manager : NetworkBehaviour
{
    /// <summary>
    /// 场上的骰子数据
    /// 只有服务端是实例化的
    /// </summary>
    public DiceBoard _diceBoard;
    /// <summary>
    /// 控制场上和随机骰子实体
    /// </summary>
    public DiceControl _dicecontrol;
    /// <summary>
    /// 列分数文本框
    /// </summary>
    public Text[] LineGradesInScene;
    /// <summary>
    /// 总分数文本框
    /// </summary>
    public Text[] SumGradesInScene;
    /// <summary>
    /// 服务端初始化
    /// </summary>
    public override void OnStartServer()
    {
        //初始化棋盘数据
        _diceBoard = new DiceBoard();
    }
    /// <summary>
    /// 和场景中的物体做链接
    /// </summary>
    public void ConnectToScene()
    {

        GameObject[] DiceInScene = new GameObject[18];
        for (int i = 0; i < 18; ++i)
        {
            DiceInScene[i] = GameObject.Find($"DicePlane{(i < 9 ? 1 : 2)}").transform.Find(i.ToString()).gameObject;
        }
        _dicecontrol = new DiceControl(DiceInScene, GameObject.Find("NewDice").transform.Find("Dice").gameObject);



        LineGradesInScene = new Text[6];
        SumGradesInScene = new Text[2];
        for (int player = 1; player <= 2; ++player)
        {
            for (int i = 1; i <= 3; ++i)
            {
                LineGradesInScene[i - 1 + (player - 1) * 3] = GameObject.Find($"Canvas/Player{player}Grade/{i}").GetComponent<Text>();
            }
            SumGradesInScene[player - 1] = GameObject.Find($"Canvas/Player{player}Grade/Sum").GetComponent<Text>();
        }

    }

    public void GameStart()
    {
        for (int i = 0; i < 18; ++i)
        {
            _dicecontrol.ChangePutDiceValue(i, 0);
        }
        _dicecontrol.ChangeRandomDiceValue(0);
        foreach (Text item in LineGradesInScene)
        {
            item.text = "0";
        }
        foreach (Text item in SumGradesInScene)
        {
            item.text = "0";
        }
        GameObject.Find("Canvas/GameStart").SetActive(false);
    }

    public void ChangeDiceValueInScene(int pos, int value)
    {
        _dicecontrol.ChangePutDiceValue(pos, value);
    }

    public void ChangeRandomDiceValueInScene(int value)
    {
        _dicecontrol.ChangeRandomDiceValue(value);
    }

    public void ChangeLineGradeInScene(int LinePos, int value)
    {
        if (LinePos < 0 || LinePos > 5) return;

        LineGradesInScene[LinePos].text = value.ToString();
    }

    public void ChangeSumGradeInScene(int PlayerId, int value)
    {
        if (PlayerId < 1 || PlayerId > 2) return;

        SumGradesInScene[PlayerId - 1].text = value.ToString();
    }
}
