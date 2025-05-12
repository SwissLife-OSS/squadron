using System.Text;
using Docker.DotNet.Models;

namespace Squadron;

internal class CreateUserCommand : ICommand
{
    private readonly StringBuilder _command = new StringBuilder();

    private CreateUserCommand(ContainerResourceSettings settings)
    {
        _command.Append("gitea ");
        _command.Append($"admin user create --admin --username {settings.Username} --password {settings.Password} " +
                        $"--email {settings.Username}@local");
    }

    internal static ContainerExecCreateParameters Execute(ContainerResourceSettings settings)
    {
        return new CreateUserCommand(settings).ToContainerExecCreateParameters("1000");
    }
    
    public string Command => _command.ToString();
}