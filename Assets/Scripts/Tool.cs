using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tool
{
    public static void PlayAnimation(Animation anime, string anime_name, float speed)
    {
        if (speed > 0)
            anime[anime_name].time = 0; 
        else
            anime[anime_name].time = anime[anime_name].length;
        anime[anime_name].speed = speed;
        anime.Play(anime_name);
    }

    /// <summary>
    /// 将path位置的json文件读取为json字符串并转换为json对象
    /// </summary>
    /// <param name="path">读入的json文件地址</param>
    public static T JsonReader<T>(string path)
    {
        System.IO.StreamReader sr = new System.IO.StreamReader($"{Application.streamingAssetsPath}/{path}");
        string str = sr.ReadToEnd();
        sr.Close();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
    }

    public static void JsonWriter<T>(string path, T data)
    {
        System.IO.StreamWriter sw = new System.IO.StreamWriter($"{Application.streamingAssetsPath}/{path}");
        sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        sw.Close();
    }
}
