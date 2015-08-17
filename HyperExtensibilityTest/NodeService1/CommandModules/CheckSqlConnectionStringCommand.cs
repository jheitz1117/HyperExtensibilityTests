using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace NodeService1.CommandModules
{
    public class CheckSqlConnectionStringCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
