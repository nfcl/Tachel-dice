using UnityEngine;
using UnityEngine.UI;

public class Start_Canvas_SettingPannel_Manager : MonoBehaviour
{
    [SerializeField]
    private Animation   _anime;     //动画播放器
    [SerializeField]
    private InputField  _name;      //名称输入框
    [SerializeField]
    private Image       _avatar;    //当前头像
    [SerializeField]
    private GameObject  Prefab_Avatar;      //头像预设体
    [SerializeField]
    private RectTransform  Parent_Avatar;   //头像列表展示框

    public const int AVATARNUM = 1;        //头像数量

    private n_LocalPlayerData.Root _data;   //json读取缓冲区

    private void Start()
    {
        //根据头像数量更改头像列表展示区域的高度
        Parent_Avatar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (1 + (AVATARNUM - 1) / 4) * (128 + 33.6f) + 33.6f);

        float __startX, __startY;
        GameObject __clone;
        __startX = 33.6f + 64 - Parent_Avatar.rect.width / 2;
        __startY = -64 - 33.6f;

        //生成头像列表
        for (int i = 1; i <= AVATARNUM; ++i)
        {
            __clone = GameObject.Instantiate(Prefab_Avatar, Parent_Avatar.gameObject.transform);
            __clone.transform.localPosition = new Vector3(__startX, __startY, 0);
            __clone.transform.Find("avatar").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Avatar/{i}");
            ButtonAddListener(__clone.transform.Find("avatar").GetComponent<Button>(), i);
            __clone.transform.Find("BeSelected").gameObject.SetActive(false);
            __clone.name = i.ToString();
            __startX += (33.6f + 128);
            if (i % 4 == 0)
            {
                __startX = 33.6f + 64 - Parent_Avatar.rect.width / 2;
                __startY -= (33.6f + 128);
            }
        }
    }

    /// <summary>
    /// 循环参数地址不变
    /// 绑定到按钮上的值都是一样的
    /// 需要这样才行
    /// </summary>
    /// <param name="goal">要绑定到的按钮</param>
    /// <param name="cs">参数</param>
    private void ButtonAddListener(Button goal,int cs)
    {
        goal.onClick.AddListener(delegate { SelectAvatar(cs); });
    }

    //场景事件绑定
    //{

            public void Button_Init()       => Init();      //点击头像调出界面初始化
            public void Button_Submit()     => Submit();    //点击确定确认更改
            public void Button_Cancel()     => Cancel();    //点击确定确认更改

            public void TextInput_NameChange(string newName) => NameChange(newName);    //更改文本输入框内容修改名称

    //}

    /// <summary>
    /// 初始化
    /// 调出界面的方法
    /// </summary>
    private void Init()
    {
        //读取json中的数据
        _data = Tool.JsonReader<n_LocalPlayerData.Root>("LocalPlayerData.json");
        //对名称重新赋值
        _name.text = _data.LocalPlayerData.Name;
        //如果头像id不合法重置
        if (_data.LocalPlayerData.Avatar < 1 || _data.LocalPlayerData.Avatar > AVATARNUM)
            _data.LocalPlayerData.Avatar = 1;
        //更新头像
        UpdateAvatar();
        //显示头像列表中对应头像的选择图标
        UpdateSelectAvatar(_data.LocalPlayerData.Avatar, true);
        //播放界面出现动画
        Tool.PlayAnimation(_anime, "Setting_Pannel_show", 1);
    }

    /// <summary>
    /// 更改名称
    /// </summary>
    /// <param name="newName">新的名称</param>
    private void NameChange(string newName)
    {
        if (newName.Length == 0)
        //更改后的名称为空不合法
        {
            //变为更改前的名称
            _name.text = _data.LocalPlayerData.Name;
        }
        else
        //更改后的名称合法
        {
            //更改数据
            _data.LocalPlayerData.Name = newName;
        }
    }

    /// <summary>
    /// 选择头像
    /// </summary>
    /// <param name="index">新的头像id</param>
    private void SelectAvatar(int index = 1)
    {
        //如果和已选中的相同则不需要修改
        if(_data.LocalPlayerData.Avatar == index)
        {
            return;
        }
        //隐藏旧的头像选中图标
        UpdateSelectAvatar(_data.LocalPlayerData.Avatar, false);
        //更改数据
        _data.LocalPlayerData.Avatar = index;
        //显示新的头像选中图标
        UpdateSelectAvatar(_data.LocalPlayerData.Avatar, true);
        //更新左侧的头像
        UpdateAvatar();
    }

    /// <summary>
    /// 更新选中的头像
    /// </summary>
    private void UpdateAvatar()
    {
        //从Resources读取头像
        _avatar.sprite = Resources.Load<Sprite>($"Avatar/{_data.LocalPlayerData.Avatar}");
    }

    /// <summary>
    /// 更新头像的选中图标
    /// </summary>
    /// <param name="index">选择的头像</param>
    /// <param name="isSelected">是否被选中</param>
    private void UpdateSelectAvatar(int index, bool isSelected)
    {
        Parent_Avatar.transform.Find(index.ToString()).Find("BeSelected").gameObject.SetActive(isSelected);
    }

    /// <summary>
    /// 更改名称和头像完成并确定
    /// </summary>
    private void Submit()
    {
        //名称不能为空
        if (_data.LocalPlayerData.Name.Length == 0) return;
        //保存数据
        Tool.JsonWriter("LocalPlayerData.json", _data);
        //更新其他地方的头像和名称
        Start_Canvas_Manager.instance.UpdateLocalPlayerData();
        //关闭界面
        Cancel();
    }

    /// <summary>
    /// 取消更改
    /// </summary>
    private void Cancel()
    {
        //隐藏选中的头像选中图标
        UpdateSelectAvatar(_data.LocalPlayerData.Avatar, false);
        //播放界面隐藏动画
        Tool.PlayAnimation(_anime, "Setting_Pannel_show", -1);
    }
}
