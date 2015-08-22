using System;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.SystemCommands.Contracts;

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
