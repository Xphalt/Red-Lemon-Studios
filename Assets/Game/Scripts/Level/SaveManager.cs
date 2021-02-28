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
        Debug.LogWarning(dataType.ToUpper() + " value for " + key + " already exists. Use 'UpdateSaveData()' to replace it.");
    }

    public static void PrintMissingDataWarning(string key, string dataType)
    {
        Debug.LogWarning("No " + dataType.ToUpper() + " value for " + key + " exists. Use 'AddNewData()' to create it.");
    }

    #region AddData
    public static bool AddNewData(string key, int value)
    {
        if (intDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Int");
            return false;
        }
        else intDict.Add(key, value);
        return true;    
    }

    public static bool AddNewData(string key, float value)
    {
        if (floatDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Float");
            return false;
        }
        else floatDict.Add(key, value);
        return true;
    }

    public static bool AddNewData(string key, bool value)
    {
        if (boolDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Bool");
            return false;
        }
        else boolDict.Add(key, value);
        return true;
    }

    public static bool AddNewData(string key, string value)
    {
        if (stringDict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "String");
            return false;
        }
        else stringDict.Add(key, value);
        return true;
    }

    public static bool AddNewData(string key, Vector2 value)
    {
        if (vector2Dict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Vector2");
            return false;
        }
        else vector2Dict.Add(key, value);
        return true;
    }
    public static bool AddNewData(string key, Vector3 value)
    {
        if (vector3Dict.ContainsKey(key))
        {
            PrintOverrideWarning(key, "Vector3");
            return false;
        }
        else vector3Dict.Add(key, value);
        return true;
    }
    #endregion
    #region UpdateData
    public static void UpdateSaveData(string key, int value)
    {
        intDict[key] = value;
    }

    public static void UpdateSaveData(string key, float value)
    {
        floatDict[key] = value;
    }

    public static void UpdateSaveData(string key, bool value)
    {
        boolDict[key] = value;
    }

    public static void UpdateSaveData(string key, string value)
    {
        stringDict[key] = value;
    }

    public static void UpdateSaveData(string key, Vector2 value)
    {
        vector2Dict[key] = value;
    }

    public static void UpdateSaveData(string key, Vector3 value)
    {
        vector3Dict[key] = value;
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
    #endregion

    public static void SaveDictsToLists()
    {
        int dictIndex = 0;
        foreach (KeyValuePair<string, int> element in intDict)
        {
            if (saveData.intKeys.Contains(element.Key))
            {
                saveData.intKeys[dictIndex] = element.Key;
                saveData.intValues[dictIndex] = element.Value;
            }
            else
            {
                saveData.intKeys.Add(element.Key);
                saveData.intValues.Add(element.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, float> element in floatDict)
        {
            if (saveData.floatKeys.Contains(element.Key))
            {
                saveData.floatKeys[dictIndex] = element.Key;
                saveData.floatValues[dictIndex] = element.Value;
            }
            else
            {
                saveData.floatKeys.Add(element.Key);
                saveData.floatValues.Add(element.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, bool> element in boolDict)
        {
            if (saveData.boolKeys.Contains(element.Key))
            {
                saveData.boolKeys[dictIndex] = element.Key;
                saveData.boolValues[dictIndex] = element.Value;
            }
            else
            {
                saveData.boolKeys.Add(element.Key);
                saveData.boolValues.Add(element.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, string> element in stringDict)
        {
            if (saveData.stringKeys.Contains(element.Key))
            {
                saveData.stringKeys[dictIndex] = element.Key;
                saveData.stringValues[dictIndex] = element.Value;
            }
            else
            {
                saveData.stringKeys.Add(element.Key);
                saveData.stringValues.Add(element.Value);
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, Vector2> element in vector2Dict)
        {
            if (saveData.vector2Keys.Contains(element.Key))
            {
                saveData.vector2Keys[dictIndex] = element.Key;
                saveData.vector2Values[dictIndex] = new myVector2(element.Value.x, element.Value.y);
            }
            else
            {
                saveData.vector2Keys.Add(element.Key);
                saveData.vector2Values.Add(new myVector2(element.Value.x, element.Value.y));
            }
            dictIndex++;
        }

        dictIndex = 0;
        foreach (KeyValuePair<string, Vector3> element in vector3Dict)
        {
            if (saveData.vector3Keys.Contains(element.Key))
            {
                saveData.vector3Keys[dictIndex] = element.Key;
                saveData.vector3Values[dictIndex] = new myVector3(element.Value.x, element.Value.y, element.Value.z);
            }
            else
            {
                saveData.vector3Keys.Add(element.Key);
                saveData.vector3Values.Add(new myVector3(element.Value.x, element.Value.y, element.Value.z));
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
