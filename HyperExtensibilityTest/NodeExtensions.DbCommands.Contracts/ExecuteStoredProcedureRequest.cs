using System.Data.SqlClient;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace NodeExtensions.DbCommands.Contracts
{
    [DataContract]
    public class ExecuteStoredProcedureRequest : ICommandRequest
    {
        [DataMember]
        public string StoredProcedureName { get; set; }

        [DataMember]
        public SqlParameterCollection Parameters { get; set; }
    }
}
