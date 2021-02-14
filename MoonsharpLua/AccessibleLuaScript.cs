using MoonSharp.Interpreter;

// this allows us to communicate between scripts
public class AccessibleLuaScript
{
    public AccessibleLuaScript(Script _script, string _name)
    {
        script = _script;
        scriptName = _name;
    }

    [MoonSharpHidden]
    private Script script; // MoonSharp Script

    private string scriptName;

    public DynValue Call(string _functionName, params DynValue[] _args)
    {
        object function = script.Globals[_functionName];

        return script.Call(function, _args);
    }

    public Table Globals
    {
        get
        {
            return script.Globals;
        }
    }

    public object this[string name]
    {
        get { return script.Globals[name]; }
        set { script.Globals[name] = value; }
    }
}
