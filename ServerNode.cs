namespace ClashSubConvert;

public class ServerNode
{
    public string ServerConfig { get; set; }
    public string[] ProxyGroupNames { get; set; }

    internal bool Base64Flag { get; set; } = true;
}