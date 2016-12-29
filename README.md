# DotNetCore.WindowsService

Simple library that allows one to host dot net core application as windows services. Perfect solution to power micro-services architecture.

## Installation

Using nuget:
**Install-Package PeterKottas.DotNetCore.WindowsService**

## Usage

1. Create .NETCore console app with a project.json simmilar to this:
	
	```cs
	{
		"version": "1.0.0-*",
		"buildOptions": {
			"emitEntryPoint": true
		},
		"frameworks": {
			"netcoreapp1.1": {
				"dependencies": {
					"Microsoft.NETCore.App": {
						"version": "1.1.0"//Optionally add "type": "platform" if you don't want self contained app
					}
				},
				"imports": "dnxcore50"
			}
		},
		"runtimes": { //Optionally add runtimes that you want to support
			"win81-x64": {}
		}
	}
	```
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
				Console.WriteLine("Polling at {0}\n", DateTime.Now.ToString("o")));
			},
			(e) =>
			{
				Console.WriteLine("Exception while polling: {0}\n", e.ToString()));
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
				serviceConfig.ServiceFactory((extraArguments) =>
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
10. Run the service with **username:YOUR_USERNAME**, **password:YOUR_PASSWORD** and **action:install** which installs it for the given account.
11. Run the service with **description:YOUR_DESCRIPTION** and it setup description for the service.
11. Run the service with **displayName:YOUR_DISPLAY_NAME** and it setup Display name for the service.
12. Run the service with **name:YOUR_NAME** and it setup name for the service.
13. You can find the complete example in PeterKottas.DotNetCore.Example project.

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## License

MIT 
