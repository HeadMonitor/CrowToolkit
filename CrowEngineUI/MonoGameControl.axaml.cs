using Avalonia.Controls;
using Avalonia.Media;
using CrowEngineEditor___Cross_Platform_Desktop_Exporter;
using Microsoft.Xna.Framework;

namespace CrowEngineUI;

public class MonoGameControl : Control
{
    public MonoGameControl()
    {
        // Initialize the MonoGame framework
        Game game = new Game1(); 
        game.Run();
    }

    public override void Render(DrawingContext context)
    {
        // Since MonoGame has its own rendering loop, you don't need to explicitly call Draw here.
        // The game.Run() call in the constructor will start the MonoGame rendering loop.
        // However, you can add logic here to render Avalonia UI elements on top of the MonoGame content if needed.
    }
}