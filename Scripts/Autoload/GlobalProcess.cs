using Godot;
using System;

public class GlobalProcess : Node
{

    public static ResourceDirector ResourceDirector;

    private bool Paused = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();

        Paused = GetTree().Paused;
    }

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

    public void ChangeRoom(string room)
    {
        GetTree().ChangeScene("res://Scenes/Rooms/" + room + ".tscn");
    }

}
