#if NET46
namespace Squadron
{
    internal class DeployScript
    {
        private readonly string[] _content;

        internal DeployScript(string content)
        {
            _content = content.Split('\n');
        }

        internal void SetVariable(string variable, string value)
        {
            for (var i = 0; i < _content.Length; i++)
            {
                if (_content[i].StartsWith($":setvar {variable}"))
                {
                    _content[i] = $":setvar {variable} \"{value}\"";
                    break;
                }
            }
        }

        internal string Generate()
        {
            return string.Join("\n", _content);
        }
    }
}
#endif
