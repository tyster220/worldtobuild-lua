using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using UnityEngine.UI;

[BluaClass(description = "A UI panel that can contain other UI objects inside of it")]
public class LuaUIPanel : LuaUIObject
{
    [MoonSharpHidden] public Image panelImage;

    public LuaUIPanel(GameObject _uiObject) : base(_uiObject)
    {
        type = "UIPanel";
        
        UIObject = _uiObject;

        panelImage = UIObject.GetComponent<Image>();

        
    }

    [BluaProperty(description = "The color of the {object}")]
    public Color color
    {
        get
        {
            return panelImage.color;
        }
        set
        {
            panelImage.color = value;
        }
    }
}