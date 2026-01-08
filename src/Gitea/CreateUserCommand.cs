using System.Text;

namespace Squadron;

internal class CreateUserCommand : ICommand
{
    private readonly StringBuilder _command = new StringBuilder();

    private CreateUserCommand(ContainerResourceSettings settings)
    {
        _command.Append($"gitea admin user create --admin --username {settings.Username} " +
                        $"--password {settings.Password} --email {settings.Username}@local");
    }

    internal static string[] Execute(ContainerResourceSettings settings)
    {
        // Run gitea command as 'git' user since Gitea refuses to run as root
        var cmd = new CreateUserCommand(settings);
        return ["su", "-c", cmd.Command, "git"];
    }
    
    public string Command => _command.ToString();
}