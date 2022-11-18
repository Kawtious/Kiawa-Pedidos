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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
    }

    public override void _Notification(int what)
    {
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        switch (evt)
        {
            case InputEventKey keyEvent:
                HandleInput();
                break;
            default:
                break;
        }
    }

    private void HandleInput()
    {
        if (Input.IsActionJustPressed("change_window_mode"))
        {
            ResourceDirector.Preferences.SwitchFullscreen();
        }

        if (Input.IsActionJustPressed("pause"))
        {
            Paused = !Paused;
            GetTree().Paused = Paused;

            if (GetTree().Paused == true)
            {
                ResourceDirector.MenuAudioStream.Stream = (AudioStream)GlobalProcess.ResourceDirector.GetResource("pause_sound");
            }
            else
            {
                ResourceDirector.MenuAudioStream.Stream = (AudioStream)GlobalProcess.ResourceDirector.GetResource("unpause_sound");
            }

            ResourceDirector.MenuAudioStream.Play();
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
