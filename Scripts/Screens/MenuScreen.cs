using System;
using System.Collections;
using Godot;
using Godot.Collections;
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

    private AnimationTree AnimationTree;

    private AnimationNodeStateMachinePlayback AnimationState;

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

        BoxLeft = GetNode<Control>("BoxLeft");
        BoxLeftScroll = BoxLeft.GetNode<ScrollContainer>("BoxLeftScroll");
        BoxLeftVBox = BoxLeftScroll.GetNode<VBoxContainer>("BoxLeftVBox");

        BoxRight = GetNode<Control>("BoxRight");
        BoxRightTabContainer = BoxRight.GetNode<TabContainer>("TabContainer");

        AnimationTree = GetNode<AnimationTree>("Animations/AnimationTree3");
        AnimationState = AnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");

        Firebase.Connect("ValidateDishes", this, "HideNoDishesError");
        Firebase.Connect("InvalidateDishes", this, "ShowNoDishesError");
    }

    public void _OnAddDishesButtonPressed()
    {
        Array dishes = new Array();

        foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
        {
            if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
            {
                dishes.Add(dishContainer.Dish.Key);
            }
        }

        Firebase.SetMenu(GetTabName(), dishes);
    }

    public void _OnTabChanged(int tab)
    {
        RecheckDishContainers();
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

            DishContainer.CreateDishContainer(BoxLeftVBox, dish);
        }

        UpdateDayMenu(GetTabName());
    }

    private void ClearDishList()
    {
        foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
        {
            BoxLeftVBox.RemoveChild(dishContainer);
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
            DishContainer dishContainer = CheckBox.GetParent() as DishContainer;
            CheckBox.Pressed = dayMenu.Contains(dishContainer.Dish.Key);
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

    private DishContainer GetDishContainer(string key)
    {
        foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
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

            MenuDishContainer.CreateMenuDishContainer(menuContainer, dish);
        }

        RecheckDishContainers();
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

        AnimationState.Travel("showing");
        ShowingError = true;
    }

    public void HideNoDishesError()
    {
        if (!ShowingError)
        {
            return;
        }

        AnimationState.Travel("hidden");
        ShowingError = false;
    }
}
