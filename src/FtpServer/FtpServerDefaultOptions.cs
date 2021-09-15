using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squadron
{
    public class FtpServerDefaultOptions : ContainerResourceOptions
    {
        const string Password = "password";
        const string Username = "username";

        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("ftp-server")
                .Image("fauria/vsftpd")
                //.AddRuntimeVariable("DYNAMIC_PORT_1", RuntimeVariableType.DynamicPort)
                .AddEnvironmentVariable($"FTP_USER={Username}")
                .AddEnvironmentVariable($"FTP_PASS={Password}")
                .AddEnvironmentVariable($"PASV_ADDRESS=127.0.0.1")
                .AddEnvironmentVariable("PASV_MIN_PORT=50000")
                .AddEnvironmentVariable("PASV_MAX_PORT=50000")
                .AddEnvironmentVariable($"PASV_ENABLE=YES")
                .Username(Username)
                .Password(Password)
                .InternalPort(21)
                .AddPortMapping(new ContainerPortMap(50000))
                .WaitTimeout(60)
                .PreferLocalImage();
        }
    }
}
