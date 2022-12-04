using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public class ResourceDirector : Node
{

    public ResourcePreloader ResourcePreloader;

    public Preferences Preferences;

    public AudioStreamPlayer AudioStreamMenu;

    public AudioStreamPlayer AudioStreamGlobal;

    private Godot.Collections.Dictionary<string, Godot.Resource> _ResourceDictionary = new Godot.Collections.Dictionary<string, Godot.Resource>();

    [Export]
    public Godot.Collections.Dictionary<string, Godot.Resource> ResourceDictionary
    {
        get { return _ResourceDictionary; }
        set { _ResourceDictionary = value; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        AddResources(ResourceDictionary);
    }

    private void InitNodes()
    {
        ResourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        Preferences = GetNode<Preferences>("/root/Preferences");
        AudioStreamMenu = GetNode<AudioStreamPlayer>("/root/AudioStreamMenu");
        AudioStreamGlobal = GetNode<AudioStreamPlayer>("/root/AudioStreamGlobal");
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
