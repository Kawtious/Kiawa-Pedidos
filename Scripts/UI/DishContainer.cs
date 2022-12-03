using Godot;

public class DishContainer : HBoxContainer
{

    public VBoxContainer Details;

    public Label LabelTitle;

    public Label LabelPrice;

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

    public static void CreateDishContainer(Node parent, Dish dish)
    {
        PackedScene _dishContainer = GD.Load<PackedScene>("res://Scenes/UI/DishContainer.tscn");
        DishContainer dishContainer = (DishContainer)_dishContainer.Instance();
        parent.AddChild(dishContainer);

        dishContainer.Dish = dish;
    }
}
