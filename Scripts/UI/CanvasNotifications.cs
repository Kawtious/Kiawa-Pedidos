using Godot;

public class CanvasNotifications : CanvasLayer
{

    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    private AnimationTree TreeLostConnection;

    private AnimationNodeStateMachinePlayback StateLostConnection;

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

        TreeLostConnection = GetNode<AnimationTree>("Animations/TreeLostConnection");
        StateLostConnection = TreeLostConnection.Get("parameters/playback") as AnimationNodeStateMachinePlayback;

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

        StateLostConnection.Travel("show");
        GlobalProcess.Paused = true;
        Retrying = true;
    }

    public void HideMessage()
    {
        if (!Retrying)
        {
            return;
        }

        StateLostConnection.Travel("hide");
        GlobalProcess.Paused = false;
        Retrying = false;
    }

    public void ShowNewOrderNotification()
    {
        StateNewOrder.Travel("show");
    }

}
