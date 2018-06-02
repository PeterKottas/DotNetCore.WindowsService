using DasMulli.Win32.ServiceUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterKottas.DotNetCore.WindowsService.Interfaces
{
    public interface IShutdownableWin32Service : IWin32Service
    {
        void Shutdown();
    }
}
