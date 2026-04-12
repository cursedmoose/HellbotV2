using Hellbot.Core.Scenes;

namespace Hellbot.Service.Clients.OBS
{
    public class SceneManager
    {
        public const string Main = "Main Scene";
        public const string Characters = "Characters";
        public const string Dice = "Dice";
        public const string Commemoration = "Commemoration";
        public static Dictionary<string, SceneItem> GetScenes()
        {
            return new()
            {
                ["Main:Ads"] = new SceneItem(Main, 10),
                ["Main:Commemoration"] = new SceneItem(Main, 32),
                ["Character:Sheogorath"] = new SceneItem(Characters, 1),
                ["Character:AdoringFan"] = new SceneItem(Characters, 3),
                ["Character:DagothUr"] = new SceneItem(Characters, 2),
                ["Character:Maiq"] = new SceneItem(Characters, 4),
                ["Character:Werner"] = new SceneItem(Characters, 6),
                ["Character:God"] = new SceneItem(Characters, 8),
            };
    }
    }
}
