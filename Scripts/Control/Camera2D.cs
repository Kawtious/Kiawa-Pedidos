using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;
using Godot.Collections;

public class Camera2D : Godot.Camera2D
{

    private Position2D LimitTopLeft;

    private Position2D LimitBottomRight;

    private Node Points;

    private CameraPoint CurrentPoint;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();

        LimitTop = (int)LimitTopLeft.Position.y;
        LimitLeft = (int)LimitTopLeft.Position.x;
        LimitBottom = (int)LimitBottomRight.Position.y;
        LimitRight = (int)LimitBottomRight.Position.x;

        ChangePoint(CurrentPoint);
    }

    private void InitNodes()
    {
        LimitTopLeft = GetNode<Position2D>("Limits/LimitTopLeft");
        LimitBottomRight = GetNode<Position2D>("Limits/LimitBottomRight");
        Points = GetNode<Node>("Points");
        CurrentPoint = Points.GetNode<CameraPoint>("PointMain");
    }

    private void ConnectSignals()
    {
        foreach (CameraPoint point in GetPoints())
        {
            point.Connect("RequestFocus", this, "ChangePoint");
        }
    }

    public override void _Input(InputEvent evt)
    {
        if (Input.IsActionJustPressed("ui_left"))
        {
            ChangePoint(FindPrevAvailablePoint());
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            ChangePoint(FindNextAvailablePoint());
        }
    }

    private void ChangePoint(CameraPoint point)
    {
        if (point == null)
        {
            return;
        }

        int pointIndex = GetPointIndex(point);

        if (pointIndex < 0)
        {
            return;
        }

        if (point != null)
        {
            if (CurrentPoint.Smoothing)
            {
                SmoothingEnabled = point.Smoothing;
            }

            SmoothingSpeed = point.Speed;
            Zoom = point.Zoom;

            GlobalPosition = point.GlobalPosition;
            CurrentPoint = point;
        }
    }

    private CameraPoint FindNextAvailablePoint()
    {
        Array<CameraPoint> cameraPoints = GetPoints();
        CameraPoint point = CurrentPoint;

        int startingIndex = GetPointIndex(CurrentPoint);

        for (int i = startingIndex + 1; i < cameraPoints.Count; i++)
        {
            point = cameraPoints[i];
            if (!point.Ignore)
            {
                return point;
            }
        }

        for (int i = 0; i < startingIndex; i++)
        {
            point = cameraPoints[i];
            if (!point.Ignore)
            {
                return point;
            }
        }

        return point;
    }

    private CameraPoint FindPrevAvailablePoint()
    {
        Array<CameraPoint> cameraPoints = GetPoints();
        CameraPoint point = CurrentPoint;

        int startingIndex = GetPointIndex(CurrentPoint);

        for (int i = startingIndex - 1; i >= 0; i--)
        {
            point = cameraPoints[i];
            if (!point.Ignore)
            {
                return point;
            }
        }

        for (int i = cameraPoints.Count - 1; i > startingIndex; i--)
        {
            point = cameraPoints[i];
            if (!point.Ignore)
            {
                return point;
            }
        }

        return point;
    }

    private Array<CameraPoint> GetPoints()
    {
        Array<CameraPoint> points = new Array<CameraPoint>();

        foreach (CameraPoint node in Points.GetChildren())
        {
            points.Add(node);
        }

        return points;
    }

    private int GetPointIndex(CameraPoint point)
    {
        Array<CameraPoint> points = new Array<CameraPoint>();

        int index = 0;

        foreach (CameraPoint node in Points.GetChildren())
        {
            if (node.Equals(point))
            {
                return index;
            }
            index++;
        }

        return -1;
    }

}
