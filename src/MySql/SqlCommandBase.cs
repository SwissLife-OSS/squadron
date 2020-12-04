using System;
using System.Collections.Generic;
using System.Text;
using Docker.DotNet.Models;

namespace Squadron
{
    internal class SqlCommandBase
    {
        protected ContainerExecCreateParameters GetContainerExecParameters(
            string query,
            ContainerResourceSettings settings)
        {
            return new ContainerExecCreateParameters
            {
                AttachStderr = true,
                AttachStdin = false,
                AttachStdout = true,
                Cmd = GetCommand(query, settings),
                Detach = false,
                Tty = false
            };
        }

        private IList<string> GetCommand(
                string query,
                ContainerResourceSettings settings)
        {
            return new List<string>
            {
                "mysql",
                "-u",
                "root",
                $"-p{settings.Password}",
                "-e",
                query
            };
        }

    }
}
