using Godot;
using System;

public class RESTRequest : HTTPRequest
{

    private bool Requesting = false;

    public const string JSON_EXTENSION = ".json";

    public new void Get(string reference)
    {
        DoRequest(HTTPClient.Method.Get, reference, null);
    }

    public void Put(string reference, object data_to_send)
    {
        DoRequest(HTTPClient.Method.Put, reference, data_to_send);
    }

    public void Post(string reference, object data_to_send)
    {
        DoRequest(HTTPClient.Method.Post, reference, data_to_send);
    }

    public void Patch(string reference, object data_to_send)
    {
        DoRequest(HTTPClient.Method.Patch, reference, data_to_send);
    }

    public void Delete(string reference)
    {
        DoRequest(HTTPClient.Method.Delete, reference, null);
    }

    private void DoRequest(HTTPClient.Method method, string reference, object data_to_send)
    {
        // if (Requesting)
        // {
        //     return;
        // }

        if (reference.Empty())
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
                error = Request(reference);
                break;
            default:
                string[] headers = new string[] { "Content-Type: application/json" };
                string query = JSON.Print(data_to_send);

                error = Request(reference, headers, false, method, query);
                break;
        }

        if (error != Error.Ok)
        {
            GD.PrintErr("Request failed.");
            return;
        }

        Requesting = true;
    }

    public void _OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        Requesting = false;
    }
}
