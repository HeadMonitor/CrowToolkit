using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrowEngineUI.MonoGame.Avalonia.Controls;

public class MonoGameControl : Control
{
    /// <summary>
    ///     Initialzies a new instance of the <see cref="MonoGameControl" /> class.
    /// </summary>
    public MonoGameControl()
    {
        Focusable = true;
        _timer = new Stopwatch();
        _gameTime = new GameTime();
        _buffer = Array.Empty<byte>();
        _keysDownHash = new HashSet<Keys>();

        _presentationParameters = new PresentationParameters
        {
            BackBufferWidth = 1,
            BackBufferHeight = 1,
            BackBufferFormat = SurfaceFormat.Color,
            DepthStencilFormat = DepthFormat.Depth24,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
        };
    }

    /// <summary>
    ///     Renders the visual to a <see cref="DrawingContext" />.
    /// </summary>
    /// <param name="context">
    ///     The drawing context.
    /// </param>
    public override void Render(DrawingContext context)
    {
        //  Ensure we have everythign needed to render a frame of the game to the control
        //  otherwise render a blank control with the default background color.
        if (IsPaused ||
            Game is not Game game ||
            Game.GraphicsDevice is not GraphicsDevice device ||
            _writableBitmap is null ||
            Bounds.Width < 1 ||
            Bounds.Height < 1 ||
            !HandleDeviceReset(device))
        {
            context.DrawRectangle(DefaultBackground, null, new Rect(Bounds.Size));
            return;
        }

        //  Run a single frame of the game, where the graphics are written to the back buffer, then extract the
        //  back bufer data and draw it to the control.
        RunSingleFrame(game);
        ExtractFrame(device, _writableBitmap);
        context.DrawImage(_writableBitmap, new Rect(_writableBitmap.Size), Bounds);
    }

    /// <summary>
    ///     Positions child elements as part of a layout pass.
    /// </summary>
    /// <param name="finalSize">
    ///     The size available to the control.
    /// </param>
    /// <returns>
    ///     The actual size used.
    /// </returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        finalSize = base.ArrangeOverride(finalSize);

        //  If the size is not the same as the writable bitmap, we need to reset the device
        if (finalSize != _writableBitmap?.Size && Game?.GraphicsDevice is GraphicsDevice device)
            ResetDevice(device, finalSize);

        return finalSize;
    }

    /// <summary>
    ///     Called when the control is added to a rooted visual tree.
    /// </summary>
    /// <param name="e">
    ///     The event args.
    /// </param>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Start();
    }

    private bool HandleDeviceReset(GraphicsDevice device)
    {
        if (device.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset) ResetDevice(device, Bounds.Size);

        return device.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal;
    }

    private void HandlePresentationParameterChange(Game? game)
    {
        if (game is null) return;
        ResetDevice(game.GraphicsDevice,
            new Size(_presentationParameters.BackBufferWidth, _presentationParameters.BackBufferHeight));
    }

    private void Start()
    {
        if (_hasBeenInitialized) return;

        Initialize();
        _timer.Start();
        _hasBeenInitialized = true;
    }

    private void Initialize()
    {
        //  Get the window handle that the Avalonia project is running in and set that as the window handle of
        //  the game
        if (this.GetVisualRoot() is Window window && window.TryGetPlatformHandle()?.Handle is IntPtr handle)
        {
            _presentationParameters.DeviceWindowHandle = handle;
            Mouse.WindowHandle = handle;
        }

        if (Game is not Game game) return;

        var keyboardType = typeof(Keyboard);
        var setActiveMethodInfo = keyboardType.GetMethod("SetActive", BindingFlags.NonPublic | BindingFlags.Static);
        setActiveMethodInfo?.Invoke(null, new object[] { true });


        if (game.GraphicsDevice is GraphicsDevice device) ResetDevice(device, Bounds.Size);

        RunSingleFrame(game);
    }

    private void ResetDevice(GraphicsDevice device, Size size)
    {
        //  Ensure a minimum width and height of 1
        var width = Math.Max(1, (int)Math.Ceiling(size.Width));
        var height = Math.Max(1, (int)Math.Ceiling(size.Height));

        device.Viewport = new Viewport(0, 0, width, height);
        _presentationParameters.BackBufferWidth = width;
        _presentationParameters.BackBufferHeight = height;
        device.Reset(_presentationParameters);

        //  Recreate the writable bitmap
        InitializeWritableBitmap(device);
    }

    [MemberNotNull(nameof(_writableBitmap))]
    private void InitializeWritableBitmap(GraphicsDevice device)
    {
        _writableBitmap?.Dispose();
        _writableBitmap = new WriteableBitmap(
            new PixelSize(device.Viewport.Width, device.Viewport.Height),
            new Vector(96d, 96d),
            PixelFormat.Rgba8888,
            AlphaFormat.Opaque);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        _keysDownHash.Add(ConvertKeytoKey(e.Key));
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        _keysDownHash.Remove(ConvertKeytoKey(e.Key));
        base.OnKeyUp(e);
    }

    private Keys ConvertKeytoKey(Key key)
    {
        return key switch
        {
            Key.None => Keys.None,
            Key.Back => Keys.Back,
            Key.Tab => Keys.Tab,
            Key.Enter => Keys.Enter,
            Key.CapsLock => Keys.CapsLock,
            Key.Escape => Keys.Escape,
            Key.Space => Keys.Space,
            Key.PageUp => Keys.PageUp,
            Key.PageDown => Keys.PageDown,
            Key.End => Keys.End,
            Key.Home => Keys.Home,
            Key.Left => Keys.Left,
            Key.Up => Keys.Up,
            Key.Right => Keys.Right,
            Key.Down => Keys.Down,
            Key.Select => Keys.Select,
            Key.Print => Keys.Print,
            Key.Execute => Keys.Execute,
            Key.PrintScreen => Keys.PrintScreen,
            Key.Insert => Keys.Insert,
            Key.Delete => Keys.Delete,
            Key.Help => Keys.Help,
            Key.D0 => Keys.D0,
            Key.D1 => Keys.D1,
            Key.D2 => Keys.D2,
            Key.D3 => Keys.D3,
            Key.D4 => Keys.D4,
            Key.D5 => Keys.D5,
            Key.D6 => Keys.D6,
            Key.D7 => Keys.D7,
            Key.D8 => Keys.D8,
            Key.D9 => Keys.D9,
            Key.A => Keys.A,
            Key.B => Keys.B,
            Key.C => Keys.C,
            Key.D => Keys.D,
            Key.E => Keys.E,
            Key.F => Keys.F,
            Key.G => Keys.G,
            Key.H => Keys.H,
            Key.I => Keys.I,
            Key.J => Keys.J,
            Key.K => Keys.K,
            Key.L => Keys.L,
            Key.M => Keys.M,
            Key.N => Keys.N,
            Key.O => Keys.O,
            Key.P => Keys.P,
            Key.Q => Keys.Q,
            Key.R => Keys.R,
            Key.S => Keys.S,
            Key.T => Keys.T,
            Key.U => Keys.U,
            Key.V => Keys.V,
            Key.W => Keys.W,
            Key.X => Keys.X,
            Key.Y => Keys.Y,
            Key.Z => Keys.Z,
            Key.LWin => Keys.LeftWindows,
            Key.RWin => Keys.RightWindows,
            Key.Apps => Keys.Apps,
            Key.Sleep => Keys.Sleep,
            Key.NumPad0 => Keys.NumPad0,
            Key.NumPad1 => Keys.NumPad1,
            Key.NumPad2 => Keys.NumPad2,
            Key.NumPad3 => Keys.NumPad3,
            Key.NumPad4 => Keys.NumPad4,
            Key.NumPad5 => Keys.NumPad5,
            Key.NumPad6 => Keys.NumPad6,
            Key.NumPad7 => Keys.NumPad7,
            Key.NumPad8 => Keys.NumPad8,
            Key.NumPad9 => Keys.NumPad9,
            Key.Multiply => Keys.Multiply,
            Key.Add => Keys.Add,
            Key.Separator => Keys.Separator,
            Key.Subtract => Keys.Subtract,
            Key.Decimal => Keys.Decimal,
            Key.Divide => Keys.Divide,
            Key.F1 => Keys.F1,
            Key.F2 => Keys.F2,
            Key.F3 => Keys.F3,
            Key.F4 => Keys.F4,
            Key.F5 => Keys.F5,
            Key.F6 => Keys.F6,
            Key.F7 => Keys.F7,
            Key.F8 => Keys.F8,
            Key.F9 => Keys.F9,
            Key.F10 => Keys.F10,
            Key.F11 => Keys.F11,
            Key.F12 => Keys.F12,
            Key.F13 => Keys.F13,
            Key.F14 => Keys.F14,
            Key.F15 => Keys.F15,
            Key.F16 => Keys.F16,
            Key.F17 => Keys.F17,
            Key.F18 => Keys.F18,
            Key.F19 => Keys.F19,
            Key.F20 => Keys.F20,
            Key.F21 => Keys.F21,
            Key.F22 => Keys.F22,
            Key.F23 => Keys.F23,
            Key.F24 => Keys.F24,
            Key.NumLock => Keys.NumLock,
            Key.Scroll => Keys.Scroll,
            Key.LeftShift => Keys.LeftShift,
            Key.RightShift => Keys.RightShift,
            Key.LeftCtrl => Keys.LeftControl,
            Key.RightCtrl => Keys.RightControl,
            Key.LeftAlt => Keys.LeftAlt,
            Key.RightAlt => Keys.RightAlt,
            Key.BrowserBack => Keys.BrowserBack,
            Key.BrowserForward => Keys.BrowserForward,
            Key.BrowserRefresh => Keys.BrowserRefresh,
            Key.BrowserStop => Keys.BrowserStop,
            Key.BrowserSearch => Keys.BrowserSearch,
            Key.BrowserFavorites => Keys.BrowserFavorites,
            Key.BrowserHome => Keys.BrowserHome,
            Key.VolumeMute => Keys.VolumeMute,
            Key.VolumeDown => Keys.VolumeDown,
            Key.VolumeUp => Keys.VolumeUp,
            Key.MediaNextTrack => Keys.MediaNextTrack,
            Key.MediaPreviousTrack => Keys.MediaPreviousTrack,
            Key.MediaStop => Keys.MediaStop,
            Key.MediaPlayPause => Keys.MediaPlayPause,
            Key.LaunchMail => Keys.LaunchMail,
            Key.SelectMedia => Keys.SelectMedia,
            Key.LaunchApplication1 => Keys.LaunchApplication1,
            Key.LaunchApplication2 => Keys.LaunchApplication2,
            Key.OemSemicolon => Keys.OemSemicolon,
            Key.OemPlus => Keys.OemPlus,
            Key.OemComma => Keys.OemComma,
            Key.OemMinus => Keys.OemMinus,
            Key.OemPeriod => Keys.OemPeriod,
            Key.OemQuestion => Keys.OemQuestion,
            Key.OemTilde => Keys.OemTilde,
            Key.OemOpenBrackets => Keys.OemOpenBrackets,
            Key.OemPipe => Keys.OemPipe,
            Key.OemCloseBrackets => Keys.OemCloseBrackets,
            Key.OemQuotes => Keys.OemQuotes,
            Key.Oem8 => Keys.Oem8,
            Key.OemBackslash => Keys.OemBackslash,
            //Key.ProcessKey = Microsoft.Xna.Framework.Input.Keys.ProcessKey,
            Key.Attn => Keys.Attn,
            Key.CrSel => Keys.Crsel,
            Key.ExSel => Keys.Exsel,
            Key.EraseEof => Keys.EraseEof,
            Key.Play => Keys.Play,
            Key.Zoom => Keys.Zoom,
            Key.Pa1 => Keys.Pa1,
            // Key.ChatPadGreen => Microsoft.Xna.Framework.Input.Keys.ChatPadGreen ,
            // Key.ChatPadOrange => Microsoft.Xna.Framework.Input.Keys.ChatPadOrange ,
            Key.OemClear => Keys.OemClear,
            Key.Pause => Keys.Pause,
            Key.ImeConvert => Keys.ImeConvert,
            Key.ImeNonConvert => Keys.ImeNoConvert,
            Key.KanaMode => Keys.Kana,
            Key.KanjiMode => Keys.Kanji,
            Key.OemAuto => Keys.OemAuto,
            Key.OemCopy => Keys.OemCopy,
            Key.OemEnlw => Keys.OemEnlW,
            _ => Keys.None
        };
    }

    private void RunSingleFrame(Game game)
    {
        _gameTime.ElapsedGameTime = _timer.Elapsed;
        _gameTime.TotalGameTime += _gameTime.ElapsedGameTime;
        _timer.Restart();

        try
        {
            var keyboardType = typeof(Keyboard);
            var setKeysMethod = keyboardType.GetMethod("SetKeys", BindingFlags.NonPublic | BindingFlags.Static);
            setKeysMethod?.Invoke(null, new object[] { _keysDownHash.ToList() });

            game.RunOneFrame();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Normal);
        }
    }

    private void ExtractFrame(GraphicsDevice device, WriteableBitmap? writeAbleBitmap)
    {
        if (_writableBitmap is null) return;

        using (var lockedFrameBuffer = writeAbleBitmap.Lock())
        {
            //  Determine the length of the buffer
            var size = lockedFrameBuffer.RowBytes * lockedFrameBuffer.Size.Height;

            //  Resize internal buffer if needed
            if (_buffer.Length < size) Array.Resize(ref _buffer, size);

            //  Pull the data from the graphics device back buffer
            device.GetBackBufferData(_buffer, 0, size);

            //  Copy the backbuffer data into the writable bitmap
            Marshal.Copy(_buffer, 0, lockedFrameBuffer.Address, size);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// Avalonia Direct Properties
    ////////////////////////////////////////////////////////////////////////////////

    #region Avalonia Direct Properties

    /// <summary>
    ///     Property for Avalonia that is used to change the default background color used when the control is rendered.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, IBrush> DefaultBackgroundProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, IBrush>(nameof(DefaultBackground), g => g.DefaultBackground,
            (s, v) => s.DefaultBackground = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Game" /> instance being played
    ///     by the control
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, Game?> GameProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, Game?>(nameof(Game), g => g.Game, (s, v) => s.Game = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the
    ///     <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.BackBufferWidth" />
    ///     value used.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, int> BackBufferWidthProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, int>(nameof(BackBufferWidth), g => g.BackBufferWidth,
            (s, v) => s.BackBufferWidth = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the
    ///     <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.BackBufferHeight" />
    ///     value used.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, int> BackBufferHeightProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, int>(nameof(BackBufferHeight), g => g.BackBufferHeight,
            (s, v) => s.BackBufferHeight = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the
    ///     <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.BackBufferFormat" />
    ///     value used.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, SurfaceFormat> BackBufferFormatProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, SurfaceFormat>(nameof(BackBufferFormat),
            g => g.BackBufferFormat, (s, v) => s.BackBufferFormat = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the
    ///     <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.DepthStencilFormat" />
    ///     value used.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, DepthFormat> DepthStencilFormatProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, DepthFormat>(nameof(DepthStencilFormat),
            g => g.DepthStencilFormat, (s, v) => s.DepthStencilFormat = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the
    ///     <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.PresentationInterval" />
    ///     value used.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, PresentInterval> PresentationIntervalProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, PresentInterval>(nameof(PresentationInterval),
            g => g.PresentationInterval, (s, v) => s.PresentationInterval = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the
    ///     <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.IsFullScreen" />
    ///     value used.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, bool> IsFullScreenProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, bool>(nameof(IsFullScreen), g => g.IsFullScreen,
            (s, v) => s.IsFullScreen = v);

    /// <summary>
    ///     Property for Avalonia that is used to define the
    ///     <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.IsFullScreen" />
    ///     value used.
    /// </summary>
    public static readonly DirectProperty<MonoGameControl, bool> PauseProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, bool>(nameof(IsPaused), g => g.IsPaused,
            (s, v) => s.IsPaused = v);

    #endregion

    ////////////////////////////////////////////////////////////////////////////////
    /// Fields
    ////////////////////////////////////////////////////////////////////////////////

    #region Fields

    private readonly Stopwatch _timer;

    private readonly GameTime _gameTime;

    private Game? _game;
    private byte[] _buffer;
    private WriteableBitmap? _writableBitmap;
    private bool _hasBeenInitialized;
    private PresentationParameters _presentationParameters;
    private bool _isPaused;
    private readonly HashSet<Keys> _keysDownHash;

    #endregion Fields

    ////////////////////////////////////////////////////////////////////////////////
    /// Properties
    ////////////////////////////////////////////////////////////////////////////////

    #region Properties

    /// <summary>
    ///     Gets or Sets the default background color to use when rendering the control in an Avalonia view
    /// </summary>
    public IBrush DefaultBackground { get; set; } = Brushes.CornflowerBlue;

    /// <summary>
    ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Game" /> instance being played by the control.
    /// </summary>
    public Game? Game
    {
        get => _game;
        set
        {
            if (_game == value) return;

            _game = value;

            if (_hasBeenInitialized) Initialize();
        }
    }

    /// <summary>
    ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters" /> used by the
    ///     graphics device of the game.
    /// </summary>
    public PresentationParameters PresentationParameters
    {
        get => _presentationParameters;
        set
        {
            if (_presentationParameters == value) return;
            _presentationParameters = value;
            HandlePresentationParameterChange(_game);
        }
    }

    /// <summary>
    ///     Gets or Sets the width, in pixels, of the back buffer used by the graphics device of the game.
    /// </summary>
    public int BackBufferWidth
    {
        get => _presentationParameters.BackBufferWidth;
        set
        {
            if (_presentationParameters.BackBufferWidth == value) return;
            _presentationParameters.BackBufferWidth = value;
            HandlePresentationParameterChange(_game);
        }
    }

    /// <summary>
    ///     Gets or Sets the height, in pixels, of hte back buffer used by the graphics device of the game.
    /// </summary>
    public int BackBufferHeight
    {
        get => _presentationParameters.BackBufferHeight;
        set
        {
            if (_presentationParameters.BackBufferHeight == value) return;
            _presentationParameters.BackBufferHeight = value;
            HandlePresentationParameterChange(_game);
        }
    }

    /// <summary>
    ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.SurfaceFormat" /> of the back buffer used
    ///     by the graphics device of the game.
    /// </summary>
    public SurfaceFormat BackBufferFormat
    {
        get => _presentationParameters.BackBufferFormat;
        set
        {
            if (_presentationParameters.BackBufferFormat == value) return;
            _presentationParameters.BackBufferFormat = value;
            HandlePresentationParameterChange(_game);
        }
    }

    /// <summary>
    ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.DepthFormat" /> used by the graphics device
    ///     of the game
    /// </summary>
    public DepthFormat DepthStencilFormat
    {
        get => _presentationParameters.DepthStencilFormat;
        set
        {
            if (_presentationParameters.DepthStencilFormat == value) return;
            _presentationParameters.DepthStencilFormat = value;
            HandlePresentationParameterChange(_game);
        }
    }

    /// <summary>
    ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.PresentInterval" /> used by the graphics
    ///     device of the game.
    /// </summary>
    public PresentInterval PresentationInterval
    {
        get => _presentationParameters.PresentationInterval;
        set
        {
            if (_presentationParameters.PresentationInterval == value) return;
            _presentationParameters.PresentationInterval = value;
            HandlePresentationParameterChange(_game);
        }
    }

    /// <summary>
    ///     Gets or Sets a value that indicates if the graphics of the game should be rendered in full screen.
    /// </summary>
    public bool IsFullScreen
    {
        get => _presentationParameters.IsFullScreen;
        set
        {
            if (_presentationParameters.IsFullScreen == value) return;
            _presentationParameters.IsFullScreen = value;
            HandlePresentationParameterChange(_game);
        }
    }

    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (_isPaused == value) return;
            _isPaused = value;
        }
    }

    #endregion Properties
}