using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

[BluaClass(description = "A particle system that can emit particles")]
public class LuaParticleSystem: LuaObject
{

    [MoonSharpHidden] public WTBObject WTBObject;
    [MoonSharpHidden] public List<WTBComponent> components;

    public LuaParticleSystem(WTBObject _WTBObject)
    {
        this.type = "Particles";
        if (_WTBObject != null)
        {
            WTBObject = _WTBObject;
            components = _WTBObject.Components;
            Task.luaParticleSystems.Add(WTBObject.WTBIndex, this);
        }
    }

    [BluaProperty(description = "If false, this {object} will not emit any particles")]
    public bool enabled
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Particles").PropertyByName("Enabled").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Enabled").SetValue(value);
        }
    }

    [BluaProperty(description = "If true, this {object} will repeat its emission")]
    public bool repeat
    {
        get
        {
            return (bool)WTBObject.Components.ComponentByName("Particles").PropertyByName("Enabled").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Enabled").SetValue(value);
        }
    }

    [BluaProperty(description = "The range of this {object}")]
    public float range
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Range").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Range").SetValue(value);
        }
    }

    [BluaProperty(description = "The affect that gravity has on each particle. 1 is normal gravity")]
    public float gravity
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Gravity").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Gravity").SetValue(value);
        }
    }

    [BluaProperty(description = "The speed of each particle")]
    public float speed
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Speed").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Speed").SetValue(value);
        }
    }

    [BluaProperty(description = "How long each particle lasts")]
    public float runtime
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Run Time").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Run Time").SetValue(value);
        }
    }

    [BluaProperty(description = "The amount of particles from this {object}")]
    public float amount
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Amount").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Amount").SetValue(value);
        }
    }

    [BluaProperty(description = "The spray size")]
    public float spraysize
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Spray Size").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Spray Size").SetValue(value);
        }
    }

    [BluaProperty(description = "The angle that particles come out of this {object} from")]
    public float sprayangle
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Spray Angle").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Spray Angle").SetValue(value);
        }
    }

    [BluaProperty(description = "The starting size of each particle")]
    public float startsize
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("Start Size").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Start Size").SetValue(value);
        }
    }

    [BluaProperty(description = "The ending size of each particle")]
    public float endsize
    {
        get
        {
            return (float)WTBObject.Components.ComponentByName("Particles").PropertyByName("End Size").GetValue();
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("End Size").SetValue(value);
        }
    }

    [BluaProperty(description = "The starting color of each particle")]
    public Color startcolor
    {
        get
        {
            Color color = new Color(-1, -1, -1, -1);
            color = (Color)WTBObject.Components.ComponentByName("Particles").PropertyByName("Start Color").GetValue();
            return color;
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("Start Color").SetValue((Color)value);
        }
    }

    [BluaProperty(description = "The ending color of each particle")]
    public Color endcolor
    {
        get
        {
            Color color = new Color(-1, -1, -1, -1);
            color = (Color)WTBObject.Components.ComponentByName("Particles").PropertyByName("End Color").GetValue();
            return color;
        }
        set
        {
            WTBObject.Components.ComponentByName("Particles").PropertyByName("End Color").SetValue((Color)value);
        }
    }

    [BluaMethod(description = "Creates a particle from this {object}", scriptSide = ScriptSide.Any)]
    public void CreateParticle(Vector3? position = null, Vector3? velocity = null, int count = 1)
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = WTBObject.GameObject.transform.position;
        if (position != null) emitParams.position = (Vector3)position;
        emitParams.velocity = Vector3.zero;
        if (velocity != null) emitParams.velocity = (Vector3)velocity;

        WTBObject.GameObject.GetComponentInChildren<ParticleSystem>().Emit(emitParams, count);
    }
    
    [BluaMethod(description = "Clears the particles that are active that came from this {object}", scriptSide = ScriptSide.Any)]
    public void Clear()
    {
        WTBObject.GameObject.GetComponentInChildren<ParticleSystem>().Clear();
    }
}