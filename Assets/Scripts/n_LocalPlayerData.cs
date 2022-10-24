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

/// <summary>
/// n_LocalPlayerData.LocalPlayerData的序列化和反序列化类
/// </summary>
public static class n_LocalPlayerData_LocalPlayerDataReaderWriter
{
    public static void Write_n_LocalPlayerData_LocalPlayerData(this Mirror.NetworkWriter writer, n_LocalPlayerData.LocalPlayerData source)
    {
        writer.Write(source.Name);
        writer.Write(source.Avatar);
    }

    public static n_LocalPlayerData.LocalPlayerData Read_n_LocalPlayerData_LocalPlayerData(this Mirror.NetworkReader reader)
    {
        return new n_LocalPlayerData.LocalPlayerData { Name = reader.Read<string>(), Avatar = reader.Read<int>() };
    }
}

/// <summary>
/// n_LocalPlayerData.Root的序列化和反序列化类
/// </summary>
public static class n_LocalPlayerData_RootReaderWriter
{
    public static void Write_n_LocalPlayerData_Root(this Mirror.NetworkWriter writer, n_LocalPlayerData.Root source)
    {
        writer.Write(source.LocalPlayerData);
    }

    public static n_LocalPlayerData.Root Read_n_LocalPlayerData_Root(this Mirror.NetworkReader reader)
    {
        return new n_LocalPlayerData.Root { LocalPlayerData = reader.Read<n_LocalPlayerData.LocalPlayerData>() };
    }
}