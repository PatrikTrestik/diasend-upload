# Build
* NSwag code generation sometimes fails to regenerate after yaml change. Run rebuild for sure.

# Testing
Diasend servers does not send CORS. If testing manually you have to disable CORS security requirement.

'chrome.exe --disable-web-security --disable-gpu'

## Setup

dotnet user-secrets set ApiUserName <your Dasend user>
  
dotnet user-secrets set ApiPassword <your password>
