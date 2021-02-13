using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using System.IO;
using System.Text;

[BluaClass]
public class LuaFile
{
    [MoonSharpHidden]
    string mainPath = "";

    public LuaFile()
    {
        mainPath = Application.persistentDataPath + "/" + Launcher.WORLDID;

        //check if directory doesn't exit
        if (!Directory.Exists(mainPath))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(mainPath);

        }
    }

    [BluaMethod]
    public bool Exists(string filename)
    {
        return File.Exists(GetPathFor(filename));
    }

    [BluaMethod]
    public void Write(string filename, string stringToWrite)
    {
        byte[] saveDataRaw = Encoding.UTF8.GetBytes(stringToWrite);
        File.WriteAllBytes( GetPathFor(filename), saveDataRaw);
    }

    [BluaMethod]
    public void WriteCompressed(string filename, string stringToWrite)
    {
        byte[] saveDataRaw = Encoding.UTF8.GetBytes(stringToWrite);
        File.WriteAllBytes(GetPathFor(filename), saveDataRaw.CompressBytes());
    }

    [BluaMethod]
    public string Read(string filename)
    {
        string filePath = GetPathFor(filename);

        if (!File.Exists(filePath))
        {
            return "File not found: " + filename;
        }

        byte[] rawFile = File.ReadAllBytes(filePath);
        string bytesToString = Encoding.UTF8.GetString(rawFile);

        return bytesToString;
    }

    [BluaMethod]
    public string ReadCompressed(string filename)
    {
        string filePath = GetPathFor(filename);

        if (!File.Exists(filePath))
        {
            return "File not found: " + filename;
        }

        return File.ReadAllBytes(filePath).DecompressToString();
    }

    [MoonSharpHidden] string GetPathFor(string _filename)
    {
        string filePath = mainPath + "/" + _filename + ".txt";

        // don't let them go backwards until we know it's safe
        if (filePath.Contains("../"))
        {
            filePath.Replace("../", "");
            Debug.Log("../ is not allowed in lua File.Write names. Removing ../ instances in: '" + _filename + "'");
        }

        return filePath;
    }
}