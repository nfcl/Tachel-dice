using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Canvas_PannelChange
{
    private Start_Canvas_State   _last_State;
    private Animation[]         _anime;

    public Start_Canvas_PannelChange()
    {
        _anime = new Animation[]
        {
            GameObject.Find("Start_Pannel").GetComponent<Animation>() ,
            GameObject.Find("Double_Mode_Pannel").GetComponent<Animation>()
        };
        _last_State = Start_Canvas_State.Main;
    }

    public void Change(Start_Canvas_State new_State)
    {
        if (new_State == _last_State) return;
        AnimationPlay(_last_State, -1);
        AnimationPlay(new_State, 1);
        _last_State = new_State;
    }

    private void AnimationPlay(Start_Canvas_State state, int speed)
    {
        switch (state)
        {
            case Start_Canvas_State.Main:
                {
                    Tool.PlayAnimation(_anime[0], "Start_Pannel_Show", speed);
                    break;
                }
            case Start_Canvas_State.DoubleModeSelect:
                {
                    Tool.PlayAnimation(_anime[1], "DoubleMode_Pannel_show", speed);
                    break;
                }
        }
    }
}