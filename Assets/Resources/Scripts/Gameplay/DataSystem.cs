using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;

public class DataSystem : MonoBehaviour
{
    static string DATA_VERSION = "dev-1F";
  
    static JObject DEFAULT_DATA = new JObject() {
        {"version",DATA_VERSION},
        {"datatest", 125}
    };

    internal static DataSystem instance;

    JObject data;

    void Start()
    {
        if (FindObjectsByType<DataSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

        instance = this;

    }


    /// <summary>
    /// Get the raw data (string)
    /// </summary>
    /// <param name="savename"> the name of save</param>
    /// <returns>a string that contain all data</returns>
    public static string GetRawData(string savename)
    {
        if (File.Exists(GetPathFromName(savename))) return File.ReadAllText(GetPathFromName(savename));
        else return "ERR FILE NOT FOUND";
    }

    /// <summary>
    /// Set a data into the save with a datapath. The path is something like : "my/path/to/JTokenPlace"
    /// </summary>
    /// <param name="datapath"> the path into the architecture. something like : "my/path/to/JTokenPlace" </param>
    /// <param name="value"> the value </param>
    internal void SetData(string datapath, object value)
    {
        JToken jt = data.SelectToken(datapath);
        JToken newJT = JToken.FromObject(value);
        jt.Replace(newJT);
    }

    /// <summary>
    /// Get a value from the data with a datapath. The path is something like : "my/path/to/JTokenPlace"
    /// </summary>
    /// <typeparam name="T"> type of the value</typeparam>
    /// <param name="datapath"> the path into the architecture. something like : "my/path/to/JTokenPlace" </param>
    /// <returns> the value </returns>
    internal T GetData<T>(string datapath)
    {
        JToken jt = data.SelectToken(datapath);
        if (jt == null) return default(T);
        else return jt.ToObject<T>();
    }

    /// <summary>
    /// Save the data into file
    /// </summary>
    /// <param name="saveName"> the name of save </param>
    internal void Save(string saveName)
    {
        File.WriteAllText(GetPathFromName(saveName), data.ToString());
    }

    /// <summary>
    /// Load the data from file
    /// </summary>
    /// <param name="saveName"> the name of the save </param>
    internal void Load(string saveName)
    {
        if (File.Exists(GetPathFromName(saveName)))
        {
            string rawData = GetRawData(saveName);
            data = JObject.Parse(rawData);
            CheckVersion(ref data);
        }
        else ResetData(saveName);
    }

    /// <summary>
    /// Reset all data to default
    /// </summary>
    internal void ResetData(string saveName)
    {
        data = new JObject(DEFAULT_DATA);
        Save(saveName);
    }

    /// <summary>
    /// Check and return the data updated to the actual data version
    /// </summary>
    /// <param name="dataToCheck"> data to check </param>
    /// <returns> the data updated to the actual data version </returns>
    internal void CheckVersion(ref JObject dataToCheck)
    {
        print($"need checking:{dataToCheck.ToString()}");
        if(dataToCheck["version"].ToString() == DATA_VERSION) return; // same version ; dont touch

        // need update (add new field to the data)
        foreach(KeyValuePair<string,JToken> obj in DEFAULT_DATA) {
            UpdateThisJO(obj);
        }

        dataToCheck["version"].Replace(DEFAULT_DATA["version"]);

    }

    void UpdateThisJO(KeyValuePair<string, JToken> pair) {
        JToken defToken = pair.Value;
        JToken inData   = data.SelectToken(defToken.Path);
        print($"inData(path={defToken.Path}):{inData}");

        // check if this container / token exist
        if (inData == null)
        {
            InsertDefaultAt(pair);
            return;
        }

        if (inData.Type != JTokenType.Object) return;

        // Update all child
        foreach (KeyValuePair<string, JToken> obj in pair.Value.ToObject<JObject>())
        {
            UpdateThisJO(obj);
        }
        return;

    }

    void InsertDefaultAt(KeyValuePair<string, JToken> toAdd)
    {
        string parentPath = toAdd.Value.Parent.Parent.Path;

        print($"need adding: {toAdd.Value.Parent.Parent.Path} pair key:{toAdd.Key}  pair value:{toAdd.Value}");
        print($"parent path: {parentPath}");

        JToken dataParent = parentPath == "" ? data : data.SelectToken(parentPath);
        print($"dataParent: {dataParent}");

        dataParent.Parent.Add(new JProperty(toAdd.Key, toAdd.Value));

    }

    static string GetPathFromName(string dataname)
    {
        return $"{Application.persistentDataPath}/{dataname}.dat";
    }


}
