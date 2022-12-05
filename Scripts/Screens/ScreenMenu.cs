using System;
using System.Collections;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class ScreenMenu : Control
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    private Control BoxLeft;

    private ScrollContainer ScrollContainer;

    private VBoxContainer ContainerVBoxLeft;

    private Control BoxRight;

    private TabContainer BoxRightTabContainer;

    private AnimationTree TreeNoDishes;

    private AnimationNodeStateMachinePlayback StateNoDishes;

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        BoxLeft = GetNode<Control>("BoxLeft");
        ScrollContainer = BoxLeft.GetNode<ScrollContainer>("ScrollContainer");
        ContainerVBoxLeft = ScrollContainer.GetNode<VBoxContainer>("ContainerVBoxLeft");

        BoxRight = GetNode<Control>("BoxRight");
        BoxRightTabContainer = BoxRight.GetNode<TabContainer>("TabContainer");

        TreeNoDishes = GetNode<AnimationTree>("Animations/TreeNoDishes");
        StateNoDishes = TreeNoDishes.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");

        Firebase.Connect("ValidateDishes", this, "HideNoDishesError");
        Firebase.Connect("InvalidateDishes", this, "ShowNoDishesError");
    }

    public void UpdateData()
    {
        UpdateDishes();

        foreach (Control tab in GetTabs())
        {
            UpdateDayMenu(tab.Name.ToLower());
        }
    }

    private void UpdateDishes()
    {
        ClearDishList();

        if (Firebase.Dishes == null)
        {
            return;
        }

        foreach (DictionaryEntry entry in Firebase.Dishes)
        {
            Dictionary map = entry.Value as Dictionary;

            Dish dish = Dish.FromMap(map);
            dish.Key = (string)entry.Key;

            ContainerDishAdd.CreateDishAddContainer(ContainerVBoxLeft, dish);
        }

        UpdateDayMenu(GetTabName());
    }

    private void ClearDishList()
    {
        foreach (ContainerDishAdd dishContainer in ContainerVBoxLeft.GetChildren())
        {
            ContainerVBoxLeft.RemoveChild(dishContainer);
            dishContainer.QueueFree();
        }
    }

    private void RecheckDishContainers()
    {
        foreach (CheckBox CheckBox in GetDishCheckBoxes())
        {
            CheckBox.Pressed = false;
        }

        Array dayMenu = Firebase.GetMenu(GetTabName());

        if (dayMenu == null)
        {
            return;
        }

        if (dayMenu.Count < 1)
        {
            return;
        }

        foreach (CheckBox CheckBox in GetDishCheckBoxes())
        {
            ContainerDishAdd dishContainer = CheckBox.GetParent() as ContainerDishAdd;
            CheckBox.Pressed = dayMenu.Contains(dishContainer.Dish.Key);
        }
    }

    private Array GetDishCheckBoxes()
    {
        Array dishContainers = ContainerVBoxLeft.GetChildren();
        Array checkBoxes = new Array();

        foreach (ContainerDishAdd dishContainer in dishContainers)
        {
            checkBoxes.Add(dishContainer.GetNode<CheckBox>("CheckBox"));
        }

        return checkBoxes;
    }

    private ContainerDishAdd GetDishContainer(string key)
    {
        foreach (ContainerDishAdd dishContainer in ContainerVBoxLeft.GetChildren())
        {
            if (dishContainer.Dish.Key.Equals(key))
            {
                return dishContainer;
            }
        }
        return null;
    }

    private void UpdateDayMenu(string day)
    {
        VBoxContainer menuContainer = ClearMenuContainer(day);

        Array dayMenu = Firebase.GetMenu(day);

        if (dayMenu == null)
        {
            return;
        }

        if (dayMenu.Count < 1)
        {
            return;
        }

        foreach (object element in dayMenu)
        {
            string key = (string)element;

            Dish dish = Firebase.GetDish(key);

            if (dish == null)
            {
                continue;
            }

            ContainerMenuDish.CreateMenuDishContainer(menuContainer, dish);
        }

        RecheckDishContainers();
    }

    private VBoxContainer ClearMenuContainer(string day)
    {
        VBoxContainer menuContainer =
            BoxRightTabContainer.GetNode<VBoxContainer>(day.ToUpper() + "/ScrollContainer/VBoxContainer");

        foreach (ContainerMenuDish menuDishContainer in menuContainer.GetChildren())
        {
            menuContainer.RemoveChild(menuDishContainer);
            menuDishContainer.QueueFree();
        }

        return menuContainer;
    }

    private string GetTabName()
    {
        return BoxRightTabContainer.GetCurrentTabControl().Name;
    }

    private Array<Control> GetTabs()
    {
        Array<Control> tabs = new Array<Control>();

        foreach (Control tab in BoxRightTabContainer.GetChildren())
        {
            tabs.Add(tab);
        }

        return tabs;
    }

    private bool ShowingError = true;

    public void ShowNoDishesError()
    {
        if (ShowingError)
        {
            return;
        }

        StateNoDishes.Travel("showing");
        ShowingError = true;
    }

    public void HideNoDishesError()
    {
        if (!ShowingError)
        {
            return;
        }

        StateNoDishes.Travel("hidden");
        ShowingError = false;
    }

    public void _OnAddDishesButtonPressed()
    {
        Array dishes = new Array();

        foreach (ContainerDishAdd dishContainer in ContainerVBoxLeft.GetChildren())
        {
            if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
            {
                dishes.Add(dishContainer.Dish.Key);
            }
        }

        Firebase.MenuSet(GetTabName(), dishes);
    }

    public void _OnTabChanged(int tab)
    {
        RecheckDishContainers();
    }
}
