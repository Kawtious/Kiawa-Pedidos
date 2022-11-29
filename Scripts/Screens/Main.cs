using Godot;

public class Main : Node2D
{

    private AnimationTree AnimationTree;

    private AnimationNodeStateMachinePlayback AnimationState;

    private int _Screen = 1;

    private int Screen
    {
        get { return _Screen; }
        set { ChangeScreen(value); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    private void InitNodes()
    {
        AnimationTree = GetNode<AnimationTree>("Animations/AnimationTree");
        AnimationState = AnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
    }

    public override void _Notification(int what)
    {
    }

    public override void _Input(InputEvent evt)
    {
        if (Input.IsActionJustPressed("ui_left"))
        {
            Screen--;
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            Screen++;
        }
    }

    private void ChangeScreen(int value)
    {
        _Screen = value;

        if (_Screen < 1)
        {
            _Screen = 1;
        }
        else if (_Screen > 3)
        {
            _Screen = 3;
        }

        AnimationState.Travel("camera_move_screen_" + _Screen.ToString());
    }

}
