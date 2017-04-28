using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using God.Qq.Core.Models;

namespace God.Qq.Core.Handlers
{
    public interface IHandler
    {
        void Run(QqContext context);
    }
}
