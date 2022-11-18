using Godot;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;
using System.Text;
using System;
using System.Linq;

public class MenuScreen : Control
{

	private GlobalProcess GlobalProcess;

	private Firebase Firebase;

	private Control BoxLeft;

	private ScrollContainer BoxLeftScroll;

	private VBoxContainer BoxLeftVBox;

	private Control BoxRight;

	private TabContainer TabContainer;

	private const int DAY_MONDAY = 0,
				DAY_TUESDAY = 1,
				DAY_WEDNESDAY = 2,
				DAY_THURSDAY = 3,
				DAY_FRIDAY = 4;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitNodes();
		ConnectSignals();

		Firebase.UpdateDishes();
		Firebase.UpdateMenu(Firebase.MondayMenuRequest, Firebase.MON_REFERENCE);
	}

	private void InitNodes()
	{
		GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
		Firebase = GetNode<Firebase>("/root/Firebase");

		BoxLeft = GetNode<Control>("BoxLeft");
		BoxLeftScroll = BoxLeft.GetNode<ScrollContainer>("BoxLeftScroll");
		BoxLeftVBox = BoxLeftScroll.GetNode<VBoxContainer>("BoxLeftVBox");

		BoxRight = GetNode<Control>("BoxRight");
		TabContainer = BoxRight.GetNode<TabContainer>("TabContainer");
	}

	private void ConnectSignals()
	{
		Firebase.Connect("UpdatedDishList", this, "UpdateDishList");
		Firebase.Connect("UpdatedMondayMenu", this, "UpdateMondayMenu");
		Firebase.Connect("UpdatedTuesdayMenu", this, "UpdateTuesdayMenu");
		Firebase.Connect("UpdatedWednesdayMenu", this, "UpdateWednesdayMenu");
		Firebase.Connect("UpdatedThursdayMenu", this, "UpdateThursdayMenu");
		Firebase.Connect("UpdatedFridayMenu", this, "UpdateFridayMenu");
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		switch (evt)
		{
			case InputEventKey keyEvent:
				HandleInput();
				break;
			default:
				break;
		}
	}

	private void HandleInput()
	{
		if (Input.IsActionJustPressed("ui_select"))
		{
			Firebase.UpdateDishes();
		}
	}

	public void UpdateDishList()
	{
		ClearDishList();

		int Index = 0;

		foreach (Dictionary entry in Firebase.Dishes)
		{
			PackedScene _DishContainer = GD.Load<PackedScene>("res://Scenes/UI/DishContainer.tscn");
			DishContainer dishContainer = (DishContainer)_DishContainer.Instance();
			BoxLeftVBox.AddChild(dishContainer);

			dishContainer.Dish.Id = Index;
			dishContainer.Dish.Title = (string)entry["titulo"];
			dishContainer.Dish.Price = (float)entry["precio"];
			Index++;
		}

		switch (TabContainer.CurrentTab)
		{
			case DAY_MONDAY:
				RecheckDishContainers(Firebase.MondayMenu);
				break;
			case DAY_TUESDAY:
				RecheckDishContainers(Firebase.TuesdayMenu);
				break;
			case DAY_WEDNESDAY:
				RecheckDishContainers(Firebase.WednesdayMenu);
				break;
			case DAY_THURSDAY:
				RecheckDishContainers(Firebase.ThursdayMenu);
				break;
			case DAY_FRIDAY:
				RecheckDishContainers(Firebase.FridayMenu);
				break;
		}
	}

	private void ClearDishList()
	{
		foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
		{
			BoxLeftVBox.RemoveChild(dishContainer);
			dishContainer.QueueFree();
		}
	}

	public void UpdateMondayMenu()
	{
		VBoxContainer Menu = TabContainer.GetNode<VBoxContainer>("MONDAY/ScrollContainer/VBoxContainer");

		foreach (MenuDishContainer menuDishContainer in Menu.GetChildren())
		{
			Menu.RemoveChild(menuDishContainer);
			menuDishContainer.QueueFree();
		}

		if (Firebase.MondayMenu != null)
		{
			foreach (object element in Firebase.MondayMenu)
			{
				PackedScene _MenuDishContainer = GD.Load<PackedScene>("res://Scenes/UI/MenuDishContainer.tscn");
				MenuDishContainer menuDishContainer = (MenuDishContainer)_MenuDishContainer.Instance();
				Menu.AddChild(menuDishContainer);

				int Index = Int32.Parse((string)element);

				Dictionary Dish = Firebase.Dishes[Index] as Dictionary;

				menuDishContainer.Dish.Id = Index;
				menuDishContainer.Dish.Title = (string)Dish["titulo"];
				menuDishContainer.Dish.Price = (float)Dish["precio"];
			}
		}

		RecheckDishContainers(Firebase.MondayMenu);
	}

	public void UpdateTuesdayMenu()
	{
		VBoxContainer Menu = TabContainer.GetNode<VBoxContainer>("TUESDAY/ScrollContainer/VBoxContainer");

		foreach (MenuDishContainer menuDishContainer in Menu.GetChildren())
		{
			Menu.RemoveChild(menuDishContainer);
			menuDishContainer.QueueFree();
		}


		if (Firebase.TuesdayMenu != null)
		{
			foreach (object element in Firebase.TuesdayMenu)
			{
				PackedScene _MenuDishContainer = GD.Load<PackedScene>("res://Scenes/UI/MenuDishContainer.tscn");
				MenuDishContainer menuDishContainer = (MenuDishContainer)_MenuDishContainer.Instance();
				Menu.AddChild(menuDishContainer);

				int Index = Int32.Parse((string)element);

				Dictionary Dish = Firebase.Dishes[Index] as Dictionary;

				menuDishContainer.Dish.Id = Index;
				menuDishContainer.Dish.Title = (string)Dish["titulo"];
				menuDishContainer.Dish.Price = (float)Dish["precio"];
			}
		}

		RecheckDishContainers(Firebase.TuesdayMenu);
	}

	public void UpdateWednesdayMenu()
	{
		VBoxContainer Menu = TabContainer.GetNode<VBoxContainer>("WEDNESDAY/ScrollContainer/VBoxContainer");

		foreach (MenuDishContainer menuDishContainer in Menu.GetChildren())
		{
			Menu.RemoveChild(menuDishContainer);
			menuDishContainer.QueueFree();
		}


		if (Firebase.WednesdayMenu != null)
		{
			foreach (object element in Firebase.WednesdayMenu)
			{
				PackedScene _MenuDishContainer = GD.Load<PackedScene>("res://Scenes/UI/MenuDishContainer.tscn");
				MenuDishContainer menuDishContainer = (MenuDishContainer)_MenuDishContainer.Instance();
				Menu.AddChild(menuDishContainer);

				int Index = Int32.Parse((string)element);

				Dictionary Dish = Firebase.Dishes[Index] as Dictionary;

				menuDishContainer.Dish.Id = Index;
				menuDishContainer.Dish.Title = (string)Dish["titulo"];
				menuDishContainer.Dish.Price = (float)Dish["precio"];
			}
		}

		RecheckDishContainers(Firebase.WednesdayMenu);
	}

	public void UpdateThursdayMenu()
	{
		VBoxContainer Menu = TabContainer.GetNode<VBoxContainer>("THURSDAY/ScrollContainer/VBoxContainer");

		foreach (MenuDishContainer menuDishContainer in Menu.GetChildren())
		{
			Menu.RemoveChild(menuDishContainer);
			menuDishContainer.QueueFree();
		}


		if (Firebase.ThursdayMenu != null)
		{
			foreach (object element in Firebase.ThursdayMenu)
			{
				PackedScene _MenuDishContainer = GD.Load<PackedScene>("res://Scenes/UI/MenuDishContainer.tscn");
				MenuDishContainer menuDishContainer = (MenuDishContainer)_MenuDishContainer.Instance();
				Menu.AddChild(menuDishContainer);

				int Index = Int32.Parse((string)element);

				Dictionary Dish = Firebase.Dishes[Index] as Dictionary;

				menuDishContainer.Dish.Id = Index;
				menuDishContainer.Dish.Title = (string)Dish["titulo"];
				menuDishContainer.Dish.Price = (float)Dish["precio"];
			}
		}

		RecheckDishContainers(Firebase.ThursdayMenu);
	}

	public void UpdateFridayMenu()
	{
		VBoxContainer Menu = TabContainer.GetNode<VBoxContainer>("FRIDAY/ScrollContainer/VBoxContainer");

		foreach (MenuDishContainer menuDishContainer in Menu.GetChildren())
		{
			Menu.RemoveChild(menuDishContainer);
			menuDishContainer.QueueFree();
		}


		if (Firebase.FridayMenu != null)
		{
			foreach (object element in Firebase.FridayMenu)
			{
				PackedScene _MenuDishContainer = GD.Load<PackedScene>("res://Scenes/UI/MenuDishContainer.tscn");
				MenuDishContainer menuDishContainer = (MenuDishContainer)_MenuDishContainer.Instance();
				Menu.AddChild(menuDishContainer);

				int Index = Int32.Parse((string)element);

				Dictionary Dish = Firebase.Dishes[Index] as Dictionary;

				menuDishContainer.Dish.Id = Index;
				menuDishContainer.Dish.Title = (string)Dish["titulo"];
				menuDishContainer.Dish.Price = (float)Dish["precio"];
			}
		}

		RecheckDishContainers(Firebase.FridayMenu);
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

	private void RecheckDishContainers(Array menu)
	{
		foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
		{
			dishContainer.GetNode<CheckBox>("CheckBox").Pressed = false;
		}

		foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
		{
			if (menu != null)
			{
				if (menu.Contains(dishContainer.Dish.Id.ToString()))
				{
					dishContainer.GetNode<CheckBox>("CheckBox").Pressed = true;
				}
			}
		}
	}

	private void UpdateTab(int tab)
	{
		switch (tab)
		{
			case DAY_MONDAY:
				Firebase.UpdateMenu(Firebase.MondayMenuRequest, Firebase.MON_REFERENCE);
				break;
			case DAY_TUESDAY:
				Firebase.UpdateMenu(Firebase.TuedayMenuRequest, Firebase.TUE_REFERENCE);
				break;
			case DAY_WEDNESDAY:
				Firebase.UpdateMenu(Firebase.WeddayMenuRequest, Firebase.WED_REFERENCE);
				break;
			case DAY_THURSDAY:
				Firebase.UpdateMenu(Firebase.ThudayMenuRequest, Firebase.THU_REFERENCE);
				break;
			case DAY_FRIDAY:
				Firebase.UpdateMenu(Firebase.FridayMenuRequest, Firebase.FRI_REFERENCE);
				break;
		}
	}

	public void _OnFetchDishesButtonPressed()
	{
		Firebase.UpdateDishes();
	}

	public void _OnAddDishesButtonPressed()
	{
		string Reference = Firebase.MENU_REFERENCE;

		switch (TabContainer.CurrentTab)
		{
			case DAY_MONDAY:
				Reference += Firebase.MON_REFERENCE;
				break;
			case DAY_TUESDAY:
				Reference += Firebase.TUE_REFERENCE;
				break;
			case DAY_WEDNESDAY:
				Reference += Firebase.WED_REFERENCE;
				break;
			case DAY_THURSDAY:
				Reference += Firebase.THU_REFERENCE;
				break;
			case DAY_FRIDAY:
				Reference += Firebase.FRI_REFERENCE;
				break;
		}

		Reference += Firebase.JSON_EXTENSION;

		Array Dishes = new Array();

		foreach (DishContainer dishContainer in BoxLeftVBox.GetChildren())
		{
			if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
			{
				Dishes.Add(dishContainer.Dish.Id.ToString());
			}
		}

		Firebase.MakePUTRequest(Reference, Dishes);
		UpdateTab(TabContainer.CurrentTab);
	}

	public void _OnTabChanged(int tab)
	{
		UpdateTab(tab);
	}

}
