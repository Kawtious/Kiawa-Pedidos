using Godot;

public class Notifications : CanvasLayer
{

    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    private AnimationTree AnimationTree;

    private AnimationNodeStateMachinePlayback AnimationState;

    private AnimationTree TreeNewOrder;

    private AnimationNodeStateMachinePlayback StateNewOrder;

    private bool Retrying = false;

    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        AnimationTree = GetNode<AnimationTree>("Animations/AnimationTree");
        AnimationState = AnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;

        TreeNewOrder = GetNode<AnimationTree>("Animations/TreeNewOrder");
        StateNewOrder = TreeNewOrder.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
    }

    private void ConnectSignals()
    {
        Firebase.Connect("PingTimeout", this, "ShowMessage");
        Firebase.Connect("PingSuccess", this, "HideMessage");
        Firebase.Connect("NewOrders", this, "ShowNewOrderNotification");
    }

    public void ShowMessage()
    {
        if (Retrying)
        {
            return;
        }

        AnimationState.Travel("show");
        GlobalProcess.Paused = true;
        Retrying = true;
    }

    public void HideMessage()
    {
        if (!Retrying)
        {
            return;
        }

        AnimationState.Travel("hide");
        GlobalProcess.Paused = false;
        Retrying = false;
    }

    public void ShowNewOrderNotification()
    {
        StateNewOrder.Travel("show");
    }

}
