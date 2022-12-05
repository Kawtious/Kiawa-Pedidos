using Godot;
using System;

public class ContainerDishAdd : HBoxContainer
{

    private Dish _Dish = new Dish();

    public Dish Dish
    {
        get { return _Dish; }
        set { _Dish = value; UpdateContainer(_Dish); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    public VBoxContainer Details;

    public Label LabelTitle;

    public Label LabelPrice;

    private void InitNodes()
    {
        Details = GetNode<VBoxContainer>("Details");
        LabelTitle = Details.GetNode<Label>("Title");
        LabelPrice = Details.GetNode<Label>("Price");
    }

    private void UpdateContainer(Dish value)
    {
        LabelTitle.Text = value.Title;
        LabelPrice.Text = "$" + value.Price.ToString();
    }

    public static ContainerDishAdd CreateDishAddContainer(Node parent, Dish dish)
    {
        PackedScene _dishContainer = GD.Load<PackedScene>("res://Scenes/UI/ContainerDishAdd.tscn");
        ContainerDishAdd dishContainer = (ContainerDishAdd)_dishContainer.Instance();
        parent.AddChild(dishContainer);

        dishContainer.Dish = dish;
        return dishContainer;
    }
}
