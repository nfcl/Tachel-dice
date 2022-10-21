namespace Game_Canvas
{
    public class GameCanvasPlayerData
    {
        private string _name;
        private int _avatar;

        public string Name { get => _name; }
        public int Avatar { get => _avatar; }

        private GameCanvasPlayerData() { }

        public GameCanvasPlayerData(n_LocalPlayerData.Root source)
        {
            _name = source.LocalPlayerData.Name;
            _avatar = source.LocalPlayerData.Avatar;
        }

        public GameCanvasPlayerData(string name, int avatar)
        {
            _name = name;
            _avatar = avatar;
        }
    }
}

public static class Game_Canvas_GameStateReaderWriter
{
    public static void Write_LocalPlayerData_Root(this Mirror.NetworkWriter writer, Game_Canvas.GameCanvasPlayerData source)
    {
        if (source is null)
        {
            writer.Write("null");
            writer.Write(1);
        }
        else
        {
            writer.Write(source.Name);
            writer.Write(source.Avatar);
        }
    }

    public static Game_Canvas.GameCanvasPlayerData Readn_LocalPlayerData_Root(this Mirror.NetworkReader reader)
    {
        return new Game_Canvas.GameCanvasPlayerData(reader.Read<string>(), reader.Read<int>());
    }
}