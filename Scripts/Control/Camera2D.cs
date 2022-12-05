using Godot;
using Godot.Collections;

public class Camera2D : Godot.Camera2D
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    private Position2D LimitTopLeft;

    private Position2D LimitBottomRight;

    private Node Points;

    private CameraPoint CurrentPoint;

    private void InitNodes()
    {
        LimitTopLeft = GetNode<Position2D>("Limits/LimitTopLeft");
        LimitBottomRight = GetNode<Position2D>("Limits/LimitBottomRight");

        LimitTop = (int)LimitTopLeft.Position.y;
        LimitLeft = (int)LimitTopLeft.Position.x;
        LimitBottom = (int)LimitBottomRight.Position.y;
        LimitRight = (int)LimitBottomRight.Position.x;

        Points = GetNode<Node>("Points");
        CurrentPoint = Points.GetNode<CameraPoint>("PointMain");

        ChangePoint(CurrentPoint);
    }

    private void ConnectSignals()
    {
        foreach (CameraPoint point in GetCameraPoints())
        {
            point.Connect("RequestFocus", this, "ChangePoint");
        }
    }

    public override void _Input(InputEvent evt)
    {
        if (Input.IsActionJustPressed("ui_left"))
        {
            ChangePoint(PreviousPoint());
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            ChangePoint(NextPoint());
        }
    }

    private void ChangePoint(CameraPoint point)
    {
        if (point == null)
        {
            return;
        }

        if (GetPointIndex(point) < 0)
        {
            return;
        }

        if (point != null)
        {
            ChangeCameraPoint(point);
        }
    }

    private void ChangeCameraPoint(CameraPoint point)
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

    private CameraPoint NextPoint()
    {
        Array<CameraPoint> cameraPoints = GetCameraPoints();
        CameraPoint currentPoint = CurrentPoint;

        int startingIndex = GetPointIndex(CurrentPoint);

        for (int i = startingIndex + 1; i < cameraPoints.Count; i++)
        {
            currentPoint = cameraPoints[i];
            if (!currentPoint.Ignore)
            {
                return currentPoint;
            }
        }

        for (int i = 0; i < startingIndex; i++)
        {
            currentPoint = cameraPoints[i];
            if (!currentPoint.Ignore)
            {
                return currentPoint;
            }
        }

        return currentPoint;
    }

    private CameraPoint PreviousPoint()
    {
        Array<CameraPoint> cameraPoints = GetCameraPoints();
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

    private int GetPointIndex(CameraPoint point)
    {
        int index = 0;

        foreach (CameraPoint node in GetCameraPoints())
        {
            if (node.Equals(point))
            {
                return index;
            }
            index++;
        }

        return -1;
    }

    private Array<CameraPoint> GetCameraPoints()
    {
        Array<CameraPoint> points = new Array<CameraPoint>();

        foreach (CameraPoint node in Points.GetChildren())
        {
            points.Add(node);
        }

        return points;
    }

}
