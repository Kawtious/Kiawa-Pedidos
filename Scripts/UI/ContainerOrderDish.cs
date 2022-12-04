using Godot;
using System;

public class ContainerOrderDish : VBoxContainer
{

    private Firebase Firebase;

    public HBoxContainer Container;

    public VBoxContainer Details;

    public Label LabelTitle;

    public CheckBox CheckBox;

    private Dish _Dish = new Dish();

    public Dish Dish
    {
        get { return _Dish; }
        set { _Dish = value; UpdateContainer(_Dish); }
    }

    [Signal]
    delegate void AmountChanged();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    private void InitNodes()
    {
        Firebase = GetNode<Firebase>("/root/Firebase");

        Container = GetNode<HBoxContainer>("Container");
        Details = Container.GetNode<VBoxContainer>("Details");
        LabelTitle = Details.GetNode<Label>("Title");
        CheckBox = Container.GetNode<CheckBox>("CheckBox");
    }

    private void UpdateContainer(Dish value)
    {
        LabelTitle.Text = value.Title;
    }

    public void _OnCheckBoxPressed()
    {
        EmitSignal("AmountChanged");
    }

    public static ContainerOrderDish CreateOrderDishContainer(Node parent, Dish dish)
    {
        PackedScene _orderDishContainer = GD.Load<PackedScene>("res://Scenes/UI/ContainerOrderDish.tscn");
        ContainerOrderDish orderDishContainer = (ContainerOrderDish)_orderDishContainer.Instance();
        parent.AddChild(orderDishContainer);

        orderDishContainer.Dish = dish;
        return orderDishContainer;
    }
}
