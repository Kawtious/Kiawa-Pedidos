using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

public class Camera2D : Godot.Camera2D
{

    private Position2D TopLeft;

    private Position2D BottomRight;

    private Node Points;

    private int _ActivePoint = 0;

    private int ActivePoint
    {
        get { return _ActivePoint; }
        set { ChangePoint(value); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();

        LimitTop = (int)TopLeft.Position.y;
        LimitLeft = (int)TopLeft.Position.x;
        LimitBottom = (int)BottomRight.Position.y;
        LimitRight = (int)BottomRight.Position.x;
    }

    private void InitNodes()
    {
        TopLeft = GetNode<Position2D>("Limits/Top Left");
        BottomRight = GetNode<Position2D>("Limits/Bottom Right");
        Points = GetNode<Node>("Points");
    }

    public override void _Input(InputEvent evt)
    {
        if (Input.IsActionJustPressed("ui_left"))
        {
            ActivePoint--;
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            ActivePoint++;
        }
    }

    private void ChangePoint(int value)
    {
        Array cameraPoints = GetPoints();

        _ActivePoint = value;

        if (_ActivePoint < 0)
        {
            _ActivePoint = 0;
        }
        else if (_ActivePoint >= cameraPoints.Count)
        {
            _ActivePoint = cameraPoints.Count - 1;
        }

        Position2D point = GetPoints()[_ActivePoint] as Position2D;

        if (point != null)
        {
            GlobalPosition = point.GlobalPosition;
        }
    }

    private Array GetPoints()
    {
        Array points = new Array();

        foreach (Node node in Points.GetChildren())
        {
            if (node is Position2D)
            {
                points.Add(node);
            }
        }

        return points;
    }

}
