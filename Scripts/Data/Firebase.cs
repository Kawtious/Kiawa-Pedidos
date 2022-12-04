using Godot;
using System.Linq;
using System.Text;

using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class Firebase : Node
{

    private Timer Timer;

    private Timer TimerPing;

    private HTTPRequest RequestData;

    private HTTPRequest RequestSetMenu;

    private HTTPRequest RequestSendOrder;

    private HTTPRequest RequestCreateDish;

    private HTTPRequest RequestPing;

    private HTTPRequest RequestDeleteOrder;

    private HTTPRequest RequestDeleteDish;

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

    public const string DATABASE_REFERENCE = "https://kiawa-service-default-rtdb.firebaseio.com/";

    public const string DATA_REFERENCE = DATABASE_REFERENCE + "data/";

    public const string MENU_REFERENCE = DATA_REFERENCE + "menu/";

    public const string DISH_REFERENCE = DATA_REFERENCE + "dishes/";

    public const string ORDER_REFERENCE = DATA_REFERENCE + "orders/";

    public const string JSON_EXTENSION = ".json";

    private Dictionary _Data = new Dictionary();

    private Dictionary Data
    {
        get { return _Data; }
        set { _Data = value; ValidateData(); }
    }

    public Dictionary Menu
    {
        get { return GetMenus(Data); }
    }

    public Dictionary Dishes
    {
        get { return GetDishes(Data); }
    }

    public Dictionary Orders
    {
        get { return GetOrders(Data); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        UpdateData();
        Ping();
    }

    private void InitNodes()
    {
        Timer = GetNode<Timer>("Timer");
        TimerPing = GetNode<Timer>("TimerPing");
        RequestData = GetNode<HTTPRequest>("RequestData");
        RequestSetMenu = GetNode<HTTPRequest>("RequestSetMenu");
        RequestSendOrder = GetNode<HTTPRequest>("RequestSendOrder");
        RequestCreateDish = GetNode<HTTPRequest>("RequestCreateDish");
        RequestPing = GetNode<HTTPRequest>("RequestPing");
        RequestDeleteOrder = GetNode<HTTPRequest>("RequestDeleteOrder");
        RequestDeleteDish = GetNode<HTTPRequest>("RequestDeleteDish");
    }

    private void ValidateData()
    {
        string menuValidation = DoValidateMenus() ? "ValidateMenus" : "InvalidateMenus";
        string dishValidation = DoValidateDishes() ? "ValidateDishes" : "InvalidateDishes";
        string orderValidation = DoValidateOrders() ? "ValidateOrders" : "InvalidateOrders";
        EmitSignal(menuValidation);
        EmitSignal(dishValidation);
        EmitSignal(orderValidation);
    }

    private void CompareOrders(Dictionary oldData, Dictionary newData)
    {
        Dictionary oldOrders = GetOrders(oldData);
        Dictionary newOrders = GetOrders(newData);

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

    private bool DoValidateMenus()
    {
        if (GetMenus(Data) == null)
        {
            return false;
        }

        return GetMenus(Data).Count > 0;
    }

    private bool DoValidateDishes()
    {
        if (GetDishes(Data) == null)
        {
            return false;
        }

        return GetDishes(Data).Count > 0;
    }

    private bool DoValidateOrders()
    {
        if (GetOrders(Data) == null)
        {
            return false;
        }

        return GetOrders(Data).Count > 0;
    }

    private Dictionary GetMenus(Dictionary data)
    {
        if (!data.Contains("menu"))
        {
            return null;
        }

        return data["menu"] as Dictionary;
    }

    private Dictionary GetDishes(Dictionary data)
    {
        if (!data.Contains("dishes"))
        {
            return null;
        }

        return data["dishes"] as Dictionary;
    }

    private Dictionary GetOrders(Dictionary data)
    {
        if (!data.Contains("orders"))
        {
            return null;
        }

        return data["orders"] as Dictionary;
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

    public Array GetTodayMenu()
    {
        string today = System.DateTime.Now.DayOfWeek.ToString();
        return GetMenu(today);
    }

    public void SetMenu(string day, Array dishes)
    {
        PUTRequest(RequestSetMenu, MENU_REFERENCE + day.ToLower(), dishes);
    }

    public void SendOrder(Array dishes)
    {
        string today = System.DateTime.Now.ToString();

        // It's possible that someone can get a duplicate ticket
        int ticketNumber = (int)GD.RandRange(1, 10000);
        string user = "Ticket-" + ticketNumber.ToString();

        Order order = new Order(user, today, dishes);

        POSTRequest(RequestSendOrder, ORDER_REFERENCE, order.ToMap());
    }

    public void CreateDish(string key, Dish dish)
    {
        if (!key.Empty())
        {
            PUTRequest(RequestCreateDish, DISH_REFERENCE + "/" + key, dish.ToMap());
        }
        else
        {
            POSTRequest(RequestCreateDish, DISH_REFERENCE, dish.ToMap());
        }
    }

    public void DeleteDish(string key)
    {
        if (key.Empty())
        {
            return;
        }

        string reference = DISH_REFERENCE + "/" + key;

        DELETERequest(RequestDeleteDish, reference);
    }

    public void DeleteOrder(string key)
    {
        if (key.Empty())
        {
            return;
        }

        string reference = ORDER_REFERENCE + "/" + key;

        DELETERequest(RequestDeleteOrder, reference);
    }

    public void UpdateData()
    {
        GETRequest(RequestData, DATA_REFERENCE);
    }

    public void Ping()
    {
        GETRequest(RequestPing, DATABASE_REFERENCE);
        TimerPing.Start(2);
    }

    private void GETRequest(HTTPRequest request, string reference)
    {
        DoRequest(HTTPClient.Method.Get, reference, null, request);
    }

    public void PUTRequest(HTTPRequest request, string reference, object data_to_send)
    {
        DoRequest(HTTPClient.Method.Put, reference, data_to_send, request);
    }

    public void POSTRequest(HTTPRequest request, string reference, object data_to_send)
    {
        DoRequest(HTTPClient.Method.Post, reference, data_to_send, request);
    }

    public void PATCHRequest(HTTPRequest request, string reference, object data_to_send)
    {
        DoRequest(HTTPClient.Method.Patch, reference, data_to_send, request);
    }

    public void DELETERequest(HTTPRequest request, string reference)
    {
        DoRequest(HTTPClient.Method.Delete, reference, null, request);
    }

    private void DoRequest(HTTPClient.Method method, string reference, object data_to_send, HTTPRequest request)
    {
        if (reference.Empty())
        {
            return;
        }

        if (request == null)
        {
            return;
        }

        if (!reference.EndsWith(JSON_EXTENSION))
        {
            reference += JSON_EXTENSION;
        }

        Error error;

        switch (method)
        {
            case HTTPClient.Method.Get:
                error = request.Request(reference);
                break;
            default:
                string[] headers = new string[] { "Content-Type: application/json" };
                string query = JSON.Print(data_to_send);

                error = request.Request(reference, headers, false, method, query);
                break;
        }

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
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

        CompareOrders(Data, data);

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

    public void _OnPingRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        EmitSignal("PingSuccess");
        Ping();
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
