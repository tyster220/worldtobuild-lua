using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;


public class LuaWTBObject: LuaObject
{

    [MoonSharpHidden] public WTBObject WTBObject;
    //[MoonSharpHidden] public List<WTBComponent> components;

    public LuaWTBObject(WTBObject _WTBObject)
    {
        this.type = "Part";
        if (_WTBObject != null)
        {
            WTBObject = _WTBObject;
            //components = _WTBObject.Components;
            if (!Task.luaObjects.ContainsKey(_WTBObject.WTBIndex))
            {
                Task.luaObjects.Add(_WTBObject.WTBIndex, this);
            }
            else
            {
                Task.luaObjects[_WTBObject.WTBIndex] = this;
            }
        }
    }

    [BluaMethod(description = "Converts a position relative to this {object} to world space.", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(Vector3)
        })]
    public Vector3 LocalPositionToWorld(Vector3 _point)
    {
        return WTBObject.transform.TransformPoint(_point);
    }

    [BluaMethod(description = "Converts a world space position to a position relative to this {object}", scriptSide = ScriptSide.Any,
        parameterTypes = new System.Type[1]
        {
            typeof(Vector3)
        })]
    public Vector3 WorldPositionToLocal(Vector3 _point)
    {
        return WTBObject.transform.InverseTransformPoint(_point);
    }

    [BluaProperty(description = "The scripts on this {object}")]
    public List<AccessibleLuaScript> scripts
    {
        get
        {
            return WTBObject.scripts;
        }
    }

    [BluaProperty(description = "The net ID of this {object}")]
    public int id
    {
        get
        {
            if (PhotonNetwork.offlineMode == true) return WTBObject.WTBIndex;
            return WTBObject.GameObject.GetComponent<PhotonView>().viewID;
        }
    }

    [BluaProperty(description = "If false, this {object} will ignore raycasts")]
    public bool ignoreRaycast
    {
        get
        {
            return _ignoreRaycast;
        }
        set
        {
            if (value)
            {
                WTBObject.GameObject.layer = 2;
            }
            else
            {
                WTBObject.GameObject.layer = 0;
            }
            _ignoreRaycast = value;
        }
    }
    public bool _ignoreRaycast = false;

    [BluaProperty(description = "The name of this {object}")]
    public string name
    {
        get
        {
            return (string)WTBObject.Components.ComponentByName("World").PropertyByName("Name").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("World").PropertyByName("Name").SetValue(value, false);
        }
    }
    
    [BluaMethod(description = "Returns the children of this {object}", scriptSide = ScriptSide.Any)]
    public List<LuaWTBObject> Children()
    {
        List<LuaWTBObject> l_transforms = new List<LuaWTBObject>();

        foreach (WTBObject child in this.WTBObject.GetChildren())
        {
            l_transforms.Add(Task.GetOrMakeLuaPart(child));
        }

        return l_transforms;
    }

    [BluaMethod(description = "Returns the children of this {object}", scriptSide = ScriptSide.Any,
        deprecated = true, deprecatedMessage = "Please use Children() instead")]
    public List<LuaWTBObject> AllChildren()
    {
        List<LuaWTBObject> l_transforms = new List<LuaWTBObject>();

        foreach (WTBObject child in this.WTBObject.GetChildrenAll())
        {
            l_transforms.Add(Task.GetOrMakeLuaPart(child));
        }

        return l_transforms;
    }

    [BluaProperty(description = "The number of children on this {object}")]
    public int childcount
    {
        get
        {
            return this.WTBObject.children.Count;
        }
    }

    [BluaMethod(description = "Returns a list of children by name", scriptSide = ScriptSide.Any)]
    public List<LuaWTBObject> ChildrenByName(string name)
    {

        List<LuaWTBObject> l_transforms = new List<LuaWTBObject>();
        foreach (int ci in this.WTBObject.children)
        {
            WTBObject wtbo = Task.builderTransform.WTBObjectByIndex[ci];
            LuaWTBObject lwtbo = Task.GetOrMakeLuaPart(wtbo);

            if (lwtbo != null && lwtbo.name == name)
            {
                l_transforms.Add(lwtbo);
            }
        }
        return l_transforms;
    }

    [BluaMethod(description = "Destroys this {object}", scriptSide = ScriptSide.Server)]
    public void Remove()
    {
        WTBObject.Destroy();
    }

    [BluaMethod(description = "Creates a duplicate of this {object} and returns it", scriptSide = ScriptSide.Server)]
    public LuaWTBObject Duplicate()
    {
        List<WTBObject> dupeObjects = new List<WTBObject>() { WTBObject };
        dupeObjects.AddRange(WTBObject.GetChildrenAll());

        DuplicateTargetCommand cmd = new DuplicateTargetCommand(dupeObjects);

        // list also contains the children
        List<WTBObject> duplicatedWTBOs = cmd.LuaDuplicate();

        foreach (WTBObject wtbo in duplicatedWTBOs)
        {
            wtbo.Sync();
            LuaHandler scriptHandler = wtbo.GetComponent<LuaHandler>();
            if (scriptHandler != null)
            {
                scriptHandler.Run();
            }

            
        }

        duplicatedWTBOs[0].transform.position = WTBObject.transform.position;
        duplicatedWTBOs[0].transform.rotation = WTBObject.transform.rotation;

        duplicatedWTBOs[0].ComponentByName("Transform").PropertyByName("HasPhysics").Refresh();

        return Task.GetOrMakeLuaPart(duplicatedWTBOs[0]);
    }

    [BluaProperty(description = "Returns the parent of this {object}")]
    public LuaWTBObject parent
    {
        get
        {
            return Task.GetOrMakeLuaPart(WTBObject.parent);
        }
        set
        {
            WTBObject.parent = value.WTBObject;
        }
    }

    [BluaMethod(description = "Returns a child by name", scriptSide = ScriptSide.Any)]
    public LuaWTBObject ChildByName(string name)
    {

        List<LuaWTBObject> l_transforms = new List<LuaWTBObject>();
        foreach (WTBObject wtbo in this.WTBObject.GetChildrenAll())
        {
            LuaWTBObject lwtbo = Task.GetOrMakeLuaPart(wtbo);

            if (lwtbo != null && lwtbo.name == name)
            {
                return lwtbo;
            }
        }
        return null;
    }

    [BluaProperty(description = "Returns the light component on this {object}")]
    public LuaLight light
    {
        get
        {
            if (WTBObject.ComponentByName("Light") != null)
            {
                return Task.GetOrMakeLuaLight(WTBObject);
            }

            return null;
        }
    }

    [BluaProperty(description = "Returns the particles component on this {object}")]
    public LuaParticleSystem particles
    {
        get
        {
            if (WTBObject.ComponentByName("Particles") != null)
            {
                return Task.GetOrMakeLuaParticleSystem(WTBObject);
            }

            return null;
        }
    }

    [BluaProperty(description = "Returns the respawn point component on this {object}")]
    public LuaRespawn respawn
    {
        get
        {
            if (WTBObject.ComponentByName("Respawn") != null)
            {
                return Task.GetOrMakeLuaRespawn(WTBObject);
            }

            return null;
        }
    }


    [BluaProperty(description = "Returns the text component on this {object}",
        deprecated = true, deprecatedMessage = "Please use worldText")]
    public LuaWorldText text
    {
        get
        {
            if (WTBObject.ComponentByName("Text") != null)
            {
                return Task.GetOrMakeLuaWorldText(WTBObject);
            }

            return null;
        }
    }

    [BluaProperty(description = "Returns the world text component on this {object}")]
    public LuaWorldText worldText
    {
        get
        {
            if (WTBObject.ComponentByName("Text") != null)
            {
                return Task.GetOrMakeLuaWorldText(WTBObject);
            }

            return null;
        }
    }

    [BluaMethod(description = "Rotates the {object} to look at a position", scriptSide = ScriptSide.Server)]
    public void LookAt(Vector3 position)
    {
        WTBObject.GameObject.transform.LookAt(2 * WTBObject.GameObject.transform.position - position);
    }

    [BluaProperty(description = "Returns the script component on this {object}")]
    public string script
    {
        get
        {
            if (WTBObject.GameObject.GetComponent<LuaHandler>() != null)
            {
                return WTBObject.GameObject.GetComponent<LuaHandler>().scriptName;
            }
            return null;
        }
        set
        {
            // if the named script exists in the table
            if (Task.ScriptTextTable.ContainsKey(value))
            {
                // if the script comp doesn't exist, make one
                if (WTBObject.ComponentByName("Script") == null)
                {
                    WTBObject.Components.Add(new ScriptComponent(WTBObject));
                }
                // so if the script component exists
                if (WTBObject.ComponentByName("Script") != null)
                {
                    // set the script reference to the value
                    ((PropertyScript)WTBObject.ComponentByName("Script").PropertyByName("Script")).Actuate(value);
                    // if we're setting this we're already in runtime so start it up
                    WTBObject.GetComponent<LuaHandler>().Run();
                    
                }
            }
        }
    }

    [BluaProperty(description = "Returns the string text of the script running on this {object}")]
    public string scriptText
    {
        get
        {
            return Task.ScriptTextTable[scriptName];
        }
    }



    [BluaProperty(description = "Returns the position of this {object}")]
    public Vector3 position
    {
        get
        {
            return (Vector3)WTBObject.transform.position;
        }
        set
        {
            WTBObject.Components.ComponentByName("Transform").PropertyByName("Position").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Returns the screen position of this {object}")]
    public Vector3 screenposition
    {
        get
        {
            return Camera.main.WorldToScreenPoint(((Vector3)WTBObject.Components.ComponentByName("Transform").PropertyByName("Position").GetValue()));
        }
    }

    [BluaProperty(description = "Returns the angles of this {object}")]
    public Vector3 angles
    {
        get
        {
            return (Vector3)WTBObject.transform.eulerAngles;
        }
        set
        {
            WTBObject.Components.ComponentByName("Transform").PropertyByName("Rotation").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Returns the forward direction vector of this {object}")]
    public Vector3 forward
    {
        get
        {
            return WTBObject.GameObject.transform.forward;
        }
    }

    [BluaProperty(description = "Returns the backward direction vector of this {object}")]
    public Vector3 backward
    {
        get
        {
            return -forward;
        }
    }

    [BluaProperty(description = "Returns the right direction vector of this {object}")]
    public Vector3 right
    {
        get
        {
            return WTBObject.GameObject.transform.right;
        }
    }

    [BluaProperty(description = "Returns the left direction vector of this {object}")]
    public Vector3 left
    {
        get
        {
            return -right;
        }
    }

    [BluaProperty(description = "Returns the up direction vector of this {object}")]
    public Vector3 up
    {
        get
        {
            return WTBObject.GameObject.transform.up;
        }
    }

    [BluaProperty(description = "Returns the down direction vector of this {object}")]
    public Vector3 down
    {
        get
        {
            return -up;
        }
    }

    [BluaProperty(description = "Returns the size of this {object}")]
    public Vector3 size
    {
        get
        {
            return (Vector3)WTBObject.Components.ComponentByName("Transform").PropertyByName("Size").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Transform").PropertyByName("Size").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Returns the velocity of this {object}")]
    public Vector3 velocity
    {
        get
        {
            return WTBObject.GetComponent<Rigidbody>().velocity;
        }
        set
        {
            WTBObject.GetComponent<Rigidbody>().velocity = value;
        }
    }

    [BluaProperty(description = "Returns the angular velocity of this {object}")]
    public Vector3 angularvelocity
    {
        get
        {
            return WTBObject.GetComponent<Rigidbody>().angularVelocity;
        }
        set
        {
            WTBObject.GetComponent<Rigidbody>().angularVelocity = value;
        }
    }

    [BluaProperty(description = "Returns the mass of this {object}")]
    public float mass
    {
        get
        {
            return WTBObject.GetComponent<Rigidbody>().mass;
        }
        set
        {
            WTBObject.GetComponent<Rigidbody>().mass = value;
        }
    }

    [BluaProperty(description = "Returns the transparency of this {object}")]
    public float transparency
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Renderer").PropertyByName("Transparency").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Renderer").PropertyByName("Transparency").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Returns the color of this {object}")]
    public Color color
    {
        get
        {
            Color color = new Color(-1, -1, -1, -1);
            color = (Color)WTBObject.Components.ComponentByName("Renderer").PropertyByName("Color").GetValue();
            return color;
        }
        set
        {
            // mask vec4 to color
            RendererComponent renderComp = (RendererComponent)WTBObject.ComponentByName("Renderer");
            ((PropertyTransparency)renderComp.PropertyByName("Transparency")).Actuate(1-((Color)value).a);
            ((PropertyColor)renderComp.PropertyByName("Color")).Actuate((Color)value);
            //WTBObject.Components.ComponentByName("Renderer").PropertyByName("Color").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Returns whether or not this {object} is frozen and can interact with physics")]
    public bool frozen
    {
        get
        {
            return !(bool)WTBObject.Components.ComponentByName("Transform").PropertyByName("HasPhysics").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Transform").PropertyByName("HasPhysics").SetValue(!value, false);
        }
    }

    [BluaProperty(description = "Returns whether or not this {object} has rounded edges")]
    public bool bevel
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Renderer").PropertyByName("Rounded").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Renderer").PropertyByName("Rounded").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Returns whether or not this {object} is visible")]
    public bool visible
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Renderer").PropertyByName("Visible").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Renderer").PropertyByName("Visible").SetValue(value, false);
        }
    }

    [BluaProperty(description = "Returns whether or not this {object} can collide with other objects")]
    public bool cancollide
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Transform").PropertyByName("CanCollide").GetValue();
        }
        set
        {
            bool val = value;
            WTBObject.Components.ComponentByName("Transform").PropertyByName("CanCollide").dataBool = value;
            WTBObject.StartCoroutine(((PropertyCanCollide)WTBObject.Components.ComponentByName("Transform").PropertyByName("CanCollide")).RefreshDelayed(1));
        }
    }

    [BluaProperty(description = "Returns the part type of this {object}")]
    public int parttype
    {
        get
        {
            return (int)WTBObject.Components.ComponentByName("Renderer").PropertyByName("PartType").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Renderer").PropertyByName("PartType").SetValue(value, false);
        }
    }
}