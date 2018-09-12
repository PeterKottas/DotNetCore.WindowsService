# DotNetCore.WindowsService

Simple library that allows one to host dot net core application as windows services. Perfect solution to power micro-services architecture.

## Important note 

This library was created to enable one to host CONSOLE dot net core applications. If you want to host a WEBSITE as a service, you're better of following https://docs.microsoft.com/en-us/aspnet/core/hosting/windows-service

## Installation

Using nuget:
**Install-Package PeterKottas.DotNetCore.WindowsService**

## Quick start

Easiest way to start is using a brand new template. Just do :
```
dotnet new -i PeterKottas.DotNetCore.WindowsService.Templates::*
```
This will add one template at the moment.

Follow up with this
```
mkdir NameOfYourProject
cd NameOfYourProject
dotnet new [ mcrsvc-min | mcrsvc-std ]
```
This will create a sample project for you. Next chapter explains its features in more details especially points 6 onwards if you used the template.

Community, feel encouraged to add more templates if you find something missing/usefull. I'll be more than happy to add these. Just copy the project in https://github.com/PeterKottas/DotNetCore.WindowsService/tree/master/Source/Templates/PeterKottas.DotNetCore.WindowsService.MinimalTemplate and follow instructions in https://github.com/dotnet/templating if you need more specific behvaiour.

## Usage

1. Create .NETCore console app.
	
2. Create your first service, something like this:
	```cs
	public class ExampleService : IMicroService
	{
		public void Start()
		{
			Console.WriteLine("I started");
		}
		
		public void Stop()
		{
			Console.WriteLine("I stopped");
		}
	}
	```
2. You can also inherit MicroService base class and take advantage of built in timers:
	```cs
	public class ExampleService : MicroService, IMicroService
	{
		public void Start()
		{
			this.StartBase();
			Timers.Start("Poller", 1000, () =>
			{
				Console.WriteLine("Polling at {0}\n", DateTime.Now.ToString("o"));
			},
			(e) =>
			{
				Console.WriteLine("Exception while polling: {0}\n", e.ToString());
			});
			Console.WriteLine("I started");
		}
		
		public void Stop()
		{
			this.StopBase();
			Console.WriteLine("I stopped");
		}
	}
	```
3. Api for services (and yeah, it's simmilar to Topshelf, thanks for inspiration, I just couldn't wait for you guys to implement this):
	```cs
	ServiceRunner<ExampleService>.Run(config =>
	{
		var name = config.GetDefaultName();
		config.Service(serviceConfig =>
			{
				serviceConfig.ServiceFactory((extraArguments, microServiceController) =>
			{
				return new ExampleService();
			});
			serviceConfig.OnStart((service, extraArguments) =>
			{
				Console.WriteLine("Service {0} started", name);
				service.Start();
			});

			serviceConfig.OnStop(service =>
			{
				Console.WriteLine("Service {0} stopped", name);
				service.Stop();
			});
			
			serviceConfig.OnInstall(service =>
			{
				Console.WriteLine("Service {0} installed", name);
			});
			
			serviceConfig.OnUnInstall(service =>
			{
				Console.WriteLine("Service {0} uninstalled", name);
			});
			
			serviceConfig.OnPause(service =>
			{
				Console.WriteLine("Service {0} paused", name);
			});
			
			serviceConfig.OnContinue(service =>
			{
				Console.WriteLine("Service {0} continued", name);
			});
			
			serviceConfig.OnShutdown(service =>
			{
				Console.WriteLine("Service {0} shutdown", name);
			});

			serviceConfig.OnError(e =>
			{
				Console.WriteLine("Service {0} errored with exception : {1}", name, e.Message);
			});
		});
	});
	```
4. Optionally set the name of the service like this:
	
	```cs
	ServiceRunner<ExampleService>.Run(config =>
	{
		config.SetName("MyTestService");
	});
	```
5. Run the service without arguments and it runs like console app.
6. Run the service with **action:install** and it will install the service.
7. Run the service with **action:uninstall** and it will uninstall the service.
8. Run the service with **action:start** and it will start the service.
9. Run the service with **action:stop** and it will stop the service.
9. Run the service with **action:pause** and it will pause the service.
9. Run the service with **action:continue** and it will continue the service.
10. Run the service with **username:YOUR_USERNAME**, **password:YOUR_PASSWORD** and **action:install** which installs it for the given account.
11. Run the service with **built-in-account:(NetworkService|LocalService|LocalSystem)** and **action:install** which installs it for the given built in account. Defaults to **LocalSystem**.
12. Run the service with **description:YOUR_DESCRIPTION** and it setup description for the service.
13. Run the service with **display-name:YOUR_DISPLAY_NAME** and it setup Display name for the service.
14. Run the service with **name:YOUR_NAME** and it setup name for the service.
15. Run the service with **start-immediately:(true|false)** to start service immediately after install. Defaults to **true**.
16. You can find the complete example in PeterKottas.DotNetCore.Example project.
17. Install the service using powershell: dotnet.exe $serviceDllPath action:install

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## License

MIT 

## Credit

Huge thanks goes to @dasMulli the guy behind a useful [lib](https://github.com/dasMulli/dotnet-win32-service) which is one of the dependecies for this library.
