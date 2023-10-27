using CrowEngineEditor___Cross_Platform_Desktop_Exporter;

namespace CrowEngineUI.ViewModels;

public class MonoGameViewModel
{
    
    private Game1 _game = new Game1();
    public Game1 Game
    {
        get => _game;
        set => _game = value;
    }
    
}