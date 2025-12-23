using System.Text;

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

    internal static string[] Execute(ContainerResourceSettings settings)
    {
        // Note: Testcontainers doesn't support user parameter in ExecAsync
        return new CreateUserCommand(settings).ToCommandArray();
    }
    
    public string Command => _command.ToString();
}