using DasMulli.Win32.ServiceUtils;

namespace PeterKottas.DotNetCore.WindowsService.Interfaces
{
    public interface IShutdownableWin32Service : IWin32Service
    {
        void Shutdown();
    }
}
