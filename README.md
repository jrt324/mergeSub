# ClashSubConvert
Add proxy server to an existing subscription link , and generate a new subscription link.
subUrl: Orignal subscribe url
addServer: The proxy server using base64 encode
toRule: Append to proxy groups , Use commas to separate
http://localhost:8081/covert?subUrl={subUrl}&addServer={addServer}&toRule=Proxy,OpenAI
