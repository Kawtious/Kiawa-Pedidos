using System;
using Godot;

using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class MenuScreen : Control
{
    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    private Control BoxLeft;

    private ScrollContainer BoxLeftScroll;

    private VBoxContainer BoxLeftVBox;

    private Control BoxRight;

    private TabContainer BoxRightTabContainer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();

        Firebase.UpdateData();
    }

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        BoxLeft = GetNode<Control>("BoxLeft");
        BoxLeftScroll = BoxLeft.GetNode<ScrollContainer>("BoxLeftScroll");
        BoxLeftVBox = BoxLeftScroll.GetNode<VBoxContainer>("BoxLeftVBox");

        BoxRight = GetNode<Control>("BoxRight");
        BoxRightTabContainer = BoxRight.GetNode<TabContainer>("TabContainer");
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");
    }

    public void _OnAddDishesButtonPressed()
    {
        Array dishes = new Array();

        foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
        {
            if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
            {
                dishes.Add(dishContainer.Dish.Id.ToString());
            }
        }

        Firebase.SetMenu(GetTabName(), dishes);
        Firebase.UpdateData();
    }

    public void _OnTabChanged(int tab)
    {
        RecheckDishContainers();
        Firebase.UpdateData();
    }

    public void UpdateData()
    {
        UpdateDishes();

        UpdateDayMenu("monday");
        UpdateDayMenu("tuesday");
        UpdateDayMenu("wednesday");
        UpdateDayMenu("thursday");
        UpdateDayMenu("friday");
    }

    private void UpdateDishes()
    {
        ClearDishList();

        if (Firebase.Dishes == null)
        {
            return;
        }

        int index = 0;

        foreach (Dictionary entry in Firebase.Dishes)
        {
            string titulo = (string)entry["titulo"];
            float precio = (float)entry["precio"];

            CreateDishContainer(index, titulo, precio);
            index++;
        }

        UpdateDayMenu(GetTabName());
    }

    private void UpdateDayMenu(string day)
    {
        VBoxContainer menuContainer = ClearMenuContainer(day);

        Array dayMenu = Firebase.GetMenu(day);

        if (dayMenu.Count > 0)
        {
            foreach (object element in dayMenu)
            {
                int index = Int32.Parse((string)element);
                Dictionary dish = Firebase.GetDish(index);
                string titulo = (string)dish["titulo"];
                float precio = (float)dish["precio"];

                CreateMenuDishContainer(menuContainer, index, titulo, precio);
            }

            RecheckDishContainers();
        }
    }

    private void CreateDishContainer(int index, string titulo, float precio)
    {
        PackedScene _dishContainer = GD.Load<PackedScene>("res://Scenes/UI/DishContainer.tscn");
        DishContainer dishContainer = (DishContainer)_dishContainer.Instance();
        BoxLeftVBox.AddChild(dishContainer);

        dishContainer.Dish.Id = index;
        dishContainer.Dish.Title = titulo;
        dishContainer.Dish.Price = precio;
    }

    private void CreateMenuDishContainer(VBoxContainer menuContainer, int index, string titulo, float precio)
    {
        PackedScene _menuDishContainer = GD.Load<PackedScene>("res://Scenes/UI/MenuDishContainer.tscn");
        MenuDishContainer menuDishContainer = (MenuDishContainer)_menuDishContainer.Instance();
        menuContainer.AddChild(menuDishContainer);

        menuDishContainer.Dish.Id = index;
        menuDishContainer.Dish.Title = titulo;
        menuDishContainer.Dish.Price = precio;
    }

    private void ClearDishList()
    {
        foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
        {
            BoxLeftVBox.RemoveChild(dishContainer);
            dishContainer.QueueFree();
        }
    }

    private VBoxContainer ClearMenuContainer(string day)
    {
        VBoxContainer menuContainer =
            BoxRightTabContainer.GetNode<VBoxContainer>(day.ToUpper() + "/ScrollContainer/VBoxContainer");

        foreach (MenuDishContainer menuDishContainer in menuContainer.GetChildren())
        {
            menuContainer.RemoveChild(menuDishContainer);
            menuDishContainer.QueueFree();
        }

        return menuContainer;
    }

    private void RecheckDishContainers()
    {
        Array dayMenu = Firebase.GetMenu(GetTabName());

        foreach (CheckBox CheckBox in GetDishCheckBoxes())
        {
            CheckBox.Pressed = false;
        }

        if (dayMenu.Count > 0)
        {
            foreach (CheckBox CheckBox in GetDishCheckBoxes())
            {
                DishContainer dishContainer = CheckBox.GetParent() as DishContainer;
                CheckBox.Pressed = dayMenu.Contains(dishContainer.Dish.Id.ToString());
            }
        }
    }

    private Array GetDishCheckBoxes()
    {
        Array dishContainers = BoxLeftVBox.GetChildren();
        Array checkBoxes = new Array();

        foreach (DishContainer dishContainer in dishContainers)
        {
            checkBoxes.Add(dishContainer.GetNode<CheckBox>("CheckBox"));
        }

        return checkBoxes;
    }

    private DishContainer GetDishContainer(int index)
    {
        foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
        {
            if (dishContainer.Dish.Id == index)
            {
                return dishContainer;
            }
        }
        return null;
    }

    private string GetTabName()
    {
        return BoxRightTabContainer.GetCurrentTabControl().Name;
    }
}
