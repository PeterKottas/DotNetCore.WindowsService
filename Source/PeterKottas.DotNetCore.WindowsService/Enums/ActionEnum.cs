using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService.Enums
{
    public enum ActionEnum
    {
        Install,
        Uninstall,
        Run,
        RunInteractive,
        Stop,
        Start,
        Pause,
        Continue
    }
}
