using Godot;
using System;

public class Camera2D : Godot.Camera2D {

    private Position2D _TopLeft;

    private Position2D _BottomRight;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        InitNodes();

        LimitTop = (int)_TopLeft.Position.y;
        LimitLeft = (int)_TopLeft.Position.x;
        LimitBottom = (int)_BottomRight.Position.y;
        LimitRight = (int)_BottomRight.Position.x;
    }

    private void InitNodes() {
        _TopLeft = GetNode<Position2D>("Limits/Top Left");
        _BottomRight = GetNode<Position2D>("Limits/Bottom Right");
    }

}
