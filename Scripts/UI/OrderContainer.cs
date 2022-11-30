using Godot;
using System;

public class OrderContainer : HBoxContainer
{

    public Order Order;

    public VBoxContainer Details;

    public Label LabelUser;

    public Label LabelDate;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
        InitView();
    }

    private void InitNodes()
    {
        Order = GetNode<Order>("Order");
        Details = GetNode<VBoxContainer>("Details");
        LabelUser = Details.GetNode<Label>("User");
        LabelDate = Details.GetNode<Label>("Date");
    }

    private void ConnectSignals()
    {
        Order.Connect("UpdatedUser", this, "SetUser");
        Order.Connect("UpdatedDate", this, "SetDate");
    }

    private void InitView()
    {
        LabelUser.Text = Order.User;
        LabelDate.Text = Order.Date;
    }

    public void SetUser(string value)
    {
        LabelUser.Text = value;
    }

    public void SetDate(string value)
    {
        LabelDate.Text = value;
    }
}
