using Godot;
using System.Linq;
using System.Text;

using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class Firebase : Node
{

    private UserData UserData;

    public const string DATABASE_REFERENCE = "https://kiawa-service-default-rtdb.firebaseio.com/data";

    public const string JSON_EXTENSION = ".json";

    public HTTPRequest DataRequest;

    public HTTPRequest SetMenuRequest;

    public HTTPRequest SendOrderRequest;

    [Signal]
    delegate void UpdatedData();

    private Dictionary Data = new Dictionary();

    public Dictionary Menu
    {
        get { return GetMenus(); }
    }

    public Array Dishes
    {
        get { return GetDishes(); }
    }

    public Dictionary Orders
    {
        get { return GetOrders(); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    private void InitNodes()
    {
        UserData = GetNode<UserData>("/root/UserData");
        DataRequest = GetNode<HTTPRequest>("DataRequest");
        SetMenuRequest = GetNode<HTTPRequest>("SetMenuRequest");
        SendOrderRequest = GetNode<HTTPRequest>("SendOrderRequest");
    }

    private Dictionary GetMenus()
    {
        if (Data.Contains("dishes"))
        {
            return Data["menu"] as Dictionary;
        }
        else
        {
            return null;
        }
    }

    private Array GetDishes()
    {
        if (Data.Contains("dishes"))
        {
            return Data["dishes"] as Array;
        }
        else
        {
            return null;
        }
    }

    private Dictionary GetOrders()
    {
        if (Data.Contains("orders"))
        {
            return Data["orders"] as Dictionary;
        }
        else
        {
            return null;
        }
    }

    public Array GetMenu(string day)
    {
        Array result = new Array();

        if (Menu.Contains(day.ToLower()))
        {
            result = Menu[day.ToLower()] as Array;
        }

        return result;
    }

    public Dictionary GetDish(int index)
    {
        return Dishes[index] as Dictionary;
    }

    public Dictionary GetOrder(string key)
    {
        Dictionary result = new Dictionary();

        if (Menu.Contains(key))
        {
            result = Menu[key] as Dictionary;
        }

        return result;
    }

    public Array GetTodayMenu()
    {
        string today = System.DateTime.Now.DayOfWeek.ToString();
        return GetMenu(today);
    }

    public void SetMenu(string day, Array dishes)
    {
        string reference = Firebase.DATABASE_REFERENCE +
                        "/menu/" +
                        day.ToLower();

        PUTRequest(SetMenuRequest, reference, dishes);
    }

    public void SendOrder(Array dishes)
    {
        Dictionary data = new Dictionary() {
            {"user", UserData.Username},
            {"date", System.DateTime.Now.ToString()},
            {"dishes", dishes}
        };

        string reference = Firebase.DATABASE_REFERENCE +
                        "/orders";

        POSTRequest(SendOrderRequest, reference, data);
    }

    public void UpdateData()
    {
        GETRequest(DataRequest, DATABASE_REFERENCE);
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

    private void DoRequest(HTTPClient.Method method, string reference, object data_to_send, HTTPRequest request)
    {
        bool new_request = false;

        if (reference.Empty())
        {
            return;
        }

        if (request == null)
        {
            request = new HTTPRequest();
            AddChild(request);
            new_request = true;
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

        if (new_request)
        {
            RemoveChild(request);
            request.QueueFree();
        }
    }

    public void _OnDataRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object _result = json.Result;

        if (_result != null)
        {
            if (_result is Dictionary)
            {
                Data = _result as Dictionary;
            }

            EmitSignal("UpdatedData");
        }
    }

    public void _OnSetMenuRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

    public void _OnSendOrderRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

}
