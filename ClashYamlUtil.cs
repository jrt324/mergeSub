using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ClashSubConvert;

public class ClashYamlUtil
{
    static readonly HttpClient client = new();
    
    public static async Task<string> ConvertUrl(string subscribeUrl, List<ServerNode> serverNodes)
    {
        Console.WriteLine("subscribeUrl: " + subscribeUrl);
        string yamlStr = await client.GetStringAsync(subscribeUrl);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var dictionary = deserializer.Deserialize<Dictionary<string, object>>(yamlStr);

        foreach (var server in serverNodes)
        {
            var groups = server.ProxyGroupNames;
            Console.WriteLine("get remote subscribe url success!");
            Console.WriteLine("process append server node:" + server.ServerConfig);
            
            var addServerStr = server.ServerConfig;
            if (server.Base64Flag)
            {
                addServerStr = Encoding.UTF8.GetString(Convert.FromBase64String(server.ServerConfig));
            }
            var proxies = dictionary["proxies"] as List<object>;
            var addServerYaml = deserializer.Deserialize<Dictionary<string, object>>(addServerStr);
            proxies.Add(addServerYaml);
            var proxyGroups = dictionary["proxy-groups"] as List<object>;
            var targetGroups = proxyGroups.Where(g =>
            {
                var group = g as Dictionary<object, object>;
                if (group.TryGetValue("name", out var val))
                {
                    return groups.Contains(val);
                }
                return false;
            }).ToList();

            if (targetGroups.Count > 0)
            {
                foreach (var targetGroup in targetGroups)
                {
                    var targetGroupDict = targetGroup as Dictionary<object, object>;
                    var groupProxies = targetGroupDict?["proxies"] as List<object>;
                    groupProxies?.Add(addServerYaml["name"]);
                }
            }
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithIndentedSequences()
            .Build();
        var output = serializer.Serialize(dictionary);
        Console.WriteLine("convert success!");
        return output;
    }
}