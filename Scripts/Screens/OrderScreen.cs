using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

public class OrderScreen : Control
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

        string today = System.DateTime.Now.DayOfWeek.ToString();
        Box.GetNode<Label>("Label").Text = today;
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");
    }

    public void _OnSendOrderButtonPressed()
    {
        Array dishes = new Array();

        foreach (DishContainer dishContainer in BoxVBox.GetChildren())
        {
            if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
            {
                dishes.Add(dishContainer.Dish.Id.ToString());
            }
        }

        Firebase.SendOrder(dishes);
        Firebase.UpdateData();
    }

    public void UpdateData()
    {
        string today = System.DateTime.Now.DayOfWeek.ToString();
        UpdateDayMenu(today);
    }

    private void UpdateDayMenu(string day)
    {
        ClearDishList();

        Array dayMenu = Firebase.GetMenu(day);

        if (dayMenu.Count > 0)
        {
            foreach (object element in dayMenu)
            {
                PackedScene _dishContainer = GD.Load<PackedScene>("res://Scenes/UI/DishContainer.tscn");
                DishContainer dishContainer = (DishContainer)_dishContainer.Instance();
                BoxVBox.AddChild(dishContainer);

                int index = Int32.Parse((string)element);

                Dictionary dish = Firebase.GetDish(index);

                dishContainer.Dish.Id = index;
                dishContainer.Dish.Title = (string)dish["titulo"];
                dishContainer.Dish.Price = (float)dish["precio"];
            }
        }
    }

    private void ClearDishList()
    {
        foreach (DishContainer dishContainer in BoxVBox.GetChildren())
        {
            BoxVBox.RemoveChild(dishContainer);
            dishContainer.QueueFree();
        }
    }

    private Array GetDishCheckBoxes()
    {
        Array dishContainers = BoxVBox.GetChildren();
        Array checkBoxes = new Array();

        foreach (DishContainer dishContainer in dishContainers)
        {
            checkBoxes.Add(dishContainer.GetNode<CheckBox>("CheckBox"));
        }

        return checkBoxes;
    }

    private DishContainer GetDishContainer(int index)
    {
        foreach (DishContainer dishContainer in BoxVBox.GetChildren())
        {
            if (dishContainer.Dish.Id == index)
            {
                return dishContainer;
            }
        }
        return null;
    }
}
