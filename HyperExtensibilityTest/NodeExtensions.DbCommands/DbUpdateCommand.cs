using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace NodeExtensions.DbCommands
{
    public class DbUpdateCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            return new CommandResponseString(MessageProcessStatusFlags.Success, "Database Updated Successfully");
        }
    }
}
