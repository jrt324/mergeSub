# mergeSub
### **Add proxy servers to an existing subscription link ,  generate a new subscription link.**



http://localhost:8081/covert?subUrl={subUrl}&proxies={proxies}&groups={groups}

<u>Please replace the corresponding parameters in the URL</u>

- **subUrl**: Orignal subscribe url
- **proxies**: The proxy server list, using **base64** encode
  
  Json示例：
  ```json
    [
      {"name":"vps-ss","server":"22.11.22.44","port":49293,"type":"ss","password":"123","cipher":"aes-128-gcm"}
      {"name":"vps-ss-proxy","server":"22.11.22.55","port":19968,"type":"ss","password":"123","cipher":"aes-128-gcm"}
    ]
  ```
- **groups**: Proxies Append to groups , using **base64** encode
  
  Json示例：
  ```json
      [
        {
        "name":"Proxy",
        "Proxies":["vps-ss","vps-ss-proxy"]
        },
        {
        "name":"OpenAI",
        "Proxies":["vps-ss"]
        }
      ]
  ```


