using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IDataHandle
{
    public JObject toJObject();
    public JObject getDefaultJObject();
    public void fromJObject(JObject jo);

}
