# How to use these examples

1. Make sure that you have Visual Studio Code installed
2. Optional: [set up synchronization](https://marketplace.visualstudio.com/items?itemName=Shan.code-settings-sync) of your settings so you will only need to go through the steps below once in a lifetime ;)
3. Install [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension (`humao.rest-client`)
4. In VS Code, go to `File -> Preferences -> Settings -> Open Settings (JSON)`
5. Define your environments, you can use the following starter code for convenience:

```json
{
  // your current settings...
  "editor.wordWrap": "on",

  // add this section:
  "rest-client.environmentVariables": {
    "local environment": {
      "baseUrlWeatherService": "https://localhost:6001/"
    }
  }
}
```
