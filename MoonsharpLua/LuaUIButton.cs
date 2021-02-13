using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using UnityEngine.UI;

[BluaClass(description = "A UI button that can be clicked to do things")]
public class LuaUIButton : LuaUIObject
{
    [MoonSharpHidden] public Text buttonText;
    [MoonSharpHidden] public Image buttonImage;

    public LuaUIButton(GameObject _uiObject) : base(_uiObject)
    {
        type = "UIButton";
        
        UIObject = _uiObject;

        buttonText = UIObject.transform.Find("Text").GetComponent<Text>();
        buttonImage = UIObject.GetComponent<Image>();

        
    }
    // GameObject LuaUIButton(Vector2 _position, Vector2 _size, string _text = " ", GameObject _parent = null, int _fontSize = 11, Color? _buttonColorN = null, Color? _textColorN = null, TextAnchor _alignment = TextAnchor.MiddleLeft)

    [BluaProperty(description = "The color of the {object}")]
    public Color color
    {
        get
        {
            return buttonImage.color;
        }
        set
        {
            buttonImage.color = value;
        }
    }

    [BluaProperty(description = "The text on the {object}")]
    public string text
    {
        get
        {
            return buttonText.text;
        }
        set
        {
            buttonText.text = value;
        }
    }

    [BluaProperty(description = "The color of the text")]
    public Color textColor
    {
        get
        {
            return buttonText.color;
        }
        set
        {
            buttonText.color = value;
        }
    }

    [BluaProperty(description = "The size of the text")]
    public int textSize
    {
        get
        {
            return buttonText.fontSize;
        }
        set
        {
            buttonText.fontSize = value;
        }
    }


    [BluaProperty(description = "The alignment of the text")]
    public string textAlignment
    {
        get
        {
            string alignmentString = "left";

            switch(buttonText.alignment)
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
                    buttonText.alignment = TextAnchor.LowerCenter;
                    break;
                case "BottomLeft":
                    buttonText.alignment = TextAnchor.LowerLeft;
                    break;
                case "BottomRight":
                    buttonText.alignment = TextAnchor.LowerRight;
                    break;
                case "MiddleCenter":
                    buttonText.alignment = TextAnchor.MiddleCenter;
                    break;
                case "MiddleLeft":
                    buttonText.alignment = TextAnchor.MiddleLeft;
                    break;
                case "MiddleRight":
                    buttonText.alignment = TextAnchor.MiddleRight;
                    break;
                case "TopCenter":
                    buttonText.alignment = TextAnchor.UpperCenter;
                    break;
                case "TopLeft":
                    buttonText.alignment = TextAnchor.UpperLeft;
                    break;
                case "TopRight":
                    buttonText.alignment = TextAnchor.UpperRight;
                    break;

                // aliases for convenience
                case "Center":
                    buttonText.alignment = TextAnchor.MiddleCenter;
                    break;
                case "Left":
                    buttonText.alignment = TextAnchor.MiddleLeft;
                    break;
                case "Right":
                    buttonText.alignment = TextAnchor.MiddleRight;
                    break;
                default:
                    break;
            }
        }
    }

    [BluaProperty(description = "If false, this {object} will be disabled and do nothing when clicked")]
    bool clickable
    {
        get
        {
            return UIObject.GetComponent<Button>().interactable; 
        }
        set
        {
            UIObject.GetComponent<Button>().interactable = value;
        }
    }
}