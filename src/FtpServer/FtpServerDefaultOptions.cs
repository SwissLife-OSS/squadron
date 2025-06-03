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

        const string DynamicPort1VariableName = "DYNAMIC_PORT_1";

        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("ftp-server")
                .Image("fauria/vsftpd")
                .AddVariable(DynamicPort1VariableName, VariableType.DynamicPort)
                .AddEnvironmentVariable($"FTP_USER={Username}")
                .AddEnvironmentVariable($"FTP_PASS={Password}")
                .AddEnvironmentVariable($"PASV_ADDRESS=127.0.0.1")
                .AddEnvironmentVariable($"PASV_MIN_PORT={{{DynamicPort1VariableName}}}")
                .AddEnvironmentVariable($"PASV_MAX_PORT={{{DynamicPort1VariableName}}}")
                .AddEnvironmentVariable($"PASV_ENABLE=YES")
                .AddEnvironmentVariable("ALLOW_WRITEABLE_CHROOT=YES")
                .AddEnvironmentVariable("LOCAL_ENABLE=YES")
                .AddEnvironmentVariable("WRITE_ENABLE=YES")
                .AddEnvironmentVariable("ANONYMOUS_ENABLE=NO")
                .AddEnvironmentVariable("CHROOT_LOCAL_USER=YES")
                .AddEnvironmentVariable("SECCOMP_SANDBOX=NO")
                .AddEnvironmentVariable("PASV_PROMISCUOUS=YES")
                .Username(Username)
                .Password(Password)
                .InternalPort(21)
                .AddPortMapping(DynamicPort1VariableName, DynamicPort1VariableName)
                .WaitTimeout(120)
                .PreferLocalImage();
        }
    }
}
