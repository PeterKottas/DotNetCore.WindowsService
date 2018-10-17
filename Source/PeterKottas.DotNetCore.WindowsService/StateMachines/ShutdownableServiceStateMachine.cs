using DasMulli.Win32.ServiceUtils;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace PeterKottas.DotNetCore.WindowsService.StateMachines
{
    public sealed class ShutdownableServiceStateMachine : IWin32ServiceStateMachine
    {
        private readonly IShutdownableWin32Service _serviceImplementation;
        private ServiceStatusReportCallback _statusReportCallback;

        /// <summary>
        /// Initializes a new <see cref="SimpleServiceStateMachine"/> to run the specified service.
        /// </summary>
        /// <param name="serviceImplementation">The service implementation.</param>
        public ShutdownableServiceStateMachine(IShutdownableWin32Service serviceImplementation)
        {
            _serviceImplementation = serviceImplementation;
        }

        /// <summary>
        /// Called when the service is started.
        /// Use the provided <paramref name="statusReportCallback" /> to notify the service manager about
        /// state changes such as started, paused etc.
        /// </summary>
        /// <param name="startupArguments">The startup arguments passed via windows' service configuration.</param>
        /// <param name="statusReportCallback">The status report callback.</param>
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void OnStart(string[] startupArguments, ServiceStatusReportCallback statusReportCallback)
        {
            _statusReportCallback = statusReportCallback;

            try
            {
                _serviceImplementation.Start(startupArguments, HandleServiceImplementationStoppedOnItsOwn);

                statusReportCallback(ServiceState.Running, ServiceAcceptedControlCommandsFlags.Stop | ServiceAcceptedControlCommandsFlags.Shutdown, win32ExitCode: 0, waitHint: 0);
            }
            catch
            {
                statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: -1, waitHint: 0);
            }
        }

        /// <summary>
        /// Called when a command was received from windows' service system.
        /// </summary>
        /// <param name="command">The received command.</param>
        /// <param name="commandSpecificEventType">Type of the command specific event. See description of dwEventType at https://msdn.microsoft.com/en-us/library/windows/desktop/ms683241(v=vs.85).aspx</param>
        public void OnCommand(ServiceControlCommand command, uint commandSpecificEventType)
        {
            if (command == ServiceControlCommand.Stop)
            {
                _statusReportCallback(ServiceState.StopPending, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 3000);

                var win32ExitCode = 0;

                try
                {
                    _serviceImplementation.Stop();
                }
                catch
                {
                    win32ExitCode = -1;
                }

                _statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode, waitHint: 0);
            }
            else if (command == ServiceControlCommand.Shutdown)
            {
                try
                {
                    _statusReportCallback(ServiceState.StopPending, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 3000); //this is probably too much, see note down below
                    _serviceImplementation.Shutdown();
                }
                catch { }
                finally
                {
                    _statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 0);
                }
            }
        }

        private void HandleServiceImplementationStoppedOnItsOwn()
        {
            _statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 0);
        }
    }
}
