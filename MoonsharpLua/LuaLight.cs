using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

[BluaClass(description = "An object that acts a light source in the world")]
public class LuaLight: LuaObject
{

    [MoonSharpHidden] public WTBObject WTBObject;
    [MoonSharpHidden] public List<WTBComponent> components;

    public LuaLight(WTBObject _WTBObject)
    {
        this.type = "Light";
        if (_WTBObject != null)
        {
            WTBObject = _WTBObject;
            components = _WTBObject.Components;
            Task.luaLights.Add(WTBObject.WTBIndex, this);
        }
    }
    
    [BluaProperty(description = "The color of this {object}")]
    public Color color
    {
        get
        {
            Color color = new Color(-1, -1, -1, -1);
            color = (Color)WTBObject.Components.ComponentByName("Light").PropertyByName("Color").GetValue();
            return color;
       }
        set
        {
            // mask vec4 to color
            //LightComponent renderComp = (LightComponent)WTBObject.Components.ComponentByName("Light");
            WTBObject.Components.ComponentByName("Light").PropertyByName("Color").SetValue((Color)value);
            //WTBObject.Components.ComponentByName("Renderer").PropertyByName("Color").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The type of light coming from this {object}")]
    public int lightType
    {
        get
        {
            return (int)WTBObject.Components.ComponentByName("Light").PropertyByName("Type").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Light").PropertyByName("Type").SetValue((int)value, false);
        }
    }

    [BluaProperty(description = "The range of the light")]
    public float range
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Light").PropertyByName("Range").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Light").PropertyByName("Range").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The angle of the light")]
    public float angle
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Light").PropertyByName("SpotAngle").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Light").PropertyByName("SpotAngle").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The brightness of the light")]
    public float brightness
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Light").PropertyByName("Brightness").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Light").PropertyByName("Brightness").SetValue(value, false);
        }
    }

    [BluaProperty(description = "If set to true, the light will create shadows for anything the light hits")]
    public bool shadows
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Light").PropertyByName("Shadows").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Light").PropertyByName("Shadows").SetValue(value, false);
        }
    }
}