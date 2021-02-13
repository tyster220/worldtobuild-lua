using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using UnityEngine.UI;

[BluaClass(description = "A UI window that can hold other UI objects")]
public class LuaUIWindow : LuaUIObject
{
    [MoonSharpHidden] public Text titleText;
    [MoonSharpHidden] public Image titleBarBackground;
    [MoonSharpHidden] public Image background;

    public LuaUIWindow(GameObject _uiObject) : base(_uiObject)
    {
        type = "UIWindow";
        
        UIObject = _uiObject;

        titleText = UIObject.transform.Find("TitleBar").Find("Title").GetComponent<Text>();
        background = UIObject.GetComponent<Image>();
        titleBarBackground = UIObject.transform.Find("TitleBar").GetComponent<Image>();

        
    }
    // GameObject LuaUIButton(Vector2 _position, Vector2 _size, string _text = " ", GameObject _parent = null, int _fontSize = 11, Color? _buttonColorN = null, Color? _textColorN = null, TextAnchor _alignment = TextAnchor.MiddleLeft)

    [BluaProperty(description = "The color of the {object}")]
    public Color color
    {
        get
        {
            return background.color;
        }
        set
        {
            background.color = value;
        }
    }

    [BluaProperty(description = "The title that appears at the top of the {object}")]
    public string title
    {
        get
        {
            return titleText.text;
        }
        set
        {
            titleText.text = value;
        }
    }

    [BluaProperty(description = "The color of the title bar")]
    public Color titleBarColor
    {
        get
        {
            return titleText.color;
        }
        set
        {
            titleText.color = value;
        }
    }

    [BluaProperty(description = "The font size of the title")]
    public int titleSize
    {
        get
        {
            return titleText.fontSize;
        }
        set
        {
            titleText.fontSize = value;
        }
    }
}