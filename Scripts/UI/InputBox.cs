using Godot;
using System;

public class InputBox : TextEdit
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
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
    }
}
