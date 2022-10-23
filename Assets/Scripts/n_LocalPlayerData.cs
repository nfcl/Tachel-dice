namespace n_LocalPlayerData
{
    public class LocalPlayerData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public int Avatar { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public LocalPlayerData LocalPlayerData { get; set; }
    }
}