using Godot;
using System;

public class Preferences : Node
{

    private bool _Borderless = false;

    private bool _Fullscreen = false;

    private Vector2 _WindowSize = new Vector2(1280, 720);

    [Export]
    public bool Borderless
    {
        get { return _Borderless; }
        set { _Borderless = value; SetBorderless(_Borderless); }
    }

    [Export]
    public bool Fullscreen
    {
        get { return _Fullscreen; }
        set { _Fullscreen = value; SetFullscreen(_Fullscreen); }
    }

    [Export]
    public Vector2 WindowSize
    {
        get { return _WindowSize; }
        set { _WindowSize = value; SetWindowSize(_WindowSize); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        OS.WindowBorderless = Borderless;
        OS.WindowFullscreen = Fullscreen;
        OS.WindowSize = WindowSize;
    }

    private void SetBorderless(bool value)
    {
        OS.WindowBorderless = value;
        OS.CenterWindow();
    }

    private void SetFullscreen(bool value)
    {
        OS.WindowFullscreen = value;
    }

    private void SetWindowSize(Vector2 value)
    {
        OS.WindowSize = value;
        OS.CenterWindow();
    }

    public void SwitchBorderless()
    {
        Borderless = !Borderless;
    }

    public void SwitchFullscreen()
    {
        Fullscreen = !Fullscreen;
    }

}
