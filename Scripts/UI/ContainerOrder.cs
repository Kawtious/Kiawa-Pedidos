using Godot;
using System;

public class ContainerOrder : HBoxContainer
{

    private Firebase Firebase;

    public VBoxContainer Details;

    public Label LabelUser;

    public Label LabelDate;

    private Order _Order = new Order();

    public Order Order
    {
        get { return _Order; }
        set { _Order = value; UpdateContainer(_Order); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    private void InitNodes()
    {
        Firebase = GetNode<Firebase>("/root/Firebase");
        Details = GetNode<VBoxContainer>("Details");
        LabelUser = Details.GetNode<Label>("User");
        LabelDate = Details.GetNode<Label>("Date");
    }

    public void _OnTrashButtonPressed()
    {
        Firebase.DeleteOrder(Order.Key);
    }

    private void UpdateContainer(Order value)
    {
        LabelUser.Text = value.User;
        LabelDate.Text = value.Date;
    }

    public static void CreateOrderContainer(Node parent, Order order)
    {
        PackedScene _orderContainer = GD.Load<PackedScene>("res://Scenes/UI/ContainerOrder.tscn");
        ContainerOrder orderContainer = (ContainerOrder)_orderContainer.Instance();
        parent.AddChild(orderContainer);

        orderContainer.Order = order;
    }
}
