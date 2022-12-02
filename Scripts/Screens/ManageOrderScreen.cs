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

    private string _Query = "";

    [Export]
    public string Query
    {
        get { return _Query; }
        set { _Query = value; UpdateOrders(); }
    }

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

    public void _OnLineEditTextChanged(string new_text)
    {
        Query = new_text;
    }

    public void UpdateData()
    {
        UpdateOrders();
    }

    private void UpdateOrders()
    {
        ClearOrdersList();

        if (Firebase.Orders == null)
        {
            return;
        }

        foreach (System.Collections.DictionaryEntry entry in Firebase.Orders)
        {
            Dictionary element = entry.Value as Dictionary;
            string user = (string)element["user"];
            string date = (string)element["date"];

            if (Query.Empty())
            {
                CreateOrderContainer(user, date);

            }
            else if (user.ToLower().Contains(Query.ToLower()))
            {
                CreateOrderContainer(user, date);
            }
        }
    }

    private void CreateOrderContainer(string user, string date)
    {
        PackedScene _orderContainer = GD.Load<PackedScene>("res://Scenes/UI/OrderContainer.tscn");
        OrderContainer orderContainer = (OrderContainer)_orderContainer.Instance();
        BoxVBox.AddChild(orderContainer);

        orderContainer.Order.User = user;
        orderContainer.Order.Date = date;
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
