using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ClashSubConvert;

public class ClashYaml
{
    static readonly HttpClient client = new();

    public static async Task<string> ConvertClash(string subscribeUrl, string serverConfig, string proxyGroupName)
    {
        string yamlStr = await client.GetStringAsync(subscribeUrl);

        // base64 decode addServer param
        var addServerStr = Encoding.UTF8.GetString(Convert.FromBase64String(serverConfig));
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var dictionary = deserializer.Deserialize<Dictionary<string, object>>(yamlStr);
        var proxies = dictionary["proxies"] as List<object>;
        var addServerYaml = deserializer.Deserialize<Dictionary<string, object>>(addServerStr);
        proxies.Add(addServerYaml);

        var proxyGroups = dictionary["proxy-groups"] as List<object>;
        var targetGroup = proxyGroups.Where(g =>
        {
            var group = g as Dictionary<object, object>;
            if (group.TryGetValue("name", out var val))
            {
                return (string) val == proxyGroupName;
            }
            return false;
        }).FirstOrDefault();

        if (targetGroup != null)
        {
            var targetGroupDict = targetGroup as Dictionary<object, object>;
            var groupProxies = targetGroupDict?["proxies"] as List<object>;
            groupProxies?.Add(addServerYaml["name"]);
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var output = serializer.Serialize(dictionary);

        Console.WriteLine(output);
        return output;
    }
}