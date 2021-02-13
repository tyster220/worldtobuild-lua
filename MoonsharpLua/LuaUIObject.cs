using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using UnityEngine.UI;

[BluaClass(description = "An object that exists on the UI")]
public class LuaUIObject: LuaObject
{

    [MoonSharpHidden] public GameObject UIObject;

    public LuaUIObject(GameObject _uiObject)
    {
        this.type = "UIPart";
        if (_uiObject != null)
        {
            UIObject = _uiObject;
            Task.luaUIObjects.Add(_uiObject, this);
        }
    }

    [BluaProperty(description = "Controls the order of the {object} so you can choose which {object} is on top")]
    public int index
    {
        get
        {
            return UIObject.transform.GetSiblingIndex();
        }
        set
        {
            UIObject.transform.SetSiblingIndex(value);
        }
    }

    [BluaMethod(description = "Closes and removes the {object}", scriptSide = ScriptSide.Client)]
    public void Close()
    {
        Task.luaUIObjects.Remove(UIObject);
        GameObject.Destroy(UIObject);
    }

    [BluaProperty(description = "The parent {object} of this {object}")]
    public LuaUIObject parent
    {
        get
        {
            // don't let them leave the LuaUIArea, if the parent they're trying to get IS the LuaUIArea, don't even let them get that, they stay BELOW the LuaUIArea GameObject
            if (UIObject.transform.parent.name != "LuaUIArea")
            {
                return Task.GetOrMakeLuaUIObject(UIObject.transform.parent.gameObject);
            }
            return null;
        }
        set
        {
            UIObject.transform.parent = value.UIObject.transform;
        }
    }

    [BluaProperty(description = "The name of this {object}")]
    public string name
    {
        get
        {
            return UIObject.name;
        }
        set
        {
            UIObject.name = value;
        }
    }

    [BluaProperty(description = "The position of this {object}")]
    public Vector2? position
    {
        get
        {
            if (UIObject == null)
            {
                return null;
            }
            return UIObject.GetComponent<RectTransform>().position;
        }
        set
        {
        if (UIObject != null)
        {
                Vector2 reasonableVector = (Vector2)value;
                reasonableVector.y = -reasonableVector.y;

                if (parent != null)
                {
                    UIObject.GetComponent<RectTransform>().localPosition = reasonableVector;
                }
                else
                {
                    UIObject.GetComponent<RectTransform>().position = reasonableVector;
                }
            }
        }
    }

    [BluaProperty(description = "The rotation of this {object}")]
    public float rotation
    {
        get
        {
            return UIObject.GetComponent<RectTransform>().eulerAngles.z;
        }
        set
        {
            var tempAngles = UIObject.GetComponent<RectTransform>().eulerAngles;
            tempAngles.z = value;
            UIObject.GetComponent<RectTransform>().eulerAngles = tempAngles;
        }
    }

    [BluaProperty(description = "The name of this {object}")]
    public Vector2 size
    {
        get
        {
            return UIObject.GetComponent<RectTransform>().rect.size;
        }
        set
        {
            UIObject.GetComponent<RectTransform>().sizeDelta = value;
            /*Vector2 tempPos = UIObject.GetComponent<RectTransform>().rect.position;
            Vector2 newSize = value;

            UIObject.GetComponent<RectTransform>().rect.Set(tempPos.x, tempPos.y, newSize.x, newSize.y);*/
        }
    }

    [BluaProperty(description = "If set to false, this {object} will not be visible")]
    public bool enabled
    {
        get
        {
            return UIObject.activeInHierarchy;
        }
        set
        {
            UIObject.SetActive(value);
        }
    }

    [BluaMethod(description = "Removes the {object}", scriptSide = ScriptSide.Client)]
    public void Remove()
    {
        GameObject.Destroy(UIObject);
    }
}