using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

[BluaClass(description = "A basic object in the world")]
public class LuaObject
{

    public LuaObject()
    {
    }

    [BluaProperty(description = "The type of the {object} as a string")]
    public string type
    {
        get
        {
            return _type;
        }
        set
        {
            _type = value;
        }
    }
    string _type = "LuaObject";


    [BluaProperty(description = "A table of information stored in the {object}")]
    public DynValue table
    {
        get
        {
            return _table;
        }
        set
        {
            _table = value;
        }
    }
    [MoonSharpHidden]
    DynValue _table = DynValue.NewTable(new Table(null));
}