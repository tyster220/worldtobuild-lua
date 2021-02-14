using System;

[BluaClass]
public class LuaGame
{
	public LuaGame()
	{
	}

    [BluaProperty]
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

    [BluaProperty]
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

    [BluaProperty]
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

    [BluaProperty]
    public int worldID
    {
        get
        {
            return Launcher.WORLDID;
        }
    }

    [BluaProperty]
    public string worldName
    {
        get
        {
            return Launcher.WORLD_NAME;
        }
    }

    [BluaProperty]
    public int ownerID
    {
        get
        {
            return Launcher.WORLD_OWNER;
        }
    }

    [BluaProperty]
    public int maxPlayers
    {
        get
        {
            return Launcher.WORLD_MAXPLAYERS;
        }
    }

    [BluaProperty]
    public bool isTestMode
    {
        get
        {
            return Launcher.MODE == Launcher.Mode.Test;
        }
    }
}
