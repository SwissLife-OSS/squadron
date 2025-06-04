using System.Collections.Generic;
using System.Linq;

namespace Squadron;

public static class ComposableResourceSettingsExtensions
{
    public static bool HasLinks(this ComposableResourceSettings settings)
    {
        return settings.Links.Any();
    }

    public static IEnumerable<ComposableResourceSettings> Links(
        this ComposableResourceSettings resource,
        IEnumerable<ComposableResourceSettings> all )
    {
        foreach (ComposeResourceLink link in resource.Links)
        {
            ComposableResourceSettings linkRes = all.FirstOrDefault(x => x.Name == link.Name);
            yield return linkRes;
        }
    }
}