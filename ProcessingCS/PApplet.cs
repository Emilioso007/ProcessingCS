using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace ProcessingCS;

public abstract class PApplet
{

    private DateTime _startTime;
    
    protected int MouseX => GetMouseX();
    protected int MouseY => GetMouseY();
    protected int PMouseX => MouseX - (int)GetMouseDelta().X;
    protected int PMouseY => MouseY - (int)GetMouseDelta().Y;
    
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

    #endregion Constants

    #region Shape

    #region 2d Primitives

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
    protected void Line(int x1, int y1, int x2, int y2)
    {
        if (StrokeWeightValue == 0) return;
        
        var start = new Vector2(x1, y1);
        var end = new Vector2(x2, y1);
        DrawLineEx(start, end, StrokeWeightValue, StrokeColor);
        
    }
    protected void Point(int x, int y)
    {
        if (StrokeWeightValue == 0) return;
        DrawPixel(x, y, StrokeColor);
    }
    protected void Rect(int x, int y, int w, int h)
    {
        if (StrokeWeightValue != 0)
        {
            DrawRectangle(x, y, w, h, StrokeColor);
        }
        DrawRectangle(x + StrokeWeightValue, y + StrokeWeightValue, w-StrokeWeightValue*2, h-StrokeWeightValue*2, FillColor);
    }
    protected void Square(int x, int y, int extent)
    {
        Rect(x, y, extent, extent);
    }
    protected void Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        var v1 = new Vector2(x1, y1);
        var v2 = new Vector2(x2, y2);
        var v3 = new Vector2(x3, y3);
        DrawTriangle(v1, v2, v3, FillColor);
    }
    
    #endregion 2d Primitives

    #endregion Shape

    #region Math

    #region Calculation

    protected static int Abs(int n) => int.Abs(n);
    protected static float Abs(float n) => float.Abs(n);
    protected static int Ceil(float n) => (int)MathF.Ceiling(n);
    protected static int Constrain(int amt, int low, int high) => int.Clamp(amt, low, high);
    protected static float Constrain(float amt, float low, float high) => float.Clamp(amt, low, high);
    protected static float Dist(float x1, float y1, float x2, float y2) => MathF.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
    protected static float Exp(float n) => MathF.Exp(n);
    protected static int Floor(float n) => (int)MathF.Floor(n);
    protected static float Lerp(float start, float stop, float amt) => start * (1 - amt) + stop * amt;
    protected static float Log(float n) => MathF.Log(n);
    protected static float Log10(float n) => MathF.Log10(n);
    protected static float Mag(float a, float b) => MathF.Sqrt((a * a) + (b * b));
    protected static float Map(float value, float start1, float stop1, float start2, float stop2) => (value - start1) * (stop2-start2)/(start1-stop1)+start2;
    protected static float Max(float n, params float[] ns) => ns.Aggregate(n, float.Max);
    protected static float Min(float n, params float[] ns) => ns.Aggregate(n, float.Min);
    protected static float Norm(float value, float start, float stop) => (value - start) / (stop - start);
    protected static float Pow(float n, float e) => MathF.Pow(n, e);
    protected static int Round(float n) => (int)n + n % 1 >= 0.5f ? 1 : 0;
    protected static float Sq(float n) => n * n;
    protected static float Sqrt(float n) => MathF.Sqrt(n);
    
    #endregion Calculation

    #region Trigonometry

    protected static float Acos(float value) => MathF.Acos(value);
    protected static float Asin(float value) => MathF.Asin(value);
    protected static float Atan2(float x, float y) => MathF.Atan2(x, y);
    protected static float Atan(float value) => MathF.Atan(value);
    protected static float Cos(float value) => MathF.Cos(value);
    protected static float Degrees(float radians) => radians * RAD2DEG;
    protected static float Radians(float degrees) => degrees * DEG2RAD;
    protected static float Sin(float value) => MathF.Sin(value);
    protected static float Tan(float value) => MathF.Tan(value);

    #endregion Trigonometry

    #endregion Math
    
    protected abstract void Setup();
    protected abstract void Draw();

}