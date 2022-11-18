using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public class ResourceDirector : Node
{

    private ResourcePreloader ResourcePreloader;

    private Preferences _Preferences;

    private AudioStreamPlayer _MenuAudioStream;

    private AudioStreamPlayer _GlobalAudioStream;

    private Godot.Collections.Dictionary<string, Godot.Resource> _ResourceDictionary = new Godot.Collections.Dictionary<string, Godot.Resource>();

    public Preferences Preferences
    {
        get { return GetPreferences(); }
        set { _Preferences = value; }
    }

    public AudioStreamPlayer MenuAudioStream
    {
        get { return GetMenuAudioStream(); }
        set { _MenuAudioStream = value; }
    }

    public AudioStreamPlayer GlobalAudioStream
    {
        get { return GetGlobalAudioStream(); }
        set { _GlobalAudioStream = value; }
    }

    [Export]
    public Godot.Collections.Dictionary<string, Godot.Resource> ResourceDictionary
    {
        get { return GetResourceDictionary(); }
        set { _ResourceDictionary = value; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitResources();
        AddResources(ResourceDictionary);
    }

    private void InitResources()
    {
        ResourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        Preferences = GetNode<Preferences>("/root/Preferences");
        MenuAudioStream = GetNode<AudioStreamPlayer>("/root/MenuAudioStream");
        GlobalAudioStream = GetNode<AudioStreamPlayer>("/root/GlobalAudioStream");
    }

    public Preferences GetPreferences()
    {
        if (_Preferences == null)
        {
            _Preferences = GetNode<Preferences>("/root/Preferences");
        }

        return _Preferences;
    }

    public AudioStreamPlayer GetMenuAudioStream()
    {
        if (_MenuAudioStream == null)
        {
            _MenuAudioStream = GetNode<AudioStreamPlayer>("/root/MenuAudioStream");
        }

        return _MenuAudioStream;
    }

    public AudioStreamPlayer GetGlobalAudioStream()
    {
        if (_GlobalAudioStream == null)
        {
            _GlobalAudioStream = GetNode<AudioStreamPlayer>("/root/GlobalAudioStream");
        }

        return _GlobalAudioStream;
    }

    public Godot.Collections.Dictionary<string, Godot.Resource> GetResourceDictionary()
    {
        if (_ResourceDictionary == null)
        {
            _ResourceDictionary = new Godot.Collections.Dictionary<string, Godot.Resource>();
        }

        return _ResourceDictionary;
    }

    public void AddResources(Godot.Collections.Dictionary<string, Godot.Resource> resources)
    {
        foreach (KeyValuePair<string, Godot.Resource> item in resources)
        {
            ResourcePreloader.AddResource(item.Key, item.Value);
        }
    }

    public void AddResource(string name, Godot.Resource resource)
    {
        ResourcePreloader.AddResource(name, resource);
    }

    public Godot.Resource GetResource(string name)
    {
        return ResourcePreloader.GetResource(name);
    }

    public string[] GetResourceList()
    {
        return ResourcePreloader.GetResourceList();
    }

    public bool HasResource(string name)
    {
        return ResourcePreloader.HasResource(name);
    }

    public void RemoveResource(string name)
    {
        ResourcePreloader.RemoveResource(name);
    }

    public void RenameResource(string name, string newname)
    {
        ResourcePreloader.RenameResource(name, newname);
    }

}
