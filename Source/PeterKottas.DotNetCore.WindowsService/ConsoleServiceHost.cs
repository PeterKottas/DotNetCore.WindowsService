using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService
{
	/// <summary>
	/// Copy of Topshelf ConsoleRunHost
	/// https://github.com/Topshelf/Topshelf/blob/develop/src/Topshelf/Hosts/ConsoleRunHost.cs
	/// </summary>
	class ConsoleServiceHost<SERVICE>
		where SERVICE : IMicroService
	{
		private InnerService _consoleService = null;
		private HostConfiguration<SERVICE> _innerConfig = null;
		private ExitCode _exitCode = 0;
		private ManualResetEvent _exit = null;
		private volatile bool _hasCancelled = false;

		public ConsoleServiceHost(InnerService consoleService, HostConfiguration<SERVICE> innerConfig)
		{
			_consoleService = consoleService 
				?? throw new ArgumentNullException(nameof(consoleService));

			_innerConfig = innerConfig
				?? throw new ArgumentNullException(nameof(innerConfig));
		}

		internal ExitCode Run()
		{
			AppDomain.CurrentDomain.UnhandledException += CatchUnhandledException;

			bool started = false;
			try
			{
				Console.WriteLine("Starting up as a console service host");

				_exit = new ManualResetEvent(false);
				_exitCode = ExitCode.Ok;

				Console.Title = _consoleService.ServiceName;
				Console.CancelKeyPress += HandleCancelKeyPress;

				_consoleService.Start(_innerConfig.ExtraArguments.ToArray(), () => Console.WriteLine("Stopping console service host"));
				started = true;

				Console.WriteLine("The {0} service is now running, press Control+C to exit.", _consoleService.ServiceName);

				_exit.WaitOne();
			}
			catch (Exception ex)
			{
				Console.WriteLine("An exception occurred", ex);

				return ExitCode.AbnormalExit;
			}
			finally
			{
				if (started)
					StopService();

				_exit.Close();
				(_exit as IDisposable).Dispose();
			}

			return _exitCode;
		}

		internal void StopService()
		{
			try
			{
				if (_hasCancelled)
					return;

				Console.WriteLine("Stopping the {0} service", _consoleService.ServiceName);

				Task stopTask = Task.Run(() => _consoleService.Stop());
                if (!stopTask.Wait(TimeSpan.FromMilliseconds(_innerConfig.ServiceTimeout)))
					throw new Exception("The service failed to stop (returned false).");

				_exitCode = ExitCode.Ok;
			}
			catch (Exception ex)
			{
				Console.WriteLine("The service did not shut down gracefully: {0}", ex.ToString());
				_exitCode = ExitCode.AbnormalExit;
			}
			finally
			{
				Console.WriteLine("The {0} service has stopped.", _consoleService.ServiceName);
				_exitCode = ExitCode.Ok;
			}
		}

		private void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
			{
				Console.WriteLine("Control+Break detected, terminating service (not cleanly, use Control+C to exit cleanly)");
				return;
			}

			e.Cancel = true;

			if (_hasCancelled)
				return;

			Console.WriteLine("Control+C detected, attempting to stop service.");
			Task stopTask = Task.Run(() => _consoleService.Stop());
            if (stopTask.Wait(TimeSpan.FromMilliseconds(_innerConfig.ConsoleTimeout)))
			{
				_hasCancelled = true;
				_exit.Set();
			}
			else
			{
				_hasCancelled = false;
				Console.WriteLine("The service is not in a state where it can be stopped.");
			}
		}

		private void CatchUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine("The service threw an unhandled exception: {0}", e.ToString());

			if (!e.IsTerminating)
				return;

			_exitCode = ExitCode.UnhandledServiceException;
			_exit.Set();
		}
	}
}
