A Raylib_cs facade to mimic Java Processing syntax

Example usage:
```csharp
global using System.Numerics;
global using ProcessingCS;
global using static ProcessingCS.PApplet;
global using Raylib_cs;

namespace YourNamespace;

public class Program : PApplet
{
    public static void Main(string[] args)
    {
        var app = new Program();
        app.Run(640, 480, "MyApp");
        //app.RunFullScreen("MyApp");
    }
    
    protected override void Setup()
    {
        SetFramerate(60);
    }

    protected override void Draw()
    {
        Background(42); // It is recommended to clear the background, otherwise artifacts may appear
        
        Fill(255, 0, 0);
        NoStroke();
        Circle(MouseX, MouseY, 50);
    }

    protected override void KeyPressed(KeyboardKey key)
    {
        Console.WriteLine(key + " was pressed!");
    }

    protected override void KeyReleased(KeyboardKey key)
    {
        Console.WriteLine(key + " was released!");
    }

    protected override void MouseClicked(MouseButton mouseButton)
    {
        Console.WriteLine(mouseButton + " was clicked!");
    }

    protected override void MouseDragged(MouseButton mouseButton, Vector2 mouseDelta)
    {
        Console.WriteLine(mouseButton + " was Dragged " + mouseDelta + " !");
    }

    protected override void MouseMoved(Vector2 mouseDelta)
    {
        Console.WriteLine("Mouse was moved " + mouseDelta + " !");
    }

    protected override void MousePressed(MouseButton mouseButton)
    {
        Console.WriteLine(mouseButton + " is pressed!");
    }

    protected override void MouseReleased(MouseButton mouseButton)
    {
        Console.WriteLine(mouseButton + " was released!");
    }

    protected override void MouseWheel(Vector2 mouseWheelMove)
    {
        Console.WriteLine("Mouse wheel: " + mouseWheelMove);
    }
}
```