using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService.Interfaces
{
    public interface IMicroService
    {
        void Start();
        void Stop();
    }
}
