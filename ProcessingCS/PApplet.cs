using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace ProcessingCS;

public abstract class PApplet
{

    private DateTime _startTime;
    
    protected int MouseX => GetMouseX();
    protected int MouseY => GetMouseY();
    protected int PMouseX => MouseX-(int)GetMouseDelta().X;
    protected int PMouseY => MouseY-(int)GetMouseDelta().Y;
    
    private Color FillColor { get; set; }
    private Color StrokeColor { get; set; }
    private int StrokeWeightValue { get; set; }
    
    
    HashSet<KeyboardKey> pressedKeys = [];
    HashSet<MouseButton> pressedMouseButtons = [];

    protected void RunFullScreen(string title)
    {
        InitWindow(GetScreenWidth(), GetScreenHeight(), title);
        ToggleFullscreen();
        Run();
    }
    
    protected void Run(int width, int height, string title)
    {
        InitWindow(width, height, title);
        Run();
    }

    private void Run()
    {
        _startTime = DateTime.Now;
        
        Setup();

        while (!WindowShouldClose())
        {

            HandleKeyboardInput();
            HandleMouseInput();
            HandleMouseWheelInput();
            
            BeginDrawing();
            Draw();
            EndDrawing();
        }
        
        CloseWindow();
    }
    


    protected void SetFramerate(int targetFramerate) => SetTargetFPS(targetFramerate);
    protected int GetFramerate() => GetFPS();
    
    protected static Color Color(int rgb) => new(rgb, rgb, rgb, 255);
    protected static Color Color(int rgb, int a) => new(rgb, rgb, rgb, a);
    protected static Color Color(int r, int g, int b) => new (r, g, b, 255);
    protected static Color Color(int r, int g, int b, int a) => new (r, g, b, a);
    
    protected void Fill(Color color) => FillColor = color;
    protected void Fill(int rgb) => FillColor = Color(rgb);
    protected void Fill(int rgb, int a) => FillColor = Color(rgb, a);
    protected void Fill(int r, int g, int b) => FillColor = Color(r, g, b);
    protected void Fill(int r, int g, int b, int a) => FillColor = Color(r, g, b, a);
    protected void NoFill() => FillColor = Color(0, 0);
    
    protected void Stroke(Color color) => StrokeColor = color;
    protected void Stroke(int rgb) => StrokeColor = Color(rgb);
    protected void Stroke(int rgb, int a) => StrokeColor = Color(rgb, a);
    protected void Stroke(int r, int g, int b) => StrokeColor = Color(r, g, b);
    protected void Stroke(int r, int g, int b, int a) => StrokeColor = Color(r, g, b, a);

    protected void StrokeWeight(int strokeWeight) => StrokeWeightValue = strokeWeight;
    protected void NoStroke() => StrokeWeightValue = 0;
    
    protected void Background(Color color) => ClearBackground(color);
    protected void Background(int rgb) => ClearBackground(Color(rgb));
    protected void Background(int r, int g, int b) => ClearBackground(Color(r, g, b));
    
    protected void Circle(int x, int y, float radius)
    {
        if (StrokeWeightValue != 0)
        {
            DrawCircle(x, y, radius, StrokeColor);
        }
        DrawCircle(x, y, radius-StrokeWeightValue*2, FillColor);
    }

    protected void Ellipse(int x, int y, int w, int h)
    {
        if (StrokeWeightValue != 0)
        {
            DrawEllipse(x, y, w, h, StrokeColor);
        }
        DrawEllipse(x, y, w-StrokeWeightValue*2, h-StrokeWeightValue*2, FillColor);
    }

    protected void Rect(int x, int y, int w, int h)
    {
        if (StrokeWeightValue != 0)
        {
            DrawRectangle(x, y, w, h, StrokeColor);
        }
        DrawRectangle(x + StrokeWeightValue, y + StrokeWeightValue, w-StrokeWeightValue*2, h-StrokeWeightValue*2, FillColor);
    }

    protected void Line(int x1, int y1, int x2, int y2)
    {
        if (StrokeWeightValue == 0) return;
        
        if (StrokeWeightValue == 1)
        {
            DrawLine(x1, y1, x2, y1, StrokeColor);
        }
        else
        {
            var start = new Vector2(x1, y1);
            var end = new Vector2(x2, y1);
            DrawLineEx(start, end, StrokeWeightValue, StrokeColor);
        }
    }

    protected void PushMatrix() => Rlgl.PushMatrix();
    protected void PopMatrix() => Rlgl.PopMatrix();
    protected void Translate(int dx, int dy, int dz = 0) => Rlgl.Translatef(dx, dy, dz);

    protected void NoCursor() => DisableCursor();
    protected void Cursor() => EnableCursor();

    #region Input
    
    #region Time & Date

    protected static int Year() => DateTime.Now.Year;
    protected static int Month() => DateTime.Now.Month;
    protected static int Day() => DateTime.Now.Day;
    protected static int Hour() => DateTime.Now.Hour;
    protected static int Minute() => DateTime.Now.Minute;
    protected static int Second() => DateTime.Now.Second;
    protected int Millis() => (int)DateTime.Now.Subtract(_startTime).TotalMilliseconds;

    #endregion Time & Date

    #region Keyboard
    
    protected virtual void KeyPressed(KeyboardKey key){}
    protected virtual void KeyReleased(KeyboardKey key){}
    protected bool IsKeyDown(KeyboardKey key) => pressedKeys.Contains(key);

    private void HandleKeyboardInput()
    {
        while (true)
        {
            var key = (KeyboardKey)GetKeyPressed();
            if(key == KeyboardKey.Null) break;
            pressedKeys.Add(key);
            KeyPressed(key);
        }
        foreach (var key in pressedKeys.Where(key => IsKeyUp(key)))
        {
            KeyReleased(key);
            pressedKeys.Remove(key);
        }
    }
    
    #endregion Keyboard

    #region Mouse

    protected virtual void MouseClicked(MouseButton mouseButton){}
    protected virtual void MouseDragged(MouseButton mouseButton, Vector2 mouseDelta){}
    protected virtual void MouseMoved(Vector2 mouseDelta){}
    protected virtual void MousePressed(MouseButton mouseButton){}
    protected virtual void MouseReleased(MouseButton mouseButton){}
    protected bool IsMouseDown(MouseButton button) => IsMouseButtonDown(button);

    private void HandleMouseInput()
    {
        foreach (var mouseButton in Enum.GetValues<MouseButton>())
        {
            if (IsMouseButtonPressed(mouseButton))
            {
                MouseClicked(mouseButton);
            }
            if (IsMouseButtonDown(mouseButton) && GetMouseDelta().Length() != 0)
            {
                MouseDragged(mouseButton, GetMouseDelta());
            }
                
            if (IsMouseButtonDown(mouseButton))
            {
                MousePressed(mouseButton);
                pressedMouseButtons.Add(mouseButton);
            }
                
            if (IsMouseButtonUp(mouseButton) && pressedMouseButtons.Contains(mouseButton))
            {
                MouseReleased(mouseButton);
                pressedMouseButtons.Remove(mouseButton);
            }
        }
        if (pressedMouseButtons.Count == 0 && GetMouseDelta().Length() != 0)
        {
            MouseMoved(GetMouseDelta());
        }
    }
    
    protected virtual void MouseWheel(Vector2 mouseWheelMove){}
    
    private void HandleMouseWheelInput()
    {
        if (GetMouseWheelMoveV().Length() != 0)
        {
            MouseWheel(GetMouseWheelMoveV());
        }
    }

    #endregion Mouse
    
    #endregion Input

    #region Constants

    protected const double QuarterPi = Math.PI * 0.25d;
    protected const double HalfPi = Math.PI * 0.5d;
    protected const double Pi = Math.PI;
    protected const double TwoPi = Math.PI * 2d;
    protected const double Tau = Math.PI * 2d;

    #endregion
    
    protected abstract void Setup();
    protected abstract void Draw();

}

public enum Mode
{
    Center,
    Radius,
    Corner,
    Corners
}