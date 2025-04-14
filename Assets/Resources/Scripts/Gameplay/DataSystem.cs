using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;

public class DataSystem : MonoBehaviour
{
    static string DATA_VERSION = "1.0";
    static JObject DEFAULT_DATA = new JObject() {
        {"version",DATA_VERSION}
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
    /// <param name="dataFilePath"> where to save </param>
    internal void Save(string dataFilePath)
    {
        File.WriteAllText(dataFilePath, data.ToString());
    }

    /// <summary>
    /// Load the data from file
    /// </summary>
    /// <param name="dataFilePath"> where is the file </param>
    internal void Load(string dataFilePath)
    {
        if (File.Exists(dataFilePath))
        {
            string rawData = File.ReadAllText(dataFilePath);
            data = JObject.Parse(rawData);
        }
        else ResetData();
    }

    /// <summary>
    /// Reset all data to default
    /// </summary>
    internal void ResetData()
    {
        data = new JObject(DEFAULT_DATA);
    }

    /// <summary>
    /// Check and return the data updated to the actual data version
    /// </summary>
    /// <param name="dataToCheck"> data to check </param>
    /// <returns> the data updated to the actual data version </returns>
    internal JObject CheckVersion(ref JObject dataToCheck)
    {
        if(dataToCheck["version"].ToString() == DATA_VERSION)
        {
            return dataToCheck; // same version ; dont touch
        }

        // need update (add new field to the data)
        foreach(KeyValuePair<string,JToken> obj in DEFAULT_DATA) {
            UpdateThisJO(obj);
        }

        return dataToCheck;

    }

    void UpdateThisJO(KeyValuePair<string, JToken> pair) {
        JToken defToken = pair.Value;
        JToken inData   = data.SelectToken(defToken.Path);
        print($"inData:{inData}");

        switch (defToken.Type)
        {
            case JTokenType.Object:
                // check if this container exist
                if (inData == null)
                {
                    InsertDefaultAt(pair);
                }

                // Update all child
                foreach (KeyValuePair<string, JToken> obj in DEFAULT_DATA)
                {
                    UpdateThisJO(obj);
                }
                return;

            case JTokenType.Array:
                // check if this container exist
                if (inData == null)
                {
                    InsertDefaultAt(pair);
                }

                // Update all child
                foreach (KeyValuePair<string, JToken> obj in DEFAULT_DATA)
                {
                    UpdateThisJO(obj);
                }
                return;

            default:
                //check if this data exist into the data at the same place
                if (inData == null)
                {
                    InsertDefaultAt(pair);
                }
                break;

        }
    }

    void InsertDefaultAt(KeyValuePair<string, JToken> toAdd)
    {
        JObject jo = data.SelectToken(toAdd.Value.Parent.Path).ToObject<JObject>();
        if (jo == null) return; // no parent
        jo.Add(toAdd.Key, toAdd.Value);
    }

}
