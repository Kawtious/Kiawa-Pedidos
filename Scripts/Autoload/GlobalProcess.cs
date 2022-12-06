using Godot;
using System;

public class GlobalProcess : Node
{

    public string Today => System.DateTime.Now.DayOfWeek.ToString();

    private bool VSync = true;

    private int Framerate = 60;

    private bool _Paused = false;

    public bool Paused
    {
        get { return _Paused; }
        set { _Paused = value; GetTree().Paused = _Paused; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();
        InitNodes();

        Paused = GetTree().Paused;
        OS.VsyncEnabled = VSync;
        Engine.TargetFps = Framerate;
    }

    public ResourceDirector ResourceDirector;

    private void InitNodes()
    {
        ResourceDirector = GetNode<ResourceDirector>("/root/ResourceDirector");
    }

    public override void _Input(InputEvent evt)
    {
        if (Input.IsActionJustPressed("change_window_mode"))
        {
            ResourceDirector.Preferences.SwitchFullscreen();
        }

        if (Input.IsActionJustPressed("quit"))
        {
            GetTree().Quit();
        }
    }

}
