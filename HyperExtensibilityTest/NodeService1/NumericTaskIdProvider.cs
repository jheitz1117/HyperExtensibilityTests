using System;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility;

namespace NodeService1
{
    public class NumericTaskIdProvider : TaskIdProviderBase
    {
        public override string CreateTaskId(IHyperNodeMessageContext context)
        {
            return (context.CommandName == SystemCommandName.GetCachedTaskProgressInfo)
                ? Guid.NewGuid().ToString()
                : "AnOverrideStringForEveryTask!";
        }
    }
}
