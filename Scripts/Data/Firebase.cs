using Godot;
using System.Linq;
using System.Text;

using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class Firebase : Node
{

    public const string MENU_STRING = "menu";

    public const string DISHES_STRING = "dishes";

    public const string ORDERS_STRING = "orders";

    public const string DATABASE_REFERENCE = "https://kiawa-service-default-rtdb.firebaseio.com/";

    public const string DATA_REFERENCE = DATABASE_REFERENCE + "data/";

    public const string MENU_REFERENCE = DATA_REFERENCE + MENU_STRING + "/";

    public const string DISH_REFERENCE = DATA_REFERENCE + DISHES_STRING + "/";

    public const string ORDER_REFERENCE = DATA_REFERENCE + ORDERS_STRING + "/";

    public const string JSON_EXTENSION = ".json";

    private Dictionary _Data = new Dictionary();

    private Dictionary Data
    {
        get { return _Data; }
        set { _Data = value; ValidateData(); }
    }

    public Dictionary Menu => GetMenus(Data);

    public Dictionary Dishes => GetDishes(Data);

    public Dictionary Orders => GetOrders(Data);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        UpdateData();
        Ping();
    }

    private GlobalProcess GlobalProcess;

    private Timer Timer;

    private Timer TimerPing;

    private RESTRequest RequestData;

    private RESTRequest RequestSetMenu;

    private RESTRequest RequestSendOrder;

    private RESTRequest RequestCreateDish;

    private RESTRequest RequestPing;

    private RESTRequest RequestDeleteOrder;

    private RESTRequest RequestDeleteDish;

    [Signal]
    delegate void UpdatedData();

    [Signal]
    delegate void ValidateMenus();

    [Signal]
    delegate void ValidateDishes();

    [Signal]
    delegate void ValidateOrders();

    [Signal]
    delegate void InvalidateMenus();

    [Signal]
    delegate void InvalidateDishes();

    [Signal]
    delegate void InvalidateOrders();

    [Signal]
    delegate void PingSuccess();

    [Signal]
    delegate void PingTimeout();

    [Signal]
    delegate void NewOrders();

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");

        Timer = GetNode<Timer>("Timer");
        TimerPing = GetNode<Timer>("TimerPing");

        RequestData = GetNode<RESTRequest>("RequestData");
        RequestSetMenu = GetNode<RESTRequest>("RequestSetMenu");
        RequestSendOrder = GetNode<RESTRequest>("RequestSendOrder");
        RequestCreateDish = GetNode<RESTRequest>("RequestCreateDish");
        RequestPing = GetNode<RESTRequest>("RequestPing");
        RequestDeleteOrder = GetNode<RESTRequest>("RequestDeleteOrder");
        RequestDeleteDish = GetNode<RESTRequest>("RequestDeleteDish");
    }

    public void Ping()
    {
        RequestPing.Get(DATABASE_REFERENCE);
        TimerPing.Start(2);
    }

    private Dictionary GetMenus(Dictionary data)
    {
        if (!data.Contains(MENU_STRING))
        {
            return null;
        }

        return data[MENU_STRING] as Dictionary;
    }

    private Dictionary GetDishes(Dictionary data)
    {
        if (!data.Contains(DISHES_STRING))
        {
            return null;
        }

        return data[DISHES_STRING] as Dictionary;
    }

    private Dictionary GetOrders(Dictionary data)
    {
        if (!data.Contains(ORDERS_STRING))
        {
            return null;
        }

        return data[ORDERS_STRING] as Dictionary;
    }

    public Array GetMenu(string day)
    {
        if (Menu == null)
        {
            return null;
        }

        if (!Menu.Contains(day.ToLower()))
        {
            return null;
        }

        return Menu[day.ToLower()] as Array;
    }

    public Dish GetDish(string key)
    {
        if (Dishes == null)
        {
            return null;
        }

        if (!Dishes.Contains(key))
        {
            return null;
        }

        Dish dish = Dish.FromMap(Dishes[key] as Dictionary);
        dish.Key = key;

        return dish;
    }

    public Dictionary GetOrder(string key)
    {
        if (Orders == null)
        {
            return null;
        }

        if (!Orders.Contains(key))
        {
            return null;
        }

        return Orders[key] as Dictionary;
    }

    public Array MenuGetToday()
    {
        return GetMenu(GlobalProcess.Today);
    }

    public void MenuSet(string day, Array dishes)
    {
        RequestSetMenu.Put(MENU_REFERENCE + day.ToLower(), dishes);
    }

    public void DishCreate(string key, Dish dish)
    {
        if (!key.Empty())
        {
            RequestCreateDish.Put(DISH_REFERENCE + "/" + key, dish.ToMap());
        }
        else
        {
            RequestCreateDish.Post(DISH_REFERENCE, dish.ToMap());
        }
    }

    public void DishDelete(string key)
    {
        if (key.Empty())
        {
            return;
        }

        string reference = DISH_REFERENCE + "/" + key;

        RequestDeleteDish.Delete(reference);
    }

    public void OrderSend(Array dishes)
    {
        string today = System.DateTime.Now.ToString();

        // It's possible that someone can get a duplicate ticket
        int ticketNumber = (int)GD.RandRange(1, 10000);

        string user = $"Ticket-{ticketNumber}";

        Order order = new Order(user, today, dishes);

        RequestSendOrder.Post(ORDER_REFERENCE, order.ToMap());
    }

    public void OrderDelete(string key)
    {
        if (key.Empty())
        {
            return;
        }

        string reference = ORDER_REFERENCE + "/" + key;

        RequestDeleteOrder.Delete(reference);
    }

    public void UpdateData()
    {
        RequestData.Get(DATA_REFERENCE);
    }

    private void CompareOrders(Dictionary oldOrders, Dictionary newOrders)
    {
        if (oldOrders == null && newOrders == null)
        {
            return;
        }

        if (oldOrders == null && newOrders != null)
        {
            EmitSignal("NewOrders");
            return;
        }

        if (oldOrders != null && newOrders == null)
        {
            return;
        }

        if (oldOrders.Count < newOrders.Count)
        {
            EmitSignal("NewOrders");
        }
    }

    private void ValidateData()
    {
        string menuValidation = DoValidateMenus(Data) ? "ValidateMenus" : "InvalidateMenus";
        string dishValidation = DoValidateDishes(Data) ? "ValidateDishes" : "InvalidateDishes";
        string orderValidation = DoValidateOrders(Data) ? "ValidateOrders" : "InvalidateOrders";

        EmitSignal(menuValidation);
        EmitSignal(dishValidation);
        EmitSignal(orderValidation);
    }

    private bool DoValidateMenus(Dictionary data)
    {
        return DoValidateData(GetMenus(data));
    }

    private bool DoValidateDishes(Dictionary data)
    {
        return DoValidateData(GetDishes(data));
    }

    private bool DoValidateOrders(Dictionary data)
    {
        return DoValidateData(GetOrders(data));
    }

    private bool DoValidateData(Dictionary data)
    {
        if (data == null)
        {
            return false;
        }

        return data.Count > 0;
    }

    public void _OnPingRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        EmitSignal("PingSuccess");
        Ping();
    }

    public void _OnDataRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        if (body == null)
        {
            return;
        }

        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object _result = json.Result;

        Dictionary data = new Dictionary();

        if (_result != null)
        {
            if (_result is Dictionary)
            {
                data = _result as Dictionary;
            }
        }

        if (Data.ToString().Equals(data.ToString()))
        {
            return;
        }

        CompareOrders(GetOrders(Data), GetOrders(data));

        Data = data;

        EmitSignal("UpdatedData");

        Timer.Start(5);
    }

    public void _OnSetMenuRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

    public void _OnSendOrderRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

    public void _OnCreateDishRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

    public void _OnDeleteOrderRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

    public void _OnDeleteDishRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

    public void _OnTimerTimeout()
    {
        UpdateData();
    }

    public void _OnPingTimerTimeout()
    {
        EmitSignal("PingTimeout");
        Ping();
    }

}
