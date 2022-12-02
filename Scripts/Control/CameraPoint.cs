using Godot;
using System;
using System.IO;

public class CameraPoint : Position2D
{

    private bool _Ignore = false;

    private bool _Smoothing = true;

    private float _Speed = 5;

    private Vector2 _Zoom = new Vector2(1, 1);

    private string _InputAction = "";

    [Export]
    public bool Ignore
    {
        get { return _Ignore; }
        set { _Ignore = value; }
    }

    [Export]
    public bool Smoothing
    {
        get { return _Smoothing; }
        set { _Smoothing = value; }
    }

    [Export]
    public float Speed
    {
        get { return _Speed; }
        set { _Speed = value; }
    }

    [Export]
    public Vector2 Zoom
    {
        get { return _Zoom; }
        set { _Zoom = value; }
    }

    [Export]
    public string InputAction
    {
        get { return _InputAction; }
        set { _InputAction = value; }
    }

    [Signal]
    delegate void RequestFocus(CameraPoint point);

    public override void _Input(InputEvent evt)
    {
        if (!InputAction.Empty())
        {
            if (Input.IsActionJustPressed(InputAction))
            {
                EmitSignal("RequestFocus", this);
            }
        }
    }
}
