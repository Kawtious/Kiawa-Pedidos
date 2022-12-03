using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;
using System.Collections;

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
        Firebase.Connect("NewOrders", this, "NotifyNewOrders");
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

        foreach (DictionaryEntry entry in Firebase.Orders)
        {
            Dictionary map = entry.Value as Dictionary;

            Order order = Order.FromMap(map);
            order.Key = (string)entry.Key;

            if (order == null)
            {
                continue;
            }

            if (Query.Empty())
            {
                OrderContainer.CreateOrderContainer(BoxVBox, order);
            }
            else if (order.User.ToLower().Contains(Query.ToLower()) ||
                    order.Date.ToLower().Contains(Query.ToLower()))
            {
                OrderContainer.CreateOrderContainer(BoxVBox, order);
            }
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

    public void NotifyNewOrders()
    {
        GlobalProcess.ResourceDirector.MenuAudioStream.Stream =
                    (AudioStream)GlobalProcess.ResourceDirector.GetResource("new_orders");

        GlobalProcess.ResourceDirector.MenuAudioStream.Play();
    }
}
