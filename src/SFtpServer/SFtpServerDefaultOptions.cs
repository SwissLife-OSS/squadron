using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squadron
{
    public class SFtpServerDefaultOptions : ContainerResourceOptions
    {
        const string Username = "username";
        const string Password = "password";
        const string Directory = "Uploads";

        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("sftp-server")
                .Image("atmoz/sftp:latest")
                .Username(Username)
                .Password(Password)
                .InternalPort(22)
                .WaitTimeout(120)
                .AddKeyValue(WellKnown.DirectoryName, Directory)
                .AddCmd($"{Username}:{Password}:::{Directory}")
                .PreferLocalImage();
        }
    }
}
