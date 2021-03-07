using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using static EnumHelper;

//Zack Pilgrim + Daniel Bibby
//
//Intention is to have all variables stored and referenced through dicts. Dicts must be converted to pairs of lists to be serialised.
//Vector2/3 are non-serialisable, hence the custom structs.
//Current variable data types are only a baseline. More can be added, possibly including storing entire lists.

public static class SaveManager
{
    public static string fileType = ".LEMON";
    public static string defaultSavePath = Application.persistentDataPath + "/GameSave" + fileType;

    public static Save saveData = new Save();

    public static Dictionary<string, int> intDict = new Dictionary<string, int>();
    public static Dictionary<string, float> floatDict = new Dictionary<string, float>();
    public static Dictionary<string, bool> boolDict = new Dictionary<string, bool>();
    public static Dictionary<string, string> stringDict = new Dictionary<string, string>();
    public static Dictionary<string, Vector2> vector2Dict = new Dictionary<string, Vector2>();
    public static Dictionary<string, Vector3> vector3Dict = new Dictionary<string, Vector3>();
    
    public static Dictionary<string, ElementTypes> elementDict = new Dictionary<string, ElementTypes>();

    public static void SaveToFile(string path="")
    {
        if (path == "") path = defaultSavePath;
        else if (!path.Contains(fileType)) path += fileType;

        PlayerPrefs.Save();

        SaveDictsToLists();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, saveData);
        file.Close();
    }

    public static bool LoadFromFile(string path = "")
    {
        if (path == "") path = defaultSavePath;
        else if (!path.Contains(fileType)) path += fileType;

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(defaultSavePath, FileMode.Open);
            saveData = (Save)bf.Deserialize(file);
            file.Close();
            LoadDictsFromLists();

            return true;
        }

        return false;
    }

    public static void PrintOverrideWarning(string key, string dataType)
    {
        dataType = dataType[0].ToString().ToUpper() + dataType.Substring(1);
        Debug.LogWarning(dataType + " value for " + key + " already exists. Use 'UpdateSaved" + dataType + "()' to replace it.");
    }

    public static void PrintMissingDataWarning(string key, string dataType)
    {
        dataType = dataType[0].ToString().ToUpper() + dataType.Substring(1);
        Debug.LogWarning("No " + dataType + " value for " + key + " exists. Use 'AddNew" + dataType + "()' to create it.");
    }

    #region AddData
    public static bool AddNewInt(string key, int value)
    {
        if (intDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Int");
            return false;
        }
        else intDict.Add(key, value);
        return true;    
    }

    public static bool AddNewFloat(string key, float value)
    {
        if (floatDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Float");
            return false;
        }
        else floatDict.Add(key, value);
        return true;
    }

    public static bool AddNewBool(string key, bool value)
    {
        if (boolDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Bool");
            return false;
        }
        else boolDict.Add(key, value);
        return true;
    }

    public static bool AddNewString(string key, string value)
    {
        if (stringDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "String");
            return false;
        }
        else stringDict.Add(key, value);
        return true;
    }

    public static bool AddNewVector2(string key, Vector2 value)
    {
        if (vector2Dict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Vector2");
            return false;
        }
        else vector2Dict.Add(key, value);
        return true;
    }
    public static bool AddNewVector3(string key, Vector3 value)
    {
        if (vector3Dict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Vector3");
            return false;
        }
        else vector3Dict.Add(key, value);
        return true;
    }

    public static bool AddNewElementType(string key, ElementTypes value)
    {
        if (elementDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "ElementType");
            return false;
        }
        else elementDict.Add(key, value);
        return true;
    }
    #endregion
    #region UpdateData
    public static void UpdateSavedInt(string key, int value)
    {
        intDict[key] = value;
    }

    public static void UpdateSavedFloat(string key, float value)
    {
        floatDict[key] = value;
    }

    public static void UpdateSavedBool(string key, bool value)
    {
        boolDict[key] = value;
    }

    public static void UpdateSavedString(string key, string value)
    {
        stringDict[key] = value;
    }

    public static void UpdateSavedVector2(string key, Vector2 value)
    {
        vector2Dict[key] = value;
    }

    public static void UpdateSavedVector3(string key, Vector3 value)
    {
        vector3Dict[key] = value;
    }

    public static void UpdateSavedElementType(string key, ElementTypes value)
    {
        elementDict[key] = value;
    }
    #endregion
    #region GetData
    public static int GetInt(string key)
    {
        if (intDict.ContainsKey(key)) return intDict[key];
        else PrintMissingDataWarning(key, "int");
        
        return 0;
    }

    public static float GetFloat(string key)
    {
        if (floatDict.ContainsKey(key)) return floatDict[key];
        else PrintMissingDataWarning(key, "float");
        
        return 0.0f;
    }

    public static bool GetBool(string key)
    {
        if (stringDict.ContainsKey(key)) return boolDict[key];
        else PrintMissingDataWarning(key, "bool");
        
        return false;
    }

    public static string GetString(string key)
    {
        if (stringDict.ContainsKey(key)) return stringDict[key];
        else PrintMissingDataWarning(key, "string");
        
        return "";
    }

    public static Vector2 GetVector2(string key)
    {
        if (vector2Dict.ContainsKey(key)) return vector2Dict[key];
        else PrintMissingDataWarning(key, "Vector2");
        
        return Vector2.zero;
    }

    public static Vector3 GetVector3(string key)
    {
        if (vector3Dict.ContainsKey(key)) return vector3Dict[key];
        else PrintMissingDataWarning(key, "Vector3");
         
        return Vector3.zero;
    }

    public static ElementTypes GetElementType(string key)
    {
        if (elementDict.ContainsKey(key)) return elementDict[key];
        else PrintMissingDataWarning(key, "ElementType");

        return ElementTypes.ElementTypesSize;
    }
    #endregion

    public static bool HasInt(string key) { return saveData.intKeys.Contains(key); }
    public static bool HasFloat(string key) { return saveData.floatKeys.Contains(key); }
    public static bool HasBool(string key) { return saveData.boolKeys.Contains(key); }
    public static bool HasString(string key) { return saveData.stringKeys.Contains(key); }
    public static bool HasVector2(string key) { return saveData.vector2Keys.Contains(key); }
    public static bool HasVector3(string key) { return saveData.vector3Keys.Contains(key); }
    public static bool HasElelmentType(string key) { return saveData.elementKeys.Contains(key); }

    public static void SaveDictsToLists()
    {
        int dictIndex = 0;
        foreach (KeyValuePair<string, int> pair in intDict)
        {
            if (saveData.intKeys.Contains(pair.Key))
            {
                saveData.intKeys[dictIndex] = pair.Key;
                saveData.intValues[dictIndex] = pair.Value;
            }
            else
            {
                saveData.intKeys.Add(pair.Key);
                saveData.intValues.Add(pair.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, float> pair in floatDict)
        {
            if (saveData.floatKeys.Contains(pair.Key))
            {
                saveData.floatKeys[dictIndex] = pair.Key;
                saveData.floatValues[dictIndex] = pair.Value;
            }
            else
            {
                saveData.floatKeys.Add(pair.Key);
                saveData.floatValues.Add(pair.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, bool> pair in boolDict)
        {
            if (saveData.boolKeys.Contains(pair.Key))
            {
                saveData.boolKeys[dictIndex] = pair.Key;
                saveData.boolValues[dictIndex] = pair.Value;
            }
            else
            {
                saveData.boolKeys.Add(pair.Key);
                saveData.boolValues.Add(pair.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, string> pair in stringDict)
        {
            if (saveData.stringKeys.Contains(pair.Key))
            {
                saveData.stringKeys[dictIndex] = pair.Key;
                saveData.stringValues[dictIndex] = pair.Value;
            }
            else
            {
                saveData.stringKeys.Add(pair.Key);
                saveData.stringValues.Add(pair.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, Vector2> pair in vector2Dict)
        {
            if (saveData.vector2Keys.Contains(pair.Key))
            {
                saveData.vector2Keys[dictIndex] = pair.Key;
                saveData.vector2Values[dictIndex] = new myVector2(pair.Value.x, pair.Value.y);
            }
            else
            {
                saveData.vector2Keys.Add(pair.Key);
                saveData.vector2Values.Add(new myVector2(pair.Value.x, pair.Value.y));
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, Vector3> pair in vector3Dict)
        {
            if (saveData.vector3Keys.Contains(pair.Key))
            {
                saveData.vector3Keys[dictIndex] = pair.Key;
                saveData.vector3Values[dictIndex] = new myVector3(pair.Value.x, pair.Value.y, pair.Value.z);
            }
            else
            {
                saveData.vector3Keys.Add(pair.Key);
                saveData.vector3Values.Add(new myVector3(pair.Value.x, pair.Value.y, pair.Value.z));
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, ElementTypes> pair in elementDict)
        {
            if (saveData.elementKeys.Contains(pair.Key))
            {
                saveData.elementKeys[dictIndex] = pair.Key;
                saveData.elementValues[dictIndex] = pair.Value;
            }
            else
            {
                saveData.elementKeys.Add(pair.Key);
                saveData.elementValues.Add(pair.Value);
            }
            dictIndex++;
        }
    }

    public static void LoadDictsFromLists()
    {
        for (int index = 0; index < saveData.intKeys.Count; index++) intDict[saveData.intKeys[index]] = saveData.intValues[index];
        for (int index = 0; index < saveData.floatKeys.Count; index++) floatDict[saveData.floatKeys[index]] = saveData.floatValues[index];
        for (int index = 0; index < saveData.boolKeys.Count; index++) boolDict[saveData.boolKeys[index]] = saveData.boolValues[index];
        for (int index = 0; index < saveData.stringKeys.Count; index++) stringDict[saveData.stringKeys[index]] = saveData.stringValues[index];
        for (int index = 0; index < saveData.vector2Keys.Count; index++)
            vector2Dict[saveData.vector2Keys[index]] = new Vector2(saveData.vector2Values[index].x, saveData.vector2Values[index].y);

        for (int index = 0; index < saveData.vector3Keys.Count; index++)
            vector3Dict[saveData.vector3Keys[index]] = new Vector3(saveData.vector3Values[index].x, saveData.vector3Values[index].y, saveData.vector3Values[index].z);

        for (int index = 0; index < saveData.elementKeys.Count; index++) elementDict[saveData.elementKeys[index]] = saveData.elementValues[index];
    }
}

[Serializable]
public class Save
{
    public List<string> intKeys = new List<string>();
    public List<int> intValues = new List<int>();

    public List<string> floatKeys = new List<string>();
    public List<float> floatValues = new List<float>();
    
    public List<string> boolKeys = new List<string>();
    public List<bool> boolValues = new List<bool>();

    public List<string> stringKeys = new List<string>();
    public List<string> stringValues = new List<string>();

    public List<string> vector2Keys = new List<string>();
    public List<myVector2> vector2Values = new List<myVector2>();

    public List<string> vector3Keys = new List<string>();
    public List<myVector3> vector3Values = new List<myVector3>();

    public List<string> elementKeys = new List<string>();
    public List<ElementTypes> elementValues = new List<ElementTypes>();

    //On creating new lists, either delete and replace saved file, or add function to check for null lists and create new instances.
}


[Serializable] 
public struct myVector2
{
    public float x, y;

    public myVector2(float nx, float ny)
    {
        x = nx;
        y = ny;
    }
}

[Serializable]
public struct myVector3
{
    public float x, y, z;

    public myVector3(float nx, float ny, float nz)
    {
        x = nx;
        y = ny;
        z = nz;
    }
}
