using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

[BluaClass(description = "A respawn point. Players will spawn at an appropriate spawn point when they respawn")]
public class LuaRespawn : LuaObject
{

    [MoonSharpHidden] public WTBObject WTBObject;
    [MoonSharpHidden] public List<WTBComponent> components;

    public LuaRespawn(WTBObject _WTBObject)
    {
        this.type = "Respawn";
        if (_WTBObject != null)
        {
            WTBObject = _WTBObject;
            components = _WTBObject.Components;
            Task.luaRespawns.Add(WTBObject.WTBIndex, this);
        }
    }
    
    [BluaProperty(description = "If set to false, players will not respawn at this respawn point")]
    public bool enabled
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Respawn").PropertyByName("Enabled").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Respawn").PropertyByName("Enabled").SetValue(value, false);
        }
    }

}




