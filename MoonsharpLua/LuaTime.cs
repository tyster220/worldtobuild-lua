using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

[BluaClass]
public class LuaTime
{


    public LuaTime()
    {

    }

    [BluaProperty]
    public float delta
    {
        get
        {
            return Time.deltaTime;
        }
    }

    [BluaProperty]
    public float fixedDelta
    {
        get
        {
            return Time.fixedDeltaTime;
        }
    }

    [BluaProperty]
    public float time
    {
        get
        {
            return Time.time;
        }
    }

    [BluaProperty]
    public int day
    {
        get
        {
            return System.DateTime.Now.Day;
        }
    }

    [BluaProperty]
    public int month
    {
        get
        {
            return System.DateTime.Now.Month;
        }
    }

    [BluaProperty]
    public int year
    {
        get
        {
            return System.DateTime.Now.Year;
        }
    }

    [BluaProperty]
    public int millisecond
    {
        get
        {
            return System.DateTime.Now.Millisecond;
        }
    }

    [BluaProperty]
    public int second
    {
        get
        {
            return System.DateTime.Now.Second;
        }
    }

    [BluaProperty]
    public int minute
    {
        get
        {
            return System.DateTime.Now.Minute;
        }
    }

    [BluaProperty]
    public int hour
    {
        get
        {
            return System.DateTime.Now.Hour;
        }
    }
}