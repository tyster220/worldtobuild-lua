using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using System;
using MoonSharp.Interpreter.CoreLib;
using UnityEngine.Networking;

public static class Vector3Extended
{
    public static Vector2 ScreenPosition(this Vector3 _vec)
    {
        Vector2 reasonableVector = Camera.main.WorldToScreenPoint(_vec);
        reasonableVector.y = -reasonableVector.y;
        return reasonableVector;
    }
}


public static class LuaGlobalEnvironment
{

    // really doesn't matter if it runs multiple times :p
    public static void Start()
    {
        UserData.RegisterAssembly();
        //UserData.RegisterType<Transform>();

        UserData.RegisterType<LuaHitData>();


        UserData.RegisterType<LuaObject>();

        UserData.RegisterType<LuaWTBObject>();
        UserData.RegisterType<LuaPlayer>();

        UserData.RegisterType<LuaTime>();
        UserData.RegisterType<LuaFile>();

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
    }
}

public class LuaHandler : MonoBehaviour
{

    // this allows us to specify System.Action (void functions) with more than four types
    public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

    NewChatSystem chatSystem;
    [HideInInspector] public Script luaScript;

    public bool hasRun = false;

    public string scriptName;

    private void Start()
    {

    }
    /// <summary>
    /// Loads a new script string in place of the old one
    /// </summary>
    public void Run()
    {

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

            Task.LuaCallOnScript(this, "Start");

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



    public void ShareHostData(Table _hostData)
    {
        if (PhotonNetwork.isMasterClient)
        {
            string hostDataString = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(_hostData);

            byte[] compressedData = hostDataString.Compress();


            // we use this for sending network stuff
            // send the compressed data over the network
            Task.handler.photonView.RPC("RPCReceiveHostData", PhotonTargets.Others, compressedData);
        }
    }


    public void StopTwo()
    {
        Task.LuaHandlers.Remove(this);
        luaScript = null;
        SetLuaScript(scriptName);
    }

    public void StopTest()
    {
        SetLuaScript(scriptName);
    }



    void Lua_CreateTalkBubble(LuaWTBObject _LO, string _text)
    {
        if (chatSystem == null)
        {
            chatSystem = FindObjectOfType<NewChatSystem>();
        }

        chatSystem.CreateTalkBubble(_LO.WTBObject.gameObject, _text);
    }

    void Lua_CreateTalkBubble(LuaPlayer _LO, string _text)
    {
        if (chatSystem == null)
        {
            chatSystem = FindObjectOfType<NewChatSystem>();
        }

        chatSystem.CreateTalkBubble(_LO.playerObject.gameObject, _text);
    }






    /// <summary>
    /// Sets up the lua script environment and preferences, runs the given text as a script,
    /// and calls the start method in the script.
    /// </summary>
    public void SetLuaScript(string _ObjectScriptName)
    {

        if (_ObjectScriptName == null || _ObjectScriptName == "") return;

        scriptName = _ObjectScriptName;

        string ObjectScriptText = Task.ScriptTextTable[_ObjectScriptName];

        if (ObjectScriptText == null || ObjectScriptText == "") return;

        // see: https://www.moonsharp.org/sandbox.html
        // use softsandbox now, this includes all core modules except LoadMethods, OS_System, IO, and Debug, with which users have unlimited access to the system.
        /// new: also give the load methods: "load", "loadsafe", "loadfile", "loadfilesafe", "dofile" and "require"
        luaScript = new Script(CoreModules.Preset_SoftSandbox | CoreModules.LoadMethod);

        //UserData.RegisterType<IList>();
        //UserData.RegisterType<IDictionary>();

        DynValue vec2 = UserData.Create(new Vector2());
        luaScript.Globals.Set("Vector2", vec2);

        DynValue vec3 = UserData.Create(new Vector3());
        luaScript.Globals.Set("Vector3", vec3);
        UserData.RegisterExtensionType(typeof(Vector3Extended));

        DynValue vec4 = UserData.Create(new Vector4());
        luaScript.Globals.Set("Vector4", vec4);

        DynValue col = UserData.Create(new Color());
        luaScript.Globals.Set("Color", col);
        //UserData.RegisterType<MeshType>();

        luaScript.Options.DebugPrint = s => {
            Debug.Log("Luaprint: " + s);
            Task.consoleControllerGlobal.Print(s);
        };




        //luaScript.Globals.Set("transform", UserData.Create(transform));
        luaScript.Globals.Set("This", UserData.Create(Task.GetOrMakeLuaPart(GetComponent<WTBObject>())));


        //luaScript.Globals.Set("LocalPlayer", UserData.Create(GetLocalPlayer));
        //luaScript.Globals.Set("LocalPlayer", UserData.Create(GetLocalPlayer()));

        luaScript.Globals["IsHost"] = PhotonNetwork.isMasterClient;

        luaScript.Globals["ScreenSize"] = (System.Func<Vector2>)GetScreenSize;

        luaScript.Globals["Time"] = new LuaTime();
        luaScript.Globals["File"] = new LuaFile();

        luaScript.Globals["LocalPlayer"] = (System.Func<LuaPlayer>)GetLocalPlayer;
        //luaScript.Globals["MeshType"] = UserData.CreateStatic<MeshType>();

        //luaScript.Globals["Remove"] = (System.Action<LuaWTBObject>)Remove;
        //luaScript.Globals["Remove"] = (System.Func<LuaPlayer>)Remove;

        luaScript.Globals["newVector2"] = (System.Func<float, float, Vector2>)Vector2Create;
        luaScript.Globals["newVector3"] = (System.Func<float, float, float, Vector3>)Vector3Create;
        luaScript.Globals["newColor"] = (System.Func<float, float, float, float, Color>)ColorCreate;

        luaScript.Globals["SetCameraSwitchAllowed"] = (System.Action<bool>)SetCameraSwitchAllowed;
        luaScript.Globals["SetCameraMode"] = (System.Action<string>)SetCameraMode;
        luaScript.Globals["SetCameraLock"] = (System.Action<bool>)SetCameraLock;
        //luaScript.Globals["Vector4"] = (System.Func<int, int, int, int, Vector4>)Vector4Create;

        luaScript.Globals["CreatePart"] = (System.Func<int, Vector3?, Vector3?, LuaWTBObject>)LuaCreatePart;

        luaScript.Globals["CreateLight"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)LuaCreateLight;
        luaScript.Globals["CreateParticles"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)LuaCreateParticles;
        luaScript.Globals["CreateRespawn"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)LuaCreateRespawn;
        luaScript.Globals["CreateWorldText"] = (System.Func<Vector3?, Vector3?, LuaWTBObject>)LuaCreateWorldText;


        luaScript.Globals["GetAllParts"] = (System.Func<List<LuaWTBObject>>)LuaGetAllParts;
        luaScript.Globals["GetAllPlayers"] = (System.Func<List<LuaPlayer>>)LuaGetAllPlayers;

        luaScript.Globals["PlayerByName"] = (System.Func<string, LuaPlayer>)LuaGetPlayerByName;
        luaScript.Globals["PlayersByNames"] = (System.Func<Table, List<LuaPlayer>>)LuaGetPlayersByName;

        luaScript.Globals["PartsByName"] = (System.Func<string, List<LuaWTBObject>>)LuaGetPartsByName;
        luaScript.Globals["PartsByNames"] = (System.Func<Table, List<LuaWTBObject>>)LuaGetPartsByName;
        luaScript.Globals["PartByName"] = (System.Func<string, LuaWTBObject>)LuaGetPartByName;

        luaScript.Globals["UIPartByName"] = (System.Func<string, LuaUIObject>)LuaGetUIPartByName;



        luaScript.Globals["PlayerByID"] = (System.Func<int, LuaPlayer>)PlayerByID;
        luaScript.Globals["PartByID"] = (System.Func<int, LuaWTBObject>)PartByID;

        luaScript.Globals["CreateTalkBubble"] = (System.Action<LuaWTBObject, string>)Lua_CreateTalkBubble;
        luaScript.Globals["CreateTalkBubble"] = (System.Action<LuaPlayer, string>)Lua_CreateTalkBubble;
        luaScript.Globals["CreateTimer"] = (System.Action<string, float>)LuaCreateTimer;

        //luaScript.Globals["AttachToPlayer"] = (System.Action<LuaPlayer, LuaWTBObject, string, Vector3?>)AttachToPlayer;

        // replace with .parent
        luaScript.Globals["SetParent"] = (System.Action<LuaWTBObject, LuaWTBObject>)LuaSetParent;
        //luaScript.Globals["SetParent"] = (System.Action<LuaUIObject, LuaUIObject>)LuaSetParent;

        luaScript.Globals["RayCast"] = (System.Func<Vector3, Vector3, LuaHitData>)LuaRayCast;

        luaScript.Globals["NetworkSendToAll"] = (System.Action<string, Table>)LuaNetMessageToAll;
        luaScript.Globals["NetworkSendToPlayer"] = (System.Action<string, Table, LuaPlayer>)LuaNetMessageToPlayer;
        luaScript.Globals["NetworkSendToHost"] = (System.Action<string, Table>)LuaNetMessageToHost;

        luaScript.Globals["HTTPRequestGet"] = (System.Action<string>)HTTPRequestGet;
        luaScript.Globals["HTTPRequestPost"] = (System.Action<string>)HTTPRequestPost;

        luaScript.Globals["InputPressed"] = (System.Func<string, bool>)LuaInputPressed;
        luaScript.Globals["InputHeld"] = (System.Func<string, bool>)LuaInputHeld;
        luaScript.Globals["InputReleased"] = (System.Func<string, bool>)LuaInputReleased;
        luaScript.Globals["UpdateOnClients"] = (System.Action<LuaWTBObject>)UpdateOnClients;
        //NetworkStringReceive(luaPlayer,string,table);
        luaScript.Globals["ShareHostData"] = (System.Action<Table>)ShareHostData;

        // UI

        //luauiwindow and luauipanel don't exist yet
        luaScript.Globals["MakeUIWindow"] = (System.Func<Vector2, Vector2, string, LuaUIWindow>)LuaMakeUIWindow;
        luaScript.Globals["MakeUIPanel"] = (System.Func<Vector2, Vector2, LuaUIObject, LuaUIPanel>)LuaMakeUIPanel;
        luaScript.Globals["MakeUIText"] = (System.Func<Vector2, Vector2, string, LuaUIObject, LuaUIText>)LuaMakeUIText;
        luaScript.Globals["MakeUIButton"] = (System.Func<Vector2, Vector2, string, LuaUIObject, LuaUIButton>)LuaMakeUIButton;

        //MouseWorldPos
        //MouseScreenPos

        luaScript.Globals["MousePosScreen"] = (System.Func<Vector2>)MouseScreenPos;
        luaScript.Globals["MousePosWorld"] = (System.Func<Vector3>)MouseWorldPos;

        luaScript.Globals["Explode"] = (Action<Vector3, float, float, bool, bool>)LuaExplode;

        luaScript.Globals["StartGame"] = (System.Action)GameKitStartGame;

        //GameObject CreateUIWindow(Vector2 _position, Vector2 _size, string _title=" ", Color? _colorN = null, Color? _titleBarColorN = null, Color? _titleColorN = null)
        //GameObject CreateUIPanel(Vector2 _position, Vector2 _size, Color? _colorN = null, GameObject _parent = null)
        //GameObject CreateUIText(Vector2 _position, Vector2 _size, string _text = " ", GameObject _parent = null, int _fontSize = 11, Color? _textColorN = null)
        //GameObject CreateUIButton(Vector2 _position, Vector2 _size, string _text = " ", GameObject _parent = null, int _fontSize = 11, Color? _buttonColorN = null, Color? _textColorN = null)


        //luaScript.DoString(ObjectScriptText);

        Task.LuaHandlers.Add(this);



    }

    void GameKitStartGame()
    {
        if (Task.gameSettingsController.settings.IsTeamBattle)
        {
            GameObject TeamSelectionUI = Instantiate(Resources.Load<GameObject>("Team Selection"), Task.Canvas);//reference to prefab, reference to canvas
            Task.teamSelectionController = TeamSelectionUI.GetComponent<TeamSelectionController>();
            Task.teamSelectionController.CreateTeamButtons();
            Task.teamSelectionController.DisplayTeamInformation();
        }
    }

    void GameKitEndGame()
    {
        Task.EndGame(Task.SortLeaders()[0]);
    }

    void LuaExplode(Vector3 _position, float _radius, float _power, bool _showExplosion = true, bool _affectFrozen = false)
    {
        if (PhotonNetwork.isMasterClient)
        {
            //send rpc telling everyone explosion is happening, we'll blow it up for us when we get that rpc as well, so it's in sync
            Task.handler.photonView.RPC("RPCLuaExplode", PhotonTargets.AllViaServer, _position, _radius, _power, _showExplosion, _affectFrozen);
        }
    }


    // keys use unity structure listed at https://docs.unity3d.com/Manual/ConventionalGameInput.html
    bool LuaInputPressed(string _key)
    {
        return Input.GetKeyDown(_key);
    }
    bool LuaInputHeld(string _key)
    {
        return Input.GetKey(_key);
    }
    bool LuaInputReleased(string _key)
    {
        return Input.GetKeyUp(_key);
    }

    private Vector2 GetScreenSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }

    float deltaTime
    {
        get
        {
            return Time.deltaTime;
        }
    }

    void UpdateOnClients(LuaWTBObject _lwtbo)
    {
        if (_lwtbo.WTBObject == null) return;

        GameObject _go = _lwtbo.WTBObject.GameObject;
        if (_go == null) return;

        if (!PhotonNetwork.isMasterClient) return;

        _go.GetComponent<SyncWTBObject>().SendWTBSync();
        //_go.GetPhotonView().RPC("RPCSendWTBSync", PhotonNetwork.masterClient, PhotonNetwork.player);
    }
    /*
    string TableToString(DynValue _dynValue)
    {
        if (_dynValue.Type == DataType.Table) {
            string s = "{ ";

            int i = 0;
            foreach (var pair in _dynValue.Table.Pairs)
            {
                var k = pair.Key;
                var v = pair.Value;
                i++;
                if (k.Type != DataType.Number) k = DynValue.NewString("\"" + k + "\"");
                s = s + "[" + k + "] = " + TableToString(v) + ",";

            }
        }
        else
        {
            return _dynValue.CastToString();
        }*/
    /*
    function dump(o)
        if type(o) == 'table' then
            local s = '{ '
            for k, v in pairs(o) do
                if type(k) ~= 'number' then k = '"'..k..'"' end
                s = s.. '['..k..'] = '..dump(v).. ','
            end
            return s.. '} '
        else
            return tostring(o)
        end
    end*/
    //}

    #region LuaNetMessages
    void LuaNetMessageToAll(string _messageName, Table _message)
    {
        if (!PhotonNetwork.isMasterClient) return;
        string _messageSerialized = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(_message);
        Task.handler.photonView.RPC("RPCReceiveLuaNetMessageTable", PhotonTargets.All, _messageName, _messageSerialized);
    }

    void LuaNetMessageToPlayer(string _messageName, Table _message, LuaPlayer _player)
    {
        if (!PhotonNetwork.isMasterClient) return;
        if (_player.playerObject != null && _player.playerObject.GetComponent<PhotonView>() != null)
        {
            string _messageSerialized = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(_message);
            Task.handler.photonView.RPC("RPCReceiveLuaNetMessageTable", _player.playerObject.GetComponent<PhotonView>().owner, _messageName, _messageSerialized);
        }
    }

    void LuaNetMessageToHost(string _messageName, Table _message)
    {
        string _messageSerialized = MoonSharp.Interpreter.Serialization.Json.JsonTableConverter.TableToJson(_message);
        Task.handler.photonView.RPC("RPCReceiveLuaNetMessageTable", PhotonTargets.MasterClient, _messageName, _messageSerialized);

    }

    #endregion

    Vector2 MouseScreenPos()
    {
        return Input.mousePosition;
    }

    Vector3 MouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
            //Debug.Log(hit);
        }
        return Vector3.zero;
    }

    LuaUIWindow LuaMakeUIWindow(Vector2 _postion, Vector2 _size, string _text = " ")
    {
        GameObject createdUIPiece = Task.luaUIHandler.CreateUIWindow(_postion, _size, _text, Color.white, Color.grey, Color.black);
        return new LuaUIWindow(createdUIPiece);
    }

    LuaUIPanel LuaMakeUIPanel(Vector2 _postion, Vector2 _size, LuaUIObject _parent = null)
    {
        GameObject parentUIObject = null;
        if (_parent != null) parentUIObject = _parent.UIObject;

        GameObject createdUIPiece = Task.luaUIHandler.CreateUIPanel(_postion, _size, Color.grey, parentUIObject);

        return new LuaUIPanel(createdUIPiece);
    }

    LuaUIText LuaMakeUIText(Vector2 _postion, Vector2 _size, string _text = " ", LuaUIObject _parent = null)
    {
        GameObject parentUIObject = null;
        if (_parent != null) parentUIObject = _parent.UIObject;

        GameObject createdUIPiece = Task.luaUIHandler.CreateUIText(_postion, _size, _text, parentUIObject, 11, Color.black);
        return new LuaUIText(createdUIPiece);
    }

    LuaUIButton LuaMakeUIButton(Vector2 _postion, Vector2 _size, string _text = " ", LuaUIObject _parent = null)
    {
        GameObject parentUIObject = null;
        if (_parent != null) parentUIObject = _parent.UIObject;

        GameObject createdUIPiece = Task.luaUIHandler.CreateUIButton(_postion, _size, _text, parentUIObject, 12, Color.grey, Color.black);
        return new LuaUIButton(createdUIPiece);
    }

    private void SetCameraSwitchAllowed(bool _allowed)
    {
        Camera.main.GetComponent<MasterCamera>().cameraSwitchAllowed = _allowed;
    }

    private void SetCameraLock(bool _locked)
    {
        Camera.main.GetComponent<MasterCamera>().ToggleMouseLock(_locked);
    }

    private void SetCameraMode(string _mode)
    {

        if (_mode.ToLower() == "first")
        {
            Camera.main.GetComponent<MasterCamera>().SetCameraDistance(0f);
        }
        if (_mode.ToLower() == "third")
        {
            Camera.main.GetComponent<MasterCamera>().SetCameraDistance(-10f);
        }
    }

    private Color ColorCreate(float _r, float _g, float _b, float _a)
    {
        return new Color(_r, _g, _b, _a);
    }

    private Vector2 Vector2Create(float _x, float _y)
    {
        return new Vector2(_x, _y);
    }

    private Vector3 Vector3Create(float _x, float _y, float _z)
    {
        return new Vector3(_x, _y, _z);
    }

    LuaHitData LuaRayCast(Vector3 _pointa, Vector3 _pointb)
    {
        RaycastHit hit;

        if (Physics.Linecast(_pointa, _pointb, out hit))
        {
            // hitdata constructor takes hit
            return new LuaHitData(hit);
        }
        // dont return a hit structu if they didn't hit anything, there "was no hit"
        return null;
    }

    void LuaSetParent(LuaWTBObject _child, LuaWTBObject _parent)
    {
        //_child.WTBObject.transform.SetParent(_parent.WTBObject.transform);
        _child.WTBObject.parent = _parent.WTBObject;
        _child.WTBObject.ComponentByName("Transform").PropertyByName("HasPhysics").Refresh();

    }

    void LuaSetParent(LuaUIObject _child, LuaUIObject _parent)
    {
        _child.UIObject.transform.SetParent(_parent.UIObject.transform);
    }

    // test[]
    void AttachToPlayer(LuaPlayer _lp, LuaWTBObject _lwtbo, string bone = "Head", Vector3? offset = null)
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


    LuaPlayer PlayerByID(int _id)
    {
        PhotonPlayer player = PhotonPlayer.Find(_id);
        return Task.GetOrMakeLuaPlayer(player);
    }

    LuaWTBObject PartByID(int _id)
    {
        if (PhotonNetwork.offlineMode)
        {
            return Task.GetOrMakeLuaPart(Task.builderTransform.WTBObjectByIndex[_id]);
        }
        else
        {
            PhotonView obj = PhotonView.Find(_id);
            if (obj != null && obj.gameObject != null)
            {
                WTBObject wtbobj = obj.gameObject.GetComponent<WTBObject>();
                if (wtbobj == null) return null;
                return Task.GetOrMakeLuaPart(wtbobj);
            }
        }
        return null;
    }

    void LuaEnableNetworking(LuaWTBObject _lwtbo)
    {
        if (PhotonNetwork.isMasterClient)
        {
            WTBObject wtbo = _lwtbo.WTBObject;
            PhotonView PV = wtbo.GetComponent<PhotonView>();

            // dont keep going if we're already networked
            if (PV.ObservedComponents.Count > 1) return;

            PV.ObservedComponents.RemoveRange(0, PV.ObservedComponents.Count);
        }
    }

    void LuaDisableNetworking(LuaWTBObject _lwtbo)
    {
        if (PhotonNetwork.isMasterClient)
        {
            WTBObject wtbo = _lwtbo.WTBObject;
            PhotonView PV = wtbo.GetComponent<PhotonView>();

            PV.ObservedComponents.RemoveRange(0, PV.ObservedComponents.Count);
        }
    }

    // the specific creates use this abstracted one
    LuaWTBObject LuaCreateObject(WTBObject _wtbo, Vector3? _position, Vector3? _angles)
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

    LuaWTBObject LuaCreatePart(int _part, Vector3? _position = null, Vector3? _angles = null)
    {
        LuaWTBObject luaWTBO = LuaCreateObject(Task.builderTransform.GetCreateObject(_part), _position, _angles);
        luaWTBO.visible = true;
        return luaWTBO;
    }

    LuaWTBObject LuaCreateLight(Vector3? _position = null, Vector3? _angles = null)
    {
        return LuaCreateObject(Task.builderTransform.GetCreateObject(0, "Light"), _position, _angles);
    }

    LuaWTBObject LuaCreateParticles(Vector3? _position = null, Vector3? _angles = null)
    {
        return LuaCreateObject(Task.builderTransform.GetCreateObject(0, "Particles"), _position, _angles);
    }

    LuaWTBObject LuaCreateRespawn(Vector3? _position = null, Vector3? _angles = null)
    {
        return LuaCreateObject(Task.builderTransform.GetCreateObject(0, "Respawn"), _position, _angles);
    }

    LuaWTBObject LuaCreateWorldText(Vector3? _position = null, Vector3? _angles = null)
    {
        return LuaCreateObject(Task.builderTransform.GetCreateObject(0, "WorldText"), _position, _angles);
    }


    LuaPlayer LuaGetPlayerByName(string _name)
    {
        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            if (pp.Nickname == _name)
            {
                return Task.GetOrMakeLuaPlayer(pp);
            }
        }

        return null;
    }

    List<LuaPlayer> LuaGetPlayersByName(Table _names)
    {
        List<LuaPlayer> luaPlayerList = new List<LuaPlayer>();

        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            LuaPlayer lp = Task.GetOrMakeLuaPlayer(pp);

            foreach (DynValue nameDyn in _names.Values)
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

    List<LuaWTBObject> LuaGetAllParts()
    {
        List<LuaWTBObject> partsList = new List<LuaWTBObject>();

        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            partsList.Add(Task.GetOrMakeLuaPart(t));
        }

        return partsList;
    }

    List<LuaWTBObject> LuaGetPartsByName(string _name)
    {
        List<LuaWTBObject> partsList = new List<LuaWTBObject>();

        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            LuaWTBObject lwtbo = Task.GetOrMakeLuaPart(t);

            if (lwtbo.name == _name)
            {
                partsList.Add(Task.GetOrMakeLuaPart(t));
            }
        }

        return partsList;
    }

    List<LuaWTBObject> LuaGetPartsByName(Table _names)
    {
        List<LuaWTBObject> partsList = new List<LuaWTBObject>();

        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            foreach (DynValue nameDyn in _names.Values)
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

    LuaUIObject LuaGetUIPartByName(string _name)
    {
        foreach (var entry in Task.luaUIObjects)
        {
            if (entry.Value.name == _name)
            {
                return entry.Value;
            }
        }
        return null;
    }

    LuaWTBObject LuaGetPartByName(string _name)
    {
        foreach (WTBObject t in FindObjectsOfType<WTBObject>())
        {
            LuaWTBObject lwtbo = Task.GetOrMakeLuaPart(t);

            if (lwtbo.name == _name)
            {
                return Task.GetOrMakeLuaPart(t);
            }
        }
        return null;
    }

    List<LuaPlayer> LuaGetAllPlayers()
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



    // add by name functions



    LuaPlayer GetLocalPlayer()
    {
        return Task.GetOrMakeLuaPlayer(PhotonNetwork.player);
    }


    void Awake()
    {
        Task.LuaCallOnScript(this, "Awake");
    }

    Dictionary<string, float> timerList = new Dictionary<string, float>();

    void LuaCreateTimer(string _timerName, float _timer)
    {
        if (!timerList.ContainsKey(_timerName))
        {
            timerList.Add(_timerName, Time.time + _timer);
        }
        else
        {
            timerList[_timerName] = Time.time + _timer;
        }
    }

    List<string> timersToEndNextFrame = new List<string>();
    void Update()
    {
        Task.LuaCallOnScript(this, "Update");

        // so we dont run into dictionary isssues, mark the ones we are going to delete until after the loop
        List<string> keysForRemoval = new List<string>();

        foreach (var timer in timersToEndNextFrame.ToArray())
        {
            if (luaScript != null && luaScript.Globals["TimerEnd"] != null)
            {
                luaScript.Call(luaScript.Globals["TimerEnd"], timer);
            }
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

    void FixedUpdate()
    {
        Task.LuaCallOnScript(this, "FixedUpdate");
    }

    private void LateUpdate()
    {
        Task.LuaCallOnScript(this, "DrawUpdate");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Task.LuaCallOnScript(this, "StartCollision", Task.GetOrMakeLuaPlayer(collision.gameObject));
        }
        else
        {
            WTBObject WTBO = collision.gameObject.GetComponent<WTBObject>();
            if (WTBO != null)
            {
                Task.LuaCallOnScript(this, "StartCollision", Task.GetOrMakeLuaPart(WTBO));
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Task.LuaCallOnScript(this, "EndCollision", Task.GetOrMakeLuaPlayer(collision.gameObject));
        }
        else
        {
            WTBObject WTBO = collision.gameObject.GetComponent<WTBObject>();
            if (WTBO != null)
            {
                Task.LuaCallOnScript(this, "EndCollision", Task.GetOrMakeLuaPart(WTBO));
            }
        }
    }



    // https://docs.unity3d.com/Manual/CollidersOverview.html shows that no oncollision event will also end up fiiring an ontrigger, 
    // so we can just still call the collision functions for simplicity

    void OnTriggerEnter(Collider collider)
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
    void OnTriggerExit(Collider collider)
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

    private void OnMouseEnter()
    {
        Task.LuaCallOnScript(this, "MouseEnter");
    }

    private void OnMouseExit()
    {
        Task.LuaCallOnScript(this, "MouseExit");
    }

    private void OnMouseDown()
    {
        Task.LuaCallOnScript(this, "MouseDown");
    }

    private void OnMouseUp()
    {
        Task.LuaCallOnScript(this, "MouseUp");
    }

    private void OnMouseUpAsButton()
    {
        Task.LuaCallOnScript(this, "MouseClick");
    }







    [BluaMethod(description = "Sends an HTTP GET to the given URL string. OnWebResponse(string) will be called on return.")]
    public void HTTPRequestGet(string url)
    {
        StartCoroutine(HTTPRequest("GET", url, null));
    }

    [BluaMethod(description = "Sends an HTTP POST to the given URL string. OnWebResponse(string) will be called on return.")]
    public void HTTPRequestPost(string url, Table _form)
    {
        StartCoroutine(HTTPRequest("POST", url, _form));
    }

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
