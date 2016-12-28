# DotNetCore.CmdArgParser

Very easy to use and feature rich command line argument parser.

## Installation

Using nuget:
**Install-Package PeterKottas.DotNetCore.CmdArgParser**

## Usage

1. Using statement:
```cs
using PeterKottas.DotNetCore.CmdArgParser;
```
2. Api for parsing:
```cs
CmdArgParser.Parse(config =>{
});
```
3. Add your first parsing parameter
```cs
CmdArgParser.Parse(config =>{
  config.AddParameter(new CmdArgParam()
    {
      Key = "1stParamKey",
      Description = "This description is used for help",
      Value = val =>
      {
        innerConfig.Username = val; //What happens when we find this key, callback with injected value
      }
  });
});
```
4. Advanced example
```cs
CmdArgParser.Parse(config =>{
  config.AddParameter(new CmdArgParam()
    {
      Key = "1stParamKey",
      Description = "This description is used for help",
      Value = val =>
      {
        switch(val){
          case "a":
            break;
          case "b":
            break
          default:
            Console.WriteLine("Only \"a\" and \"b\" are allowed values for key \"1stParamKey\". {0} was provided instead." val);
            break;
        }
      }
  });
});
```
5. Parser api
```cs
CmdArgParser.Parse(config =>{
  config.UseDefaultHelp();//Configures a "help" parameter which displays default help
  config.UseAppDescription();//Configures app description used in help
  config.ShowHelpOnExtraArguments();//Show's extra arguments
  config.CustomHelp(helpData=>
  {
  });//Configures custom help. You can provide a function that displays custom help for you application. You get the array of parameters provided to you via parameter HelpData
  config.DisplayHelp();//Displays help. Useful inside parameter callbacks.
});
```

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## License

MIT 
