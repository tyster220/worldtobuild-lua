using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using System.IO;
using System.Text;


[BluaClass]
public class LuaGame
{
    public LuaGame()
    {
    }

    [BluaProperty(description = "Get or set the gravity multiplier")]
    public float gravity
    {
        get
        {
            return Task.gameSettingsController.settings.GravityMult;
        }
        set
        {
            Task.gameSettingsController.settings.GravityMult = value;
        }
    }

    [BluaProperty(description = "Get or set the maximum number of jumps")]
    public int maxJumps
    {
        get
        {
            return Task.gameSettingsController.settings.NumberOfJumps;
        }
        set
        {
            Task.gameSettingsController.settings.NumberOfJumps = value;
        }
    }

    [BluaProperty(description = "Get or set the jump power")]
    public float jumpPower
    {
        get
        {
            return Task.gameSettingsController.settings.JumpPower;
        }
        set
        {
            Task.gameSettingsController.settings.JumpPower = value;
        }
    }

    [BluaProperty(description = "Returns the ID of the current world")]
    public int worldID
    {
        get
        {
            return Launcher.WORLDID;
        }
    }

    [BluaProperty(description = "Returns the name of the current world")]
    public string worldName
    {
        get
        {
            return Launcher.WORLD_NAME;
        }
    }

    [BluaProperty(description = "Returns the owner user ID of the current world")]
    public int ownerID
    {
        get
        {
            return Launcher.WORLD_OWNER;
        }
    }

    [BluaProperty(description = "Returns the maximum player capacity for the server")]
    public int maxPlayers
    {
        get
        {
            return Launcher.WORLD_MAXPLAYERS;
        }
    }

    [BluaProperty(description = "Returns true if the player was started with test mode")]
    public bool isTestMode
    {
        get
        {
            return Launcher.MODE == Launcher.Mode.Test;
        }
    }
}