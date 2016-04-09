using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using NodeExtensions.DbCommands.Contracts;

namespace NodeExtensions.DbCommands
{
    public class ExecuteStoredProcedureCommand : ICommandModule, ICommandRequestSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as ExecuteStoredProcedureRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(ExecuteStoredProcedureRequest), context.Request.GetType());

            return new CommandResponseString(
                MessageProcessStatusFlags.Success,
                string.Format("Stored Procedure '{0}' executed successfully.", request.StoredProcedureName)
            );
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<ExecuteStoredProcedureRequest>();
        }
    }
}
