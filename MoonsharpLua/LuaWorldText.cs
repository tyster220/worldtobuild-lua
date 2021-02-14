using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

[BluaClass(description = "A 3d text object that can be placed in the world")]
public class LuaWorldText : LuaObject
{
    [MoonSharpHidden] public WTBObject WTBObject;
    [MoonSharpHidden] public List<WTBComponent> components;

    public LuaWorldText(WTBObject _WTBObject)
    {
        this.type = "Text";
        if (_WTBObject != null)
        {
            WTBObject = _WTBObject;
            components = _WTBObject.Components;
            if (Task.luaWorldTexts.ContainsKey(WTBObject.WTBIndex))
            {
                Task.luaWorldTexts.Add(WTBObject.WTBIndex, this);
            }
            else
            {
                Task.luaWorldTexts[WTBObject.WTBIndex] = this;
            }
        }
    }
    
    [BluaProperty(description = "The text on this {object}")]
    public string text
    {
        get
        {
            return (string)WTBObject.Components.ComponentByName("Text").PropertyByName("Text").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Text").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The font of the text")]
    public string font
    {
        get
        {
            return (string)WTBObject.Components.ComponentByName("Text").PropertyByName("Font").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Font").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The size of the text")]
    public float size
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Text").PropertyByName("Size").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Size").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The alignment of the text}")]
    public int alignment
    {
        get
        {
            return (int)WTBObject.Components.ComponentByName("Text").PropertyByName("Alignment").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Alignment").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The color of the text")]
    public Color color
    {
        get
        {
            return (Color)WTBObject.Components.ComponentByName("Text").PropertyByName("Color").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Color").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The transparency of the text")]
    public float transparency
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Text").PropertyByName("Transparency").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Transparency").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Whether or not the text on this {object} has an outline")]
    public bool outline
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Text").PropertyByName("Outline").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Outline").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The outline color of the text. Must have outline set to true")]
    public Color outlineColor
    {
        get
        {
            return (Color)WTBObject.Components.ComponentByName("Text").PropertyByName("Outline Color").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Outline Color").SetValue(value, false);
        }
    }

    [BluaProperty(description = "The outline width of the text. Must have outline set to true")]
    public float outlineWidth
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Text").PropertyByName("Outline Width").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Outline Width").SetValue(value, false);
        }
    }

    [BluaProperty(description = "If set to true, the text will always face the camera")]
    public bool faceCamera
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Text").PropertyByName("Face Camera").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Text").PropertyByName("Face Camera").SetValue(value, false);
        }
    }
}