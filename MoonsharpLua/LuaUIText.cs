using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using UnityEngine.UI;

[BluaClass(description = "A UI text that can contain words and letters")]
public class LuaUIText : LuaUIObject
{
    [MoonSharpHidden] public Text textComp;

    public LuaUIText(GameObject _uiObject) : base(_uiObject)
    {
        type = "UIText";
        
        UIObject = _uiObject;

        textComp = UIObject.GetComponent<Text>();

       
    }
    
    [BluaProperty(description = "The actual text of the {object}")]
    public string text
    {
        get
        {
            return textComp.text;
        }
        set
        {
            textComp.text = value;
        }
    }

    [BluaProperty(description = "The color of the text")]
    public Color textColor
    {
        get
        {
            return textComp.color;
        }
        set
        {
            textComp.color = value;
        }
    }

    [BluaProperty(description = "The size of the text")]
    public int textSize
    {
        get
        {
            return textComp.fontSize;
        }
        set
        {
            textComp.fontSize = value;
        }
    }

    [BluaProperty(description = "The alignment of the text")]
    public string textAlignment
    {
        get
        {
            string alignmentString = "left";

            switch(textComp.alignment)
            {
                case TextAnchor.LowerCenter:
                    alignmentString = "BottomCenter";
                    break;
                case TextAnchor.LowerLeft:
                    alignmentString = "BottomLeft";
                    break;
                case TextAnchor.LowerRight:
                    alignmentString = "BottomRight";
                    break;
                case TextAnchor.MiddleCenter:
                    alignmentString = "MiddleCenter";
                    break;
                case TextAnchor.MiddleLeft:
                    alignmentString = "MiddleLeft";
                    break;
                case TextAnchor.MiddleRight:
                    alignmentString = "MiddleRight";
                    break;
                case TextAnchor.UpperCenter:
                    alignmentString = "TopCenter";
                    break;
                case TextAnchor.UpperLeft:
                    alignmentString = "TopLeft";
                    break;
                case TextAnchor.UpperRight:
                    alignmentString = "TopRight";
                    break;
            }

            return alignmentString;
        }
        set
        {
            switch (value)
            {
                case "BottomCenter":
                    textComp.alignment = TextAnchor.LowerCenter;
                    break;
                case "BottomLeft":
                    textComp.alignment = TextAnchor.LowerLeft;
                    break;
                case "BottomRight":
                    textComp.alignment = TextAnchor.LowerRight;
                    break;
                case "MiddleCenter":
                    textComp.alignment = TextAnchor.MiddleCenter;
                    break;
                case "MiddleLeft":
                    textComp.alignment = TextAnchor.MiddleLeft;
                    break;
                case "MiddleRight":
                    textComp.alignment = TextAnchor.MiddleRight;
                    break;
                case "TopCenter":
                    textComp.alignment = TextAnchor.UpperCenter;
                    break;
                case "TopLeft":
                    textComp.alignment = TextAnchor.UpperLeft;
                    break;
                case "TopRight":
                    textComp.alignment = TextAnchor.UpperRight;
                    break;

                // aliases for convenience
                case "Center":
                    textComp.alignment = TextAnchor.MiddleCenter;
                    break;
                case "Left":
                    textComp.alignment = TextAnchor.MiddleLeft;
                    break;
                case "Right":
                    textComp.alignment = TextAnchor.MiddleRight;
                    break;
                default:
                    break;
            }
        }
    }
}