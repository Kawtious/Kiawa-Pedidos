using Godot;
using System;

public class ContainerDishEditable : HBoxContainer
{

    private Firebase Firebase;

    private VBoxContainer Details;

    private Label LabelTitle;

    private Label LabelPrice;

    private CheckBox CheckBox;

    private Dish _Dish = new Dish();

    public Dish Dish
    {
        get { return _Dish; }
        set { _Dish = value; UpdateContainer(_Dish); }
    }

    [Signal]
    delegate void DishSelected(ContainerDishEditable container);

    [Signal]
    delegate void DishDeselected();

    public bool AlreadyPressed = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    private void InitNodes()
    {
        Firebase = GetNode<Firebase>("/root/Firebase");

        Details = GetNode<VBoxContainer>("Details");
        LabelTitle = Details.GetNode<Label>("Title");
        LabelPrice = Details.GetNode<Label>("Price");

        CheckBox = GetNode<CheckBox>("CheckBox");
    }

    private void UpdateContainer(Dish value)
    {
        LabelTitle.Text = value.Title;
        LabelPrice.Text = "$" + value.Price.ToString();
    }

    public void _OnTrashButtonPressed()
    {
        Firebase.DeleteDish(Dish.Key);
    }

    public void _OnCheckBoxPressed()
    {
        if (AlreadyPressed)
        {
            CheckBox.Pressed = false;
            AlreadyPressed = false;
            EmitSignal("DishDeselected");
        }
        else
        {
            AlreadyPressed = true;
            EmitSignal("DishSelected", this);
        }
    }

    public void _OnCheckBoxToggled(bool button_pressed)
    {
        if (!button_pressed && AlreadyPressed)
        {
            AlreadyPressed = false;
        }
    }

    public static ContainerDishEditable CreateContainerDishEditable(Node parent, Dish dish)
    {
        PackedScene _containerDishEditable = GD.Load<PackedScene>("res://Scenes/UI/ContainerDishEditable.tscn");
        ContainerDishEditable containerDishEditable = (ContainerDishEditable)_containerDishEditable.Instance();
        parent.AddChild(containerDishEditable);

        containerDishEditable.Dish = dish;

        return containerDishEditable;
    }
}
