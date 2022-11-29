using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

public class ManageOrderScreen : Control
{

    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    private Control Box;

    private ScrollContainer BoxScroll;

    private VBoxContainer BoxVBox;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        Box = GetNode<Control>("Box");
        BoxScroll = Box.GetNode<ScrollContainer>("BoxScroll");
        BoxVBox = BoxScroll.GetNode<VBoxContainer>("BoxVBox");
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");
    }

    public void UpdateData()
    {
        UpdateOrders();
    }

    private void UpdateOrders()
    {
        ClearOrdersList();

        foreach (System.Collections.DictionaryEntry entry in Firebase.Orders)
        {
            PackedScene _orderContainer = GD.Load<PackedScene>("res://Scenes/UI/OrderContainer.tscn");
            OrderContainer orderContainer = (OrderContainer)_orderContainer.Instance();
            BoxVBox.AddChild(orderContainer);

            Dictionary element = entry.Value as Dictionary;

            orderContainer.Order.User = (string)element["user"];
            orderContainer.Order.Date = (string)element["date"];
        }
    }

    private void ClearOrdersList()
    {
        foreach (OrderContainer orderContainer in BoxVBox.GetChildren())
        {
            BoxVBox.RemoveChild(orderContainer);
            orderContainer.QueueFree();
        }
    }
}
