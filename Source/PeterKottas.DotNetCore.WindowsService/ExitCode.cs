namespace PeterKottas.DotNetCore.WindowsService
{
	/// <summary>
	/// Copy of: https://github.com/Topshelf/Topshelf/blob/develop/src/Topshelf/TopshelfExitCode.cs
	/// </summary>
	enum ExitCode
    {
		Ok = 0,
		AbnormalExit = 1,
		SudoRequired = 2,
		ServiceAlreadyInstalled = 3,
		ServiceNotInstalled = 4,
		StartServiceFailed = 5,
		StopServiceFailed = 6,
		ServiceAlreadyRunning = 7,
		UnhandledServiceException = 8,
		ServiceNotRunning = 9,
		SendCommandFailed = 10,
	}
}
