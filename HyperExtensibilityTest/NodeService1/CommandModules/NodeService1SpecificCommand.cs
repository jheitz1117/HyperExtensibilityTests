using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace NodeService1.CommandModules
{
    public class NodeService1SpecificCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            return new CommandResponseString(MessageProcessStatusFlags.Success, GetType().FullName + " executed successfully");
        }
    }
}
