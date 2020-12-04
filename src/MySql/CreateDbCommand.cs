using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Docker.DotNet.Models;

namespace Squadron
{
    internal class CreateDbCommand : SqlCommandBase, ICommand
    {
        public ContainerExecCreateParameters Parameters { get; }

        private CreateDbCommand(
            string dbname,
            ContainerResourceSettings settings)
        {
            Parameters = GetContainerExecParameters(
                $@"CREATE DATABASE {dbname};
                   CREATE ROLE developer_{dbname};
                   GRANT alter,create,delete,drop,index,insert,select,update,trigger,alter routine,
                            create routine, execute, create temporary tables 
                   ON {dbname}.* 
                   TO '{settings.Username}';",
                settings);
        }

        internal static ContainerExecCreateParameters Execute(string dbName,
            ContainerResourceSettings settings)
            => new CreateDbCommand(dbName, settings).Parameters;

        public string Command => string.Join(" ", Parameters.Cmd);
    }
}
