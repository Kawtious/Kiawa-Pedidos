using Godot;
using System.Text;

using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class Firebase : Node
{

    private UserData UserData;

    public const string DATABASE_REFERENCE = "https://kiawa-service-default-rtdb.firebaseio.com/data";

    public const string JSON_EXTENSION = ".json";

    public HTTPRequest SetMenuRequest;

    public HTTPRequest SendOrderRequest;

    public HTTPRequest DataRequest;

    [Signal]
    delegate void UpdatedData();

    private Dictionary Data = new Dictionary();

    public Dictionary Menu
    {
        get { return Data["menu"] as Dictionary; }
    }

    public Array Dishes
    {
        get { return Data["dishes"] as Array; }
    }

    public Dictionary Orders
    {
        get { return Data["orders"] as Dictionary; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    private void InitNodes()
    {
        UserData = GetNode<UserData>("/root/UserData");
        SetMenuRequest = GetNode<HTTPRequest>("SetMenuRequest");
        SendOrderRequest = GetNode<HTTPRequest>("SendOrderRequest");
        DataRequest = GetNode<HTTPRequest>("DataRequest");
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
                        day.ToLower() +
                        Firebase.JSON_EXTENSION;

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
                        "/orders" +
                        Firebase.JSON_EXTENSION;

        POSTRequest(SendOrderRequest, reference, data);
    }

    public void UpdateData()
    {
        APIRequest(DataRequest, DATABASE_REFERENCE);
    }

    private void APIRequest(HTTPRequest node, string target)
    {
        Error error = node.Request(target + JSON_EXTENSION);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
    }

    public void PUTRequest(HTTPRequest request, string reference, object data_to_send)
    {
        string[] headers = new string[] { "Content-Type: application/json" };
        string query = JSON.Print(data_to_send);

        Error error = request.Request(reference, headers, false, HTTPClient.Method.Put, query);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
    }

    public void POSTRequest(HTTPRequest request, string reference, object data_to_send)
    {
        string[] headers = new string[] { "Content-Type: application/json" };
        string query = JSON.Print(data_to_send);

        Error error = request.Request(reference, headers, false, HTTPClient.Method.Post, query);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
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

}
