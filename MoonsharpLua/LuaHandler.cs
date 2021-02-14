using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using System;
using MoonSharp.Interpreter.CoreLib;

public static class Vector3Extended
{
    [BluaMethod(description = "Converts a world space vector to screen space.", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(Vector3)
        })]
    public static Vector2 WorldToScreen(this Vector3 _vec)
    {
        Vector2 reasonableVector = Camera.main.WorldToScreenPoint(_vec);
        reasonableVector.y = -reasonableVector.y;
        return reasonableVector;
    }

    [BluaMethod(description = "Converts a screen space vector to world space.", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(Vector3)
        })]
    public static Vector3 ScreenToWorld(this Vector3 _vec)
    {
        Vector2 reasonableVector = _vec;
        _vec.y = -_vec.y;

        return Camera.main.ScreenToWorldPoint(_vec);
    }

    [BluaMethod(description = "Converts a screen space vector to world space.", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(Vector3)
        })]
    public static Vector3 ScreenToWorld(this Vector2 _vec)
    {
        Vector2 reasonableVector = _vec;
        _vec.y = -_vec.y;

        return Camera.main.ScreenToWorldPoint(_vec);
    }
}

public static class LuaGlobalEnvironment
{
    static bool hasBeenRegistered = false;

    // really doesn't matter if it runs multiple times :p
    public static void RegisterUserData()
    {
        if (hasBeenRegistered)
        {
            return;
        }

        UserData.RegisterAssembly();
        //UserData.RegisterType<Transform>();

        UserData.RegisterType<LuaHitData>();


        UserData.RegisterType<LuaObject>();

        UserData.RegisterType<LuaWTBObject>();
        UserData.RegisterType<LuaPlayer>();

        UserData.RegisterType<LuaTime>();
        UserData.RegisterType<LuaFile>();
        UserData.RegisterType<LuaGame>();

        UserData.RegisterType<LuaLight>();
        UserData.RegisterType<LuaParticleSystem>();
        UserData.RegisterType<LuaRespawn>();
        UserData.RegisterType<LuaWorldText>();

        UserData.RegisterType<LuaUIObject>();
        UserData.RegisterType<LuaUIButton>();
        UserData.RegisterType<LuaUIText>();
        UserData.RegisterType<LuaUIWindow>();
        UserData.RegisterType<LuaUIPanel>();

        UserData.RegisterType<string>();

        UserData.RegisterType<Vector2>();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Vector4>();
        UserData.RegisterType<Color>();

        UserData.RegisterType<AccessibleLuaScript>();

        hasBeenRegistered = true;
    }




    public static void CallOnScript(LuaScript _luaScript, string _functionName, params object[] _args)
    {
        if (_luaScript != null && _luaScript.Globals[_functionName] != null)
        {
            _luaScript.Call(_luaScript.Globals[_functionName], _args);
        }
    }
}

public class LuaHandler : MonoBehaviour {

    // this allows us to specify System.Action (void functions) with more than four types
    public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

    [HideInInspector] public Script luaScript;

    public bool hasRun = false;

    public string scriptName;



    private void Start()
    {

    }

    /// <summary>
    /// Loads a new script string in place of the old one
    /// </summary>
    public void Run() {

        AccessibleLuaScript accessibleLuaScript = Task.GetOrMakeAccessibleLuaScript(luaScript, scriptName);

        if (!GetComponent<WTBObject>().scripts.Contains(accessibleLuaScript))
        {
            GetComponent<WTBObject>().scripts.Add(accessibleLuaScript);
        }
        //if (scriptName == "" || hasRun) return;
        //hasRun = true;

        // tableprint credit to https://stackoverflow.com/questions/9168058/how-to-dump-a-table-to-console

        string builtInEnvironment = @"
            function TablePrint (tbl, indent)
                if not indent then indent = 0 end
                for k, v in pairs(tbl) do
                    formatting = string.rep('  ', indent) .. k .. ': ';
                    if (type(v) == 'table') then
                        print(formatting)
                        TablePrint(v, indent+1)
                    elseif (type(v) == 'boolean') then
                        print(formatting .. tostring(v))
                    else
                        print(formatting .. v)
                    end
                end
            end

            function count(tbl)
                local count = 0
                for k, v in pairs(tbl) do
                    count = count + 1
                end
                return count
            end

            function lerp(a, b, t)
                return a * (1-t) + (b*t)
            end

            


            ";

        try
        {
            if (Task.ScriptTextTable.ContainsKey(scriptName))
            {
                luaScript.DoString(Task.ScriptTextTable[scriptName] + builtInEnvironment);
            }

            LuaGlobalEnvironment.CallOnScript(luaScript, "Start");

            hasRun = true;
        }
        catch (ScriptRuntimeException ex)
        {
            Debug.Log("Lua Error: " + ex.DecoratedMessage);

            string partIdentifier = gameObject.name;

            // null check series
            WTBObject wtbo = GetComponent<WTBObject>();
            if (wtbo != null)
            {
                WorldComponent wc = (WorldComponent)wtbo.ComponentByName("World");
                if (wc != null)
                {
                    PropertyName pn = (PropertyName)wc.PropertyByName("Name");
                    if (pn != null)
                    {
                        partIdentifier = pn.dataString + ":" + gameObject.GetPhotonView().viewID;
                    }
                }
            }

            Task.consoleControllerGlobal.Print("ERROR: [" + partIdentifier + "] " + ex.DecoratedMessage);
            if (Task.consoleController != null)
            {
                Task.consoleController.Print("ERROR: [" + partIdentifier + "] " + ex.DecoratedMessage);
            }
        }
        catch (InterpreterException ex)
        {
            Debug.Log("Lua Error: " + ex.DecoratedMessage);

            string partIdentifier = gameObject.name;

            // null check series
            WTBObject wtbo = GetComponent<WTBObject>();
            if (wtbo != null)
            {
                WorldComponent wc = (WorldComponent)wtbo.ComponentByName("World");
                if (wc != null)
                {
                    PropertyName pn = (PropertyName)wc.PropertyByName("Name");
                    if (pn != null)
                    {
                        partIdentifier = pn.dataString + ":" + gameObject.GetPhotonView().viewID;
                    }
                }
            }

            Task.consoleControllerGlobal.Print("ERROR: [" + partIdentifier + "] " + ex.DecoratedMessage);
            if (Task.consoleController != null)
            {
                Task.consoleController.Print("ERROR: [" + partIdentifier + "] " + ex.DecoratedMessage);
            }
        }
    }

    public void StopTwo()
    {
        if (Task.LuaHandlers != null)
        {
            Task.LuaHandlers.Remove(this);
        }

        luaScript = null;
        SetLuaScript(scriptName);
    }

    public void StopTest()
    {
        SetLuaScript(scriptName);
    }

    /// <summary>
    /// Sets up the lua script environment and preferences, runs the given text as a script,
    /// and calls the start method in the script.
    /// </summary>
    public void SetLuaScript(string _ObjectScriptName) {

        if (_ObjectScriptName == null || _ObjectScriptName == "") return;

        scriptName = _ObjectScriptName;

        string ObjectScriptText = "";
        if (Task.ScriptTextTable != null
            && Task.ScriptTextTable.ContainsKey(_ObjectScriptName))
        {
            ObjectScriptText = Task.ScriptTextTable[_ObjectScriptName];
        }

        if (ObjectScriptText == null || ObjectScriptText == "") return;

        // see: https://www.moonsharp.org/sandbox.html
        // use softsandbox now, this includes all core modules except LoadMethods, OS_System, IO, and Debug, with which users have unlimited access to the system.
        luaScript = new Script(CoreModules.Preset_SoftSandbox | CoreModules.LoadMethod);

        DynValue vec2 = UserData.Create(new Vector2());
        luaScript.Globals.Set("Vector2", vec2);
        
        DynValue vec3 = UserData.Create(new Vector3());
        luaScript.Globals.Set("Vector3", vec3);
        UserData.RegisterExtensionType(typeof(Vector3Extended));
        
        DynValue vec4 = UserData.Create(new Vector4());
        luaScript.Globals.Set("Vector4", vec4);
        
        DynValue col = UserData.Create(new Color());
        luaScript.Globals.Set("Color", col);
        
        luaScript.Options.DebugPrint = s => {
            Debug.Log("Luaprint: " + s);
            Task.consoleControllerGlobal.Print(s);
        };
        
        luaScript.Globals.Set("This", UserData.Create(Task.GetOrMakeLuaPart(GetComponent<WTBObject>())));

        luaScript.Globals["IsHost"] = PhotonNetwork.isMasterClient;

        luaScript.Globals["Time"] = new LuaTime();
        luaScript.Globals["File"] = new LuaFile();
        luaScript.Globals["Game"] = new LuaGame();

        luaScript.Globals["newVector2"] = (System.Func<float, float, Vector2>)newVector2;
        luaScript.Globals["newVector3"] = (System.Func<float, float, float, Vector3>)newVector3;
        luaScript.Globals["newColor"] = (System.Func<float, float, float, float, Color>)newColor;

        luaScript.Globals["SetCameraSwitchAllowed"] = (System.Action<bool>)SetCameraSwitchAllowed;
        luaScript.Globals["SetCameraMode"] = (System.Action<string>)SetCameraMode;
        luaScript.Globals["SetCameraLock"] = (System.Action<bool>)SetCameraLock;

        luaScript.Globals["GetCameraPosition"] = (System.Func<Vector3>)GetCameraPosition;

        luaScript.Globals["Explode"] = (Action<Vector3, float, float, bool, bool>)Explode;

        luaScript.Globals["SetParent"] = (System.Action<LuaWTBObject, LuaWTBObject>)SetParent;
        luaScript.Globals["CreatePart"] = (System.Func<int, Vector3?, Vector3?, LuaWTBObject>)CreatePart;
        luaScript.Globals["CreateLight"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)CreateLight;
        luaScript.Globals["CreateParticles"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)CreateParticles;
        luaScript.Globals["CreateRespawn"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)CreateRespawn;
        luaScript.Globals["CreateWorldText"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)CreateWorldText;
        
        luaScript.Globals["LocalPlayer"] = (System.Func<LuaPlayer>)GetLocalPlayer;
        luaScript.Globals["GetAllPlayers"] = (System.Func<List<LuaPlayer>>)GetAllPlayers;
        luaScript.Globals["PlayerByID"] = (System.Func<int, LuaPlayer>)PlayerByID;
        luaScript.Globals["PlayerByName"] = (System.Func<string, LuaPlayer>)PlayerByName;
        luaScript.Globals["PlayersByNames"] = (System.Func<Table, List<LuaPlayer>>)PlayersByNames;

        luaScript.Globals["GetAllParts"] = (System.Func<List<LuaWTBObject>>)GetAllParts;
        luaScript.Globals["PartByID"] = (System.Func<int, LuaWTBObject>)PartByID;
        luaScript.Globals["PartByName"] = (System.Func<string, LuaWTBObject>)PartByName;
        luaScript.Globals["PartsByName"] = (System.Func<string, List<LuaWTBObject>>)PartsByName;
        luaScript.Globals["PartsByNames"] = (System.Func<Table, List<LuaWTBObject>>)PartsByNames;

        luaScript.Globals["ToJson"] = (System.Func<Table, string>)ToJson;
        luaScript.Globals["FromJson"] = (System.Func<string, Table>)FromJson;

        luaScript.Globals["CreateTalkBubble"] = (System.Action<LuaWTBObject, string>)CreateTalkBubble;
        luaScript.Globals["CreateTalkMessage"] = (System.Action<string>)CreateTalkMessage;
        luaScript.Globals["CreateTalkMessageFor"] = (System.Action<LuaPlayer, string>)CreateTalkMessageFor;

        luaScript.Globals["CreateTimer"] = (System.Action<string, float>)CreateTimer;

        luaScript.Globals["RayCast"] = (System.Func<Vector3, Vector3, LuaHitData>)RayCast;

        luaScript.Globals["NetworkSendToAll"] = (System.Action<string, Table>)NetworkSendToAll;
        luaScript.Globals["NetworkSendToPlayer"] = (System.Action<string, Table, LuaPlayer>)NetworkSendToPlayer;
        luaScript.Globals["NetworkSendToHost"] = (System.Action<string, Table>)NetworkSendToHost;

        luaScript.Globals["HTTPRequestGet"] = (System.Action<string>)HTTPRequestGet;
        luaScript.Globals["HTTPRequestPost"] = (System.Action<string>)HTTPRequestPost;

        luaScript.Globals["InputPressed"] = (System.Func<string, bool>)InputPressed;
        luaScript.Globals["InputHeld"] = (System.Func<string, bool>)InputHeld;
        luaScript.Globals["InputReleased"] = (System.Func<string, bool>)InputReleased;
        luaScript.Globals["UpdateOnClients"] = (System.Action<LuaWTBObject>)UpdateOnClients;
        luaScript.Globals["ShareHostData"] = (System.Action<Table>)ShareHostData;

        // UI
        luaScript.Globals["UIPartByName"] = (System.Func<string, LuaUIObject>)UIPartByName;

        luaScript.Globals["MakeUIWindow"] = (System.Func<Vector2, Vector2, string, LuaUIWindow>)MakeUIWindow;
        luaScript.Globals["MakeUIPanel"] = (System.Func<Vector2, Vector2, LuaUIObject, LuaUIPanel>)MakeUIPanel;
        luaScript.Globals["MakeUIText"] = (System.Func<Vector2, Vector2, string, LuaUIObject, LuaUIText>)MakeUIText;
        luaScript.Globals["MakeUIButton"] = (System.Func<Vector2, Vector2, string, LuaUIObject, LuaUIButton>)MakeUIButton;

        luaScript.Globals["ScreenSize"] = (System.Func<Vector2>)ScreenSize;
        luaScript.Globals["MousePosScreen"] = (System.Func<Vector2>)MousePosScreen;
        luaScript.Globals["MousePosWorld"] = (System.Func<Vector3>)MousePosWorld;

        Task.LuaHandlers.Add(this);
    }



    [BluaMethod(description = "Shares a table of data to other players in the room", scriptSide = ScriptSide.Server)]
    public void ShareHostData(Table data)
    {
        if (PhotonNetwork.isMasterClient)
        {
            string hostDataString = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(data);

            byte[] compressedData = hostDataString.CompressString();


            // we use this for sending network stuff
            // send the compressed data over the network
            Task.handler.photonView.RPC("RPCReceiveHostData", PhotonTargets.Others, compressedData);
        }
    }

    [BluaMethod(description = "Creates a talk bubble on an object", scriptSide = ScriptSide.Client)]
    public void CreateTalkBubble(LuaWTBObject @object, string message)
    {
        TalkController.instance.RecieveTalkMessageNonPlayer(message, @object.WTBObject.gameObject);
    }

    [BluaMethod(description = "Creates a talk message in the chat from the world", scriptSide = ScriptSide.Client)]
    public void CreateTalkMessage(string message)
    {
        if (!PhotonNetwork.isMasterClient) return;

        TalkController.instance.RecieveTalkMessage(message, TalkController.MessageType.Developer);
    }

    [BluaMethod(description = "Creates a talk message in the chat from the world for a specific player", scriptSide = ScriptSide.Client)]
    public void CreateTalkMessageFor(LuaPlayer player, string message)
    {
        if (!PhotonNetwork.isMasterClient) return;

        if (player.playerConnection == PhotonNetwork.player) // if we're the player targeted
        {
            TalkController.instance.RecieveTalkMessage(message, TalkController.MessageType.Developer);
        }
    }

    [BluaMethod(description = "Converts a table to a json string", scriptSide = ScriptSide.Any)]
    public string ToJson(Table jsonTable)
    {
        return MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(jsonTable);
    }

    [BluaMethod(description = "Converts a json string to a table", scriptSide = ScriptSide.Any)]
    public Table FromJson(string jsonString)
    {
        return MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.JsonToTable(jsonString);
    }

    [BluaMethod(description = "Creates an explosion", scriptSide = ScriptSide.Server)]
    public void Explode(Vector3 position, float radius, float power, bool showExplosion = true, bool affectFrozen = false)
    {
        if (PhotonNetwork.isMasterClient) {
            //send rpc telling everyone explosion is happening, we'll blow it up for us when we get that rpc as well, so it's in sync
            Task.handler.photonView.RPC("RPCLuaExplode", PhotonTargets.AllViaServer, position, radius, power, showExplosion, affectFrozen);
        }
    }

    [BluaMethod(description = "Returns true if an input is pressed this frame", scriptSide = ScriptSide.Client)]
    public bool InputPressed(string key)
    {
        return Input.GetKeyDown(key);
    }

    [BluaMethod(description = "Returns true if an input is held this frame", scriptSide = ScriptSide.Client)]
    public bool InputHeld(string key)
    {
        return Input.GetKey(key);
    }

    [BluaMethod(description = "Returns true if an input is released this frame", scriptSide = ScriptSide.Client)]
    public bool InputReleased(string key)
    {
        return Input.GetKeyUp(key);
    }

    [BluaMethod(description = "Returns the screen size", scriptSide = ScriptSide.Client)]
    public Vector2 ScreenSize()
    {
        return new Vector2(Screen.width,Screen.height);
    }

    public float deltaTime
    {
        get
        {
            return Time.deltaTime;
        }
    }

    [BluaMethod(description = "Updates an object for clients", scriptSide = ScriptSide.Server)]
    public void UpdateOnClients(LuaWTBObject objectToUpdate)
    {
        if (!PhotonNetwork.isMasterClient) return;

        if (objectToUpdate.WTBObject == null) return;

        GameObject _go = objectToUpdate.WTBObject.GameObject;
        if (_go == null) return;

        _go.GetComponent<SyncWTBObject>().SendWTBSync();
        //_go.GetPhotonView().RPC("RPCSendWTBSync", PhotonNetwork.masterClient, PhotonNetwork.player);
    }

    [BluaMethod(description = "Sends a message to all clients", scriptSide = ScriptSide.Server)]
    public void NetworkSendToAll(string messageName, Table data)
    {
        if (!PhotonNetwork.isMasterClient) return;

        string _messageSerialized = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(data);
        Task.handler.photonView.RPC("RPCReceiveLuaNetMessageTable", PhotonTargets.All, messageName, _messageSerialized);
    }

    [BluaMethod(description = "Sends a message to a specific Player", scriptSide = ScriptSide.Server)]
    public void NetworkSendToPlayer(string messageName, Table data, LuaPlayer playerToSendTo)
    {
        if (!PhotonNetwork.isMasterClient) return;

        if (playerToSendTo.playerObject != null && playerToSendTo.playerObject.GetComponent<PhotonView>() != null)
        {
            string _messageSerialized = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(data);
            Task.handler.photonView.RPC("RPCReceiveLuaNetMessageTable", playerToSendTo.playerObject.GetComponent<PhotonView>().owner, messageName, _messageSerialized);
        }
    }

    [BluaMethod(description = "Sends a message to the host Player", scriptSide = ScriptSide.Client)]
    public void NetworkSendToHost(string messageName, Table data)
    {
        string _messageSerialized = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(data);
        Task.handler.photonView.RPC("RPCReceiveLuaNetMessageTable", PhotonTargets.MasterClient, messageName, _messageSerialized);
    }

    [BluaMethod(description = "Returns the screen position of the mouse", scriptSide = ScriptSide.Client)]
    public Vector2 MousePosScreen()
    {
        return Input.mousePosition;
    }

    [BluaMethod(description = "Returns the world position of the mouse", scriptSide = ScriptSide.Client)]
    public Vector3 MousePosWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return ray.origin + ray.direction * 9999;
        }
    }

    [BluaMethod(description = "Creates a UI window", scriptSide = ScriptSide.Client)]
    public LuaUIWindow MakeUIWindow(Vector2 screenPosition, Vector2 size, string text = " ")
    {
        GameObject createdUIPiece = Task.luaUIHandler.CreateUIWindow(screenPosition, size, text, Color.white, Color.grey, Color.black);
        return new LuaUIWindow(createdUIPiece);
    }

    [BluaMethod(description = "Creates a UI panel that can hold other UI elements", scriptSide = ScriptSide.Client)]
    public LuaUIPanel MakeUIPanel(Vector2 screenPosition, Vector2 size, LuaUIObject parent = null)
    {
        GameObject parentUIObject = null;
        if (parent != null) parentUIObject = parent.UIObject;
        
        GameObject createdUIPiece = Task.luaUIHandler.CreateUIPanel(screenPosition, size, Color.grey, parentUIObject);

        return new LuaUIPanel(createdUIPiece);
    }

    [BluaMethod(description = "Creates a UI text that can contain a string", scriptSide = ScriptSide.Client)]
    public LuaUIText MakeUIText(Vector2 screenPosition, Vector2 size, string text = " ", LuaUIObject parent = null)
    {
        GameObject parentUIObject = null;
        if (parent != null) parentUIObject = parent.UIObject;

        GameObject createdUIPiece = Task.luaUIHandler.CreateUIText(screenPosition, size, text, parentUIObject, 11, Color.black);
        return new LuaUIText(createdUIPiece);
    }

    [BluaMethod(description = "Creates a UI button that can be clicked by the local player", scriptSide = ScriptSide.Client)]
    public LuaUIButton MakeUIButton(Vector2 screenPosition, Vector2 size, string buttonText = " ", LuaUIObject parent = null)
    {
        GameObject parentUIObject = null;
        if (parent != null) parentUIObject = parent.UIObject;

        GameObject createdUIPiece = Task.luaUIHandler.CreateUIButton(screenPosition, size, buttonText, parentUIObject, 12, Color.grey, Color.black);
        return new LuaUIButton(createdUIPiece);
    }

    [BluaMethod(description = "If set to false, the the player can't switch between third and first person", scriptSide = ScriptSide.Client)]
    public void SetCameraSwitchAllowed(bool allowed)
    {
        Camera.main.GetComponent<MasterCamera>().cameraSwitchAllowed = allowed;
    }

    [BluaMethod(description = "If set to false, the player can't control their camera", scriptSide = ScriptSide.Client)]
    public void SetCameraLock(bool locked)
    {
        Camera.main.GetComponent<MasterCamera>().ToggleCameraLock(locked);
    }

    [BluaMethod(description = "Sets the camera mode with either `first` or `third`", scriptSide = ScriptSide.Client,
        parameterDescriptions = new string[1]
        {
            "'first' = First Person, 'third' = Third Person"
        })]
    public void SetCameraMode(string mode)
    {
        if (mode.ToLower() == "first")
        {
            Camera.main.GetComponent<MasterCamera>().SetCameraDistance(0f);
        }
        if (mode.ToLower() == "third")
        {
            Camera.main.GetComponent<MasterCamera>().SetCameraDistance(-10f);
        }
    }

    [BluaMethod(description = "Creates a new color", scriptSide = ScriptSide.Any)]
    public Color newColor(float red, float green, float blue, float alpha)
    {
        return new Color(red, green, blue, alpha);
    }

    [BluaMethod(description = "Creates a new Vector2", scriptSide = ScriptSide.Any)]
    public Vector2 newVector2(float x, float y)
    {
        return new Vector2(x, y);
    }

    [BluaMethod(description = "Creates a new Vector3", scriptSide = ScriptSide.Any)]
    public Vector3 newVector3(float x, float y, float z)
    {
        return new Vector3(x, y, z);
    }

    [BluaMethod(description = "Checks for a collision between point the start and end points", scriptSide = ScriptSide.Any)]
    public LuaHitData RayCast(Vector3 start, Vector3 end)
    {
        RaycastHit hit;
        
        if (Physics.Linecast(start, end, out hit))
        {
            // hitdata constructor takes hit
            return new LuaHitData(hit);
        }
        // dont return a hit structu if they didn't hit anything, there "was no hit"
        return null;
    }

    [BluaMethod(description = "Sets an object to have a new parent object", scriptSide = ScriptSide.Server)]
    public void SetParent(LuaWTBObject @object, LuaWTBObject newParent)
    {
        @object.WTBObject.parent = newParent.WTBObject;
        @object.WTBObject.ComponentByName("Transform").PropertyByName("HasPhysics").Refresh();
        
    }

    [BluaMethod(description = "Attaches an object to a player", scriptSide = ScriptSide.Server)]
    public void AttachToPlayer(LuaPlayer _lp, LuaWTBObject _lwtbo, string bone = "Head", Vector3? offset = null)
    {
        if (offset == null) offset = Vector3.zero;

        Transform boneTransform = null;
        
        foreach (Renderer child in _lp.playerObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (child.transform.name == bone)
            {
                boneTransform = child.transform;
            }
        }

        if (boneTransform != null)
        {
            if (_lwtbo.WTBObject.GetComponent<Rigidbody>())
            {
                Destroy(_lwtbo.WTBObject.GetComponent<Rigidbody>());
            }
            _lwtbo.WTBObject.GameObject.transform.position = boneTransform.position + (Vector3)offset;
            _lwtbo.WTBObject.GameObject.transform.SetParent(boneTransform);
            _lwtbo.WTBObject.gameObject.layer = LayerMask.NameToLayer("Player");
        }
        else
        {
            throw new ScriptRuntimeException("No bone found");
        }
    }

    [BluaMethod(description = "Returns a player by their network ID", scriptSide = ScriptSide.Any)]
    public LuaPlayer PlayerByID(int id)
    {
        PhotonPlayer player = PhotonPlayer.Find(id);
        return Task.GetOrMakeLuaPlayer(player);
    }

    [BluaMethod(description = "Returns a part by its network ID", scriptSide = ScriptSide.Any)]
    public LuaWTBObject PartByID(int id)
    {
        if (PhotonNetwork.offlineMode)
        {
            return Task.GetOrMakeLuaPart(Task.builderTransform.WTBObjectByIndex[id]);
        }
        else
        {
            PhotonView obj = PhotonView.Find(id);
            if (obj != null && obj.gameObject != null)
            {
                WTBObject wtbobj = obj.gameObject.GetComponent<WTBObject>();
                if (wtbobj == null) return null;
                return Task.GetOrMakeLuaPart(wtbobj);
            }
        }
        return null;
    }

    /// <summary> the specific "Create"s use this abstracted one </summary> 
    LuaWTBObject CreateObject(WTBObject _wtbo, Vector3? _position, Vector3? _angles)
    {
        if (!PhotonNetwork.isMasterClient) { throw new ScriptRuntimeException("You can only create parts on the host"); }

        if (_position == null) _position = Vector3.zero;
        if (_angles == null) _angles = Vector3.zero;

        LuaWTBObject spawned = Task.GetOrMakeLuaPart(_wtbo);
        spawned.position = (Vector3)_position;
        spawned.angles = (Vector3)_angles;

        spawned.WTBObject.GameObject.GetComponentInChildren<MeshRenderer>().enabled = true;

        Camera.main.GetComponent<MasterCamera>().luaRuntimeCreatedObjectList.Add(_wtbo);

        return spawned;
    }

    [BluaMethod(description = "Creates a new object", scriptSide = ScriptSide.Server,
        parameterDescriptions = new string[1]
        {
            "0 = Block, 1 = Ball, 2 = Wedge, 3 = Cylinder, 4 = Cone, 5 = WedgeCorner, 6 = QuarterPipe"
        })]
    public LuaWTBObject CreatePart(int partType, Vector3? position = null, Vector3? angles = null)
    {
        LuaWTBObject luaWTBO = CreateObject(Task.builderTransform.GetCreateObject(partType), position, angles);
        luaWTBO.visible = true;
        return luaWTBO;
    }

    [BluaMethod(description = "Creates a new light object", scriptSide = ScriptSide.Server)]
    public LuaWTBObject CreateLight(Vector3? position = null, Vector3? angles = null)
    {
        return CreateObject(Task.builderTransform.GetCreateObject(0, "Light"), position, angles);
    }

    [BluaMethod(description = "Creates a new particles object", scriptSide = ScriptSide.Server)]
    public LuaWTBObject CreateParticles(Vector3? position = null, Vector3? angles = null)
    {
        return CreateObject(Task.builderTransform.GetCreateObject(0, "Particles"), position, angles);
    }

    [BluaMethod(description = "Creates a new respawn point object", scriptSide = ScriptSide.Server)]
    public LuaWTBObject CreateRespawn(Vector3? position = null, Vector3? angles = null)
    {
        return CreateObject(Task.builderTransform.GetCreateObject(0, "Respawn"), position, angles);
    }

    [BluaMethod(description = "Creates a new world text object", scriptSide = ScriptSide.Server)]
    public LuaWTBObject CreateWorldText(Vector3? position = null, Vector3? angles = null)
    {
        return CreateObject(Task.builderTransform.GetCreateObject(0, "WorldText"), position, angles);
    }

    [BluaMethod(description = "Returns a player by their name", scriptSide = ScriptSide.Server)]
    public LuaPlayer PlayerByName(string name)
    {
        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            if (pp.Nickname == name)
            {
                return Task.GetOrMakeLuaPlayer(pp);
            }
        }

        return null;
    }

    [BluaMethod(description = "Returns a list of players found by a table of names", scriptSide = ScriptSide.Any)]
    public List<LuaPlayer> PlayersByNames(Table tableOfNames)
    {
        List<LuaPlayer> luaPlayerList = new List<LuaPlayer>();

        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            LuaPlayer lp = Task.GetOrMakeLuaPlayer(pp);

            foreach (DynValue nameDyn in tableOfNames.Values)
            {
                
                if (nameDyn.Type == DataType.String)
                {
                    string name = nameDyn.String;

                    if (lp.name == name)
                    {
                        luaPlayerList.Add(lp);
                    }
                }
                else
                {
                    throw new ScriptRuntimeException("list contained non-string object");
                }
            }
        }

        return luaPlayerList;
    }

    [BluaMethod(description = "Returns a list of all parts in the world", scriptSide = ScriptSide.Any)]
    public List<LuaWTBObject> GetAllParts()
    {
        List<LuaWTBObject> partsList = new List<LuaWTBObject>();

        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            partsList.Add(Task.GetOrMakeLuaPart(t));
        }

        return partsList;
    }

    [BluaMethod(description = "Returns a list of parts found by name", scriptSide = ScriptSide.Any)]
    public List<LuaWTBObject> PartsByName(string name)
    {
        List<LuaWTBObject> partsList = new List<LuaWTBObject>();

        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            LuaWTBObject lwtbo = Task.GetOrMakeLuaPart(t);

            if (lwtbo.name == name)
            {
                partsList.Add(Task.GetOrMakeLuaPart(t));
            }
        }

        return partsList;
    }

    [BluaMethod(description = "Returns a list of parts found by a table of names", scriptSide = ScriptSide.Any)]
    public List<LuaWTBObject> PartsByNames(Table tableOfNames)
    {
        List<LuaWTBObject> partsList = new List<LuaWTBObject>();
        
        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            foreach (DynValue nameDyn in tableOfNames.Values)
            {
                if (nameDyn.Type == DataType.String)
                {
                    string name = nameDyn.String;
                    LuaWTBObject lwtbo = Task.GetOrMakeLuaPart(t);

                    if (lwtbo.name == name)
                    {
                        partsList.Add(Task.GetOrMakeLuaPart(t));
                    }
                }
            }
        }

        return partsList;
    }

    [BluaMethod(description = "Returns a UI object by name", scriptSide = ScriptSide.Any)]
    public LuaUIObject UIPartByName(string nameToSearchFor)
    {
        foreach (var entry in Task.luaUIObjects)
        {
            if (entry.Value.name == nameToSearchFor)
            {
                return entry.Value;
            }
        }
        return null;
    }

    [BluaMethod(description = "Returns a part by name", scriptSide = ScriptSide.Any)]
    public LuaWTBObject PartByName(string name)
    {
        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            LuaWTBObject lwtbo = Task.GetOrMakeLuaPart(t);
            
            if (lwtbo.name == name)
            {
                return Task.GetOrMakeLuaPart(t);
            }
        }
        return null;
    }

    [BluaMethod(description = "Returns a list of all players in the world", scriptSide = ScriptSide.Any)]
    public List<LuaPlayer> GetAllPlayers()
    {
        List<LuaPlayer> l_players = new List<LuaPlayer>();
        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            if (pp.ID != null && pp.ID != -1)
            {
                l_players.Add(Task.GetOrMakeLuaPlayer(pp));
            }
        }
        return l_players;
    }

    [BluaMethod(description = "Returns the local player", scriptSide = ScriptSide.Client)]
    public LuaPlayer GetLocalPlayer()
    {
        return Task.GetOrMakeLuaPlayer(PhotonNetwork.player);
    }


    Dictionary<string, float> timerList = new Dictionary<string, float>();

    [BluaMethod(description = "Creates a timer", scriptSide = ScriptSide.Any)]
    public void CreateTimer(string timerName, float duration)
    {
        if (!timerList.ContainsKey(timerName))
        {
            timerList.Add(timerName, Time.time + duration);
        }
        else
        {
            timerList[timerName] = Time.time + duration;
        }
    }

    List<string> timersToEndNextFrame = new List<string>();

    [BluaMethod(description = "This is called once each frame", scriptSide = ScriptSide.Any)]
    public void Update()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "Update");

        // so we dont run into dictionary isssues, mark the ones we are going to delete until after the loop
        List<string> keysForRemoval = new List<string>();

        foreach(var timer in timersToEndNextFrame.ToArray())
        {
            LuaGlobalEnvironment.CallOnScript(luaScript, "TimerEnd", timer);
            timersToEndNextFrame.Remove(timer);
        }

        List<string> keys = new List<string>(timerList.Keys);
        foreach (var timer in keys)
        {
            if (Time.time > timerList[timer])
            {
                timersToEndNextFrame.Add(timer);
                timerList.Remove(timer);
            }
        }
        
        // Input is handled in WTBHandler
    }

    [BluaMethod(description = "This is called when the script first starts up", scriptSide = ScriptSide.Any)]
    public void Awake()
    {
        if (luaScript != null && luaScript.Globals["Awake"] != null)
        {
            LuaGlobalEnvironment.CallOnScript(luaScript, "Awake");
        }
    }

    [BluaMethod(description = "This is called once every physics frame", scriptSide = ScriptSide.Any)]
    public void FixedUpdate()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "FixedUpdate");
    }

    [BluaMethod(description = "This is called once at the end of each frame", scriptSide = ScriptSide.Any)]
    public void LateUpdate()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "DrawUpdate");
    }

    [BluaMethod(description = "This is called when another object collides with this one", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(LuaWTBObject)
        })]
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            LuaGlobalEnvironment.LuaCallOnScript(luaScript, "StartCollision", Task.GetOrMakeLuaPlayer(collision.gameObject));
        }
        else
        {
            WTBObject WTBO = collision.gameObject.GetComponent<WTBObject>();
            if (WTBO != null)
            {
                LuaGlobalEnvironment.LuaCallOnScript(luaScript, "StartCollision", Task.GetOrMakeLuaPart(WTBO));
            }
        }
    }

    [BluaMethod(description = "This is called when another object stops colliding with this one", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(LuaWTBObject)
        })]
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            LuaGlobalEnvironment.LuaCallOnScript(luaScript, "EndCollision", Task.GetOrMakeLuaPlayer(collision.gameObject));
        }
        else
        {
            WTBObject WTBO = collision.gameObject.GetComponent<WTBObject>();
            if (WTBO != null)
            {
                LuaGlobalEnvironment.LuaCallOnScript(luaScript, "EndCollision", Task.GetOrMakeLuaPart(WTBO));
            }
        }
    }

    // https://docs.unity3d.com/Manual/CollidersOverview.html shows that no oncollision event will also end up fiiring an ontrigger, 
    // so we can just still call the collision functions for simplicity

    [BluaMethod(description = "This is called when another object enters the same space as this one", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(LuaWTBObject)
        })]
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Task.LuaCallOnScript(this, "StartCollision", Task.GetOrMakeLuaPlayer(collider.gameObject));
        }
        else
        {
            WTBObject WTBO = collider.gameObject.GetComponent<WTBObject>();
            if (WTBO != null)
            {
                Task.LuaCallOnScript(this, "StartCollision", Task.GetOrMakeLuaPart(WTBO));
            }
        }
    }

    [BluaMethod(description = "This is called when another object exits the same space as this one", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(LuaWTBObject)
        })]
    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Task.LuaCallOnScript(this, "EndCollision", Task.GetOrMakeLuaPlayer(collider.gameObject));
        }
        else
        {
            WTBObject WTBO = collider.gameObject.GetComponent<WTBObject>();
            if (WTBO != null)
            {
                Task.LuaCallOnScript(this, "EndCollision", Task.GetOrMakeLuaPart(WTBO));
            }
        }
    }

    [BluaMethod(description = "This is called when your mouse hovers this object", scriptSide = ScriptSide.Client)]
    public void OnMouseEnter()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "MouseEnter");
    }

    [BluaMethod(description = "This is called when your mouse stops hovering this object", scriptSide = ScriptSide.Client)]
    public void OnMouseExit()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "MouseExit");
    }

    [BluaMethod(description = "This is called when your mouse clicks this object", scriptSide = ScriptSide.Client)]
    public void OnMouseDown()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "MouseDown");
    }

    [BluaMethod(description = "This is called when your mouse button is released over this object", scriptSide = ScriptSide.Client)]
    public void OnMouseUp()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "MouseUp");
    }

    [BluaMethod(description = "This is called when your mouse button is released over this object", scriptSide = ScriptSide.Client)]
    public void OnMouseUpAsButton()
    {
        LuaGlobalEnvironment.CallOnScript(luaScript, "MouseClick");
    }

    [BluaMethod(description = "Sends an HTTP GET to the given URL string. OnWebResponse(string) will be called on return.", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(string)
        })])]
    public void HTTPRequestGet(string url)
    {
        StartCoroutine(HTTPRequest("GET", url, null));
    }

    [BluaMethod(description = "Sends an HTTP POST to the given URL string. OnWebResponse(string) will be called on return. Table should contain string Key - string Value pairs", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(string),
            typeof(Table)
        })])]
    public void HTTPRequestPost(string url, Table _form)
    {
        StartCoroutine(HTTPRequest("POST", url, _form));
    }

    // not directly used by lua
    public IEnumerator HTTPRequest(string type, string url, Table _form)
    {

        switch (type)
        {
            case "GET":

                using (UnityWebRequest www = UnityWebRequest.Get(url))
                {

                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.LogWarning("Web request error: " + www.error);
                    }
                    else
                    {
                        if (luaScript != null && luaScript.Globals["OnWebResponse"] != null)
                        {
                            luaScript.Call(luaScript.Globals["OnWebResponse"], www.downloadHandler.text);
                        }
                    }
                }

                break;

            case "POST":

                WWWForm form = new WWWForm();

                foreach (var entry in _form.Pairs)
                {
                    if (entry.Key.String is string)
                    {
                        form.AddField(entry.Key.String, entry.Value.String);
                    }
                }

                using (UnityWebRequest www = UnityWebRequest.Post(url, form))
                {

                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.LogWarning("Web request error: " + www.error);
                    }
                    else
                    {
                        if (luaScript != null && luaScript.Globals["OnWebResponse"] != null)
                        {
                            luaScript.Call(luaScript.Globals["OnWebResponse"], www.downloadHandler.text);
                        }
                    }
                }

                break;
        }
    }

    [BluaMethod(description = "Returns the main camera position", scriptSide = ScriptSide.Any)]
    public Vector3 GetCameraPosition()
    {
        return Camera.main.transform.position;
    }
}


public class LuaHitData
{
    public LuaObject hitObject;
    public float hitDistance;
    public Vector3 hitPosition;
    public Vector3 hitNormal;

    /*
    public HitData(LuaObject _hitobject = null, float _hitdistance = 0, Vector3 _hitposition = default(Vector3), Vector3 _hitNormal = default(Vector3) )
    {
        hitObject = _hitobject;
        hitDistance = _hitdistance;
        hitPosition = _hitposition;
        hitNormal = _hitNormal;
    }*/

    public LuaHitData(RaycastHit _hit)
    {
        hitPosition = _hit.point;
        hitDistance = _hit.distance;
        hitNormal = _hit.normal;
        if (_hit.collider.gameObject.tag == "Part")
        {
            hitObject = Task.GetOrMakeLuaPart(_hit.collider.GetComponent<WTBObject>());
        }
        else if (_hit.collider.gameObject.tag == "Player")
        {
            hitObject = Task.GetOrMakeLuaPlayer(_hit.collider.gameObject);
        }
    }
}
