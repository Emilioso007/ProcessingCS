global using Raylib_cs;
global using System.Numerics;
global using static ProcessingCS.PApplet;
using LibNoise.Primitive;
using static Raylib_cs.Raylib;

namespace ProcessingCS;

public abstract class PApplet
{
    
    private static readonly HashSet<KeyboardKey> _pressedKeys = [];
    private static readonly HashSet<MouseButton> _pressedMouseButtons = [];
    
    public int MouseX => GetMouseX();
    public int MouseY => GetMouseY();
    public int PMouseX => MouseX - (int)GetMouseDelta().X;
    public int PMouseY => MouseY - (int)GetMouseDelta().Y;
    
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

        FrameCount = 0;
        Setup();

        while (!WindowShouldClose())
        {
            FrameCount++;
            HandleKeyboardInput();
            HandleMouseInput();
            HandleMouseWheelInput();

            BeginDrawing();
            Draw();
            EndDrawing();
        }

        CloseWindow();
    }

    protected abstract void Setup();
    protected abstract void Draw();

    #region Input

    #region Time & Date

    public static int Year() => DateTime.Now.Year;
    public static int Month() => DateTime.Now.Month;
    public static int Day() => DateTime.Now.Day;
    public static int Hour() => DateTime.Now.Hour;
    public static int Minute() => DateTime.Now.Minute;
    public static int Second() => DateTime.Now.Second;
    private static DateTime _startTime;
    public static int Millis() => (int)DateTime.Now.Subtract(_startTime).TotalMilliseconds;

    #endregion Time & Date

    #region Keyboard

    protected virtual void KeyPressed(KeyboardKey key) {}
    protected virtual void KeyReleased(KeyboardKey key) {}
    public static bool IsKeyDown(KeyboardKey key) => _pressedKeys.Contains(key);

    private void HandleKeyboardInput()
    {
        while (true)
        {
            var key = (KeyboardKey)GetKeyPressed();
            if (key == KeyboardKey.Null) break;
            _pressedKeys.Add(key);
            KeyPressed(key);
        }
        foreach (var key in _pressedKeys.Where(key => IsKeyUp(key)))
        {
            KeyReleased(key);
            _pressedKeys.Remove(key);
        }
    }

    #endregion Keyboard

    #region Mouse

    protected virtual void MouseClicked(MouseButton mouseButton) {}
    protected virtual void MouseDragged(MouseButton mouseButton, Vector2 mouseDelta) {}
    protected virtual void MouseMoved(Vector2 mouseDelta) {}
    protected virtual void MousePressed(MouseButton mouseButton) {}
    protected virtual void MouseReleased(MouseButton mouseButton) {}
    public static bool IsMouseDown(MouseButton button) => IsMouseButtonDown(button);

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
                _pressedMouseButtons.Add(mouseButton);
            }

            if (IsMouseButtonUp(mouseButton) && _pressedMouseButtons.Contains(mouseButton))
            {
                MouseReleased(mouseButton);
                _pressedMouseButtons.Remove(mouseButton);
            }
        }
        if (_pressedMouseButtons.Count == 0 && GetMouseDelta().Length() != 0)
        {
            MouseMoved(GetMouseDelta());
        }
    }

    protected virtual void MouseWheel(Vector2 mouseWheelMove) {}

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

    public const float QuarterPi = float.Pi * 0.25f;
    public const float HalfPi = float.Pi * 0.5f;
    public const float Pi = float.Pi;
    public const float TwoPi = float.Pi * 2f;
    public const float Tau = float.Pi * 2f;

    #endregion Constants

    #region Shape

    #region 2d Primitives

    public static void Circle(float x, float y, float radius)
    {
        int centerX = (int)x;
        int centerY = (int)y;

        if (StrokeWeightValue != 0)
        {
            DrawCircle(centerX, centerY, radius, StrokeColor);
        }
        DrawCircle(centerX, centerY, radius - StrokeWeightValue * 2, FillColor);
    }
    public static void Ellipse(float x, float y, int w, int h)
    {
        int centerX = (int)x;
        int centerY = (int)y;
        
        if (StrokeWeightValue != 0)
        {
            DrawEllipse(centerX, centerY, w, h, StrokeColor);
        }
        DrawEllipse(centerX, centerY, w - StrokeWeightValue * 2, h - StrokeWeightValue * 2, FillColor);
    }
    public static void Line(float x1, float y1, float x2, float y2)
    {
        if (StrokeWeightValue == 0) return;

        var start = new Vector2(x1, y1);
        var end = new Vector2(x2, y1);
        DrawLineEx(start, end, StrokeWeightValue, StrokeColor);

    }
    public static void Point(float x, float y)
    {
        int posX = (int)x;
        int posY = (int)y;
        
        if (StrokeWeightValue == 0) return;
        DrawPixel(posX, posY, StrokeColor);
    }
    public static void Rect(float x, float y, float w, float h)
    {
        int posX = (int)x;
        int posY = (int)y;
        int width = (int)w;
        int height = (int)h;
        if (StrokeWeightValue != 0)
        {
            DrawRectangle(posX, posY, width, height, StrokeColor);
        }
        DrawRectangle(posX + StrokeWeightValue, posY + StrokeWeightValue, width - StrokeWeightValue * 2, height - StrokeWeightValue * 2, FillColor);
    }
    public static void Square(float x, float y, float extent)
    {
        Rect(x, y, extent, extent);
    }
    public static void Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
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

    public static int Abs(int n) => int.Abs(n);
    public static float Abs(float n) => float.Abs(n);
    public static int Ceil(float n) => (int)MathF.Ceiling(n);
    public static int Constrain(int amt, int low, int high) => int.Clamp(amt, low, high);
    public static float Constrain(float amt, float low, float high) => float.Clamp(amt, low, high);
    public static float Dist(float x1, float y1, float x2, float y2) => MathF.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
    public static float Exp(float n) => MathF.Exp(n);
    public static int Floor(float n) => (int)MathF.Floor(n);
    public static float Lerp(float start, float stop, float amt) => float.Lerp(start, stop, amt);
    public static float Log(float n) => MathF.Log(n);
    public static float Log10(float n) => MathF.Log10(n);
    public static float Mag(float a, float b) => MathF.Sqrt(a * a + b * b);
    public static float Map(float value, float start1, float stop1, float start2, float stop2) => (value - start1) * (stop2 - start2) / (start1 - stop1) + start2;
    public static float Max(float n, params float[] ns) => ns.Aggregate(n, float.Max);
    public static float Min(float n, params float[] ns) => ns.Aggregate(n, float.Min);
    public static float Norm(float value, float start, float stop) => (value - start) / (stop - start);
    public static float Pow(float n, float e) => MathF.Pow(n, e);
    public static int Round(float n) => (int)n + n % 1 >= 0.5f ? 1 : 0;
    public static float Sq(float n) => n * n;
    public static float Sqrt(float n) => MathF.Sqrt(n);

    #endregion Calculation

    #region Trigonometry

    public static float Acos(float value) => MathF.Acos(value);
    public static float Asin(float value) => MathF.Asin(value);
    public static float Atan2(float x, float y) => MathF.Atan2(x, y);
    public static float Atan(float value) => MathF.Atan(value);
    public static float Cos(float value) => MathF.Cos(value);
    public static float Degrees(float radians) => radians * RAD2DEG;
    public static float Radians(float degrees) => degrees * DEG2RAD;
    public static float Sin(float value) => MathF.Sin(value);
    public static float Tan(float value) => MathF.Tan(value);

    #endregion Trigonometry

    #region Random

    private static Random _random = new();
    public static float Random() => _random.NextSingle();
    public static float Random(float high) => _random.NextSingle() * high;
    public static float Random(float low, float high) => low + _random.NextSingle() * (high - low);
    public static void RandomSeed(int seed) => _random = new Random(seed);

    private static float _nextNextGaussian;
    private static bool _haveNextNextGaussian;
    public static float RandomGaussian()
    {
        if (_haveNextNextGaussian)
        {
            _haveNextNextGaussian = false;
            return _nextNextGaussian;
        }
        float v1, v2, s;
        do
        {
            v1 = 2 * System.Random.Shared.NextSingle() - 1; // between -1.0 and 1.0
            v2 = 2 * System.Random.Shared.NextSingle() - 1; // between -1.0 and 1.0
            s = v1 * v1 + v2 * v2;
        } while (s is >= 1 or 0);
        float multiplier = MathF.Sqrt(-2 * MathF.Log(s) / s);
        _nextNextGaussian = v2 * multiplier;
        _haveNextNextGaussian = true;
        return v1 * multiplier;
    }

    private static readonly SimplexPerlin _simplexPerlin = new();
    public static float Noise(float x, float y) => _simplexPerlin.GetValue(x, y);
    public static void NoiseSeed(int seed) => _simplexPerlin.Seed = seed;

    #endregion Random

    #endregion Math

    #region Color

    #region Creating & Reading

    public static byte Red(Color color) => color.R;
    public static byte Green(Color color) => color.G;
    public static byte Blue(Color color) => color.B;
    public static byte Alpha(Color color) => color.A;

    public static Color Color(int rgb) => new(rgb, rgb, rgb, 255);
    public static Color Color(int rgb, int a) => new(rgb, rgb, rgb, a);
    public static Color Color(int r, int g, int b) => new(r, g, b, 255);
    public static Color Color(int r, int g, int b, int a) => new(r, g, b, a);

    public static Color LerpColor(Color c1, Color c2, float amt)
    {
        amt = float.Clamp(amt, 0, 1);
        return new Color(
        float.Lerp(c1.R, c2.R, amt) / 255f,
        float.Lerp(c1.G, c2.G, amt) / 255f,
        float.Lerp(c1.B, c2.B, amt) / 255f);
    }

    #endregion Creating & Reading

    #region Setting
    
    public static void Background(Color color) => ClearBackground(color);
    public static void Background(int rgb) => ClearBackground(Color(rgb));
    public static void Background(int r, int g, int b) => ClearBackground(Color(r, g, b));

    private static Color FillColor { get; set; }
    public static void Fill(Color color) => FillColor = color;
    public static void Fill(int rgb) => FillColor = Color(rgb);
    public static void Fill(int rgb, int a) => FillColor = Color(rgb, a);
    public static void Fill(int r, int g, int b) => FillColor = Color(r, g, b);
    public static void Fill(int r, int g, int b, int a) => FillColor = Color(r, g, b, a);
    public static void NoFill() => FillColor = Color(0, 0);

    private static Color StrokeColor { get; set; }
    public static void Stroke(Color color) => StrokeColor = color;
    public static void Stroke(int rgb) => StrokeColor = Color(rgb);
    public static void Stroke(int rgb, int a) => StrokeColor = Color(rgb, a);
    public static void Stroke(int r, int g, int b) => StrokeColor = Color(r, g, b);
    public static void Stroke(int r, int g, int b, int a) => StrokeColor = Color(r, g, b, a);

    private static int StrokeWeightValue { get; set; }
    public static void StrokeWeight(int strokeWeight) => StrokeWeightValue = strokeWeight;
    public static void NoStroke() => StrokeWeightValue = 0;

    #endregion Setting

    #endregion Color

    #region Transform

    public static void PushMatrix() => Rlgl.PushMatrix();
    public static void PopMatrix() => Rlgl.PopMatrix();
    public static void Translate(int dx, int dy, int dz = 0) => Rlgl.Translatef(dx, dy, dz);
    public static void Rotate(float radians) => Rlgl.Rotatef(radians * RAD2DEG, 0, 0, 1);
    public static void Scale(float s) => Rlgl.Scalef(s, s, 1);
    public static void Scale(float x, float y) => Rlgl.Scalef(x, y, 1);

    #endregion Transform

    #region Environment

    public static void Cursor() => EnableCursor();
    public static void NoCursor() => DisableCursor();
    public static void Delay(int napTime) => Thread.Sleep(napTime);
    public static int FrameCount { get; private set; }
    public static void SetFramerate(int targetFramerate) => SetTargetFPS(targetFramerate);
    public static int Framerate => GetFPS();
    public static int Width => GetScreenWidth();
    public static int Height => GetScreenHeight();

    #endregion Environment
}
