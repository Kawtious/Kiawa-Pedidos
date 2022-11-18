using Godot;
using Godot.Collections;
using System.Text;
using Dictionary = Godot.Collections.Dictionary;

public class Firebase : Node
{

    public HTTPRequest POSTRequest;

    public HTTPRequest PUTRequest;

    public HTTPRequest GETRequest;

    public HTTPRequest UpdateDishesRequest;

    public HTTPRequest MondayMenuRequest;

    public HTTPRequest TuedayMenuRequest;

    public HTTPRequest WeddayMenuRequest;

    public HTTPRequest ThudayMenuRequest;

    public HTTPRequest FridayMenuRequest;

    [Signal]
    delegate void UpdatedDishList();

    [Signal]
    delegate void UpdatedMondayMenu();

    [Signal]
    delegate void UpdatedTuesdayMenu();

    [Signal]
    delegate void UpdatedWednesdayMenu();

    [Signal]
    delegate void UpdatedThursdayMenu();

    [Signal]
    delegate void UpdatedFridayMenu();

    public readonly string[] Headers = new string[] { "Content-Type: application/json" };

    public const string JSON_EXTENSION = ".json";

    public const string DATABASE_REFERENCE = "https://kiawa-service-default-rtdb.firebaseio.com/data/";

    public const string DISH_REFERENCE = DATABASE_REFERENCE + "dishes/";

    public const string MENU_REFERENCE = DATABASE_REFERENCE + "menu/";

    public const string
            MON_REFERENCE = "monday/",
            TUE_REFERENCE = "tuesday/",
            WED_REFERENCE = "wednesday/",
            THU_REFERENCE = "thursday/",
            FRI_REFERENCE = "friday/";

    public Array Dishes = new Array();

    public Array MondayMenu = new Array(),
            TuesdayMenu = new Array(),
            WednesdayMenu = new Array(),
            ThursdayMenu = new Array(),
            FridayMenu = new Array();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        //MakeGETRequest();
        //GD.Print(TestMenu[1]);
    }

    private void InitNodes()
    {
        POSTRequest = GetNode<HTTPRequest>("POSTRequest");
        PUTRequest = GetNode<HTTPRequest>("PUTRequest");
        GETRequest = GetNode<HTTPRequest>("GETRequest");
        UpdateDishesRequest = GetNode<HTTPRequest>("UpdateDishesRequest");

        MondayMenuRequest = GetNode<HTTPRequest>("MondayMenuRequest");
        TuedayMenuRequest = GetNode<HTTPRequest>("TuedayMenuRequest");
        WeddayMenuRequest = GetNode<HTTPRequest>("WeddayMenuRequest");
        ThudayMenuRequest = GetNode<HTTPRequest>("ThudayMenuRequest");
        FridayMenuRequest = GetNode<HTTPRequest>("FridayMenuRequest");
    }

    public void UpdateDishes()
    {
        Error error = UpdateDishesRequest.Request(DISH_REFERENCE + JSON_EXTENSION);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
    }

    public void UpdateMenu(HTTPRequest request, string day)
    {
        Error error = request.Request(MENU_REFERENCE + day + JSON_EXTENSION);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
    }

    private void MakeGETRequest(string reference)
    {
        Error error = GETRequest.Request(reference);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
    }

    public void MakePOSTRequest(string reference, object data_to_send)
    {
        string query = JSON.Print(data_to_send);

        Error error = POSTRequest.Request(reference, Headers, false, HTTPClient.Method.Post, query);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
    }

    public void MakePUTRequest(string reference, object data_to_send)
    {
        string query = JSON.Print(data_to_send);

        Error error = PUTRequest.Request(reference, Headers, false, HTTPClient.Method.Put, query);

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
        }
    }

    public void _OnGETRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object Result = json.Result;

        GD.Print(Result);

        Dictionary Results = Result as Dictionary;
    }

    public void _OnPUTRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
    }

    public void _OnPOSTRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
    }

    public void _OnUpdateDishesRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object Result = json.Result;

        GD.Print(Result);

        Array Results = Result as Array;
        GD.Print(Results);

        if (Results != null)
        {
            Dishes = Results;
            EmitSignal("UpdatedDishList");
        }
    }

    public void _OnMondayMenuRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object Result = json.Result;

        GD.Print(Result);

        Array Results = Result as Array;
        GD.Print(Results);

        MondayMenu = Results;
        EmitSignal("UpdatedMondayMenu");
    }

    public void _OnTuedayMenuRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object Result = json.Result;

        GD.Print(Result);

        Array Results = Result as Array;
        GD.Print(Results);

        TuesdayMenu = Results;
        EmitSignal("UpdatedTuesdayMenu");
    }

    public void _OnWeddayMenuRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object Result = json.Result;

        GD.Print(Result);

        Array Results = Result as Array;
        GD.Print(Results);

        WednesdayMenu = Results;
        EmitSignal("UpdatedWednesdayMenu");
    }

    public void _OnThudayMenuRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object Result = json.Result;

        GD.Print(Result);

        Array Results = Result as Array;
        GD.Print(Results);

        ThursdayMenu = Results;
        EmitSignal("UpdatedThursdayMenu");
    }

    public void _OnFridayMenuRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object Result = json.Result;

        GD.Print(Result);

        Array Results = Result as Array;
        GD.Print(Results);

        FridayMenu = Results;
        EmitSignal("UpdatedFridayMenu");
    }

}
