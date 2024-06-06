using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ClashSubConvert;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.MapGet("/covert", async (string subUrl, string[] addServer, string[] toRule) =>
{
    Console.WriteLine("call covert");
    if (addServer.Length != toRule.Length && toRule.Length != 1)
    {
        return "addServer and toRule length not match";
    }
    List<ServerNode> serverNodes = new();
    for (int i = 0; i < addServer.Length; i++)
    {
        var serverNode = new ServerNode
        {
            ServerConfig = addServer[i],
            ProxyGroupNames =
                (toRule.Length == 1 ? toRule[0] : toRule[i]).Split(",", StringSplitOptions.RemoveEmptyEntries)
        };
        serverNodes.Add(serverNode);
    }
    var yaml = await ClashYamlUtil.ConvertUrl(subUrl, serverNodes);
    return yaml;
}).WithOpenApi();

app.MapGet("/convert1", async (string subUrl, string proxies, string groups) =>
{
    Console.WriteLine("call covert");
    var proxiesStr = Encoding.UTF8.GetString(Convert.FromBase64String(proxies));
    var groupsStr = Encoding.UTF8.GetString(Convert.FromBase64String(groups));
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    var addProxies = JsonSerializer.Deserialize<JsonObject[]>(proxiesStr);
    var toGroups = JsonSerializer.Deserialize<List<GroupItem>>(groupsStr,options);
    if (addProxies == null || addProxies.Length == 0)
    {
        return "addProxies is empty";
    }
    if (toGroups == null || toGroups.Count == 0)
    {
        return "toRules is empty";
    }
    List<ServerNode> serverNodes = new();
    for (int i = 0; i < addProxies.Length; i++)
    {
        var proxy = addProxies[i];
        var serverNode = new ServerNode
        {
            ServerConfig = proxy.ToJsonString(),
            Base64Flag = false,
            ProxyGroupNames = toGroups
                .Where(r => r.Proxies.Contains(proxy["name"].ToString()))
                .Select(g => g.Name)
                .Distinct().ToArray()
        };
        serverNodes.Add(serverNode);
    }
    var yaml = await ClashYamlUtil.ConvertUrl(subUrl, serverNodes);
    return yaml;
}).WithOpenApi();
app.MapGet("/test", () => "Hello World!").WithOpenApi();
app.Run();

public class GroupItem
{
    /// <summary>
    /// group name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 代理服务名称
    /// </summary>
    public List<string> Proxies { get; set; }
}