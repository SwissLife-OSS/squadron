using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Squadron;

internal class DynamicPortVariableResolver: IVariableResolver
{
    private static readonly IPEndPoint DefaultLoopbackEndpoint =
        new IPEndPoint(IPAddress.Loopback, port: 0);

    private readonly IDictionary<string, int> _resolvedPorts = new Dictionary<string, int>();

    public bool CanHandle(VariableType type)
    {
        return type == VariableType.DynamicPort;
    }

    public T Resolve<T>(string dynamicVariableName)
    {
        if (typeof(T) != typeof(int) && typeof(T) != typeof(string))
        {
            throw new NotSupportedException(
                $"The resolver {nameof(DynamicPortVariableResolver)} cannot " +
                $"resolve the type '{nameof(T)}'. Only the type 'int' is supported");
        }

        int resolvedPort = GetOrResolvePort(dynamicVariableName);

        return (T)Convert.ChangeType(resolvedPort, typeof(T));
    }

    private int GetOrResolvePort(string dynamicVariableName)
    {
        if (_resolvedPorts.TryGetValue(dynamicVariableName, out var port))
        {
            return port;
        }

        var resolvedPort = GetAvailablePort();

        _resolvedPorts.Add(dynamicVariableName, resolvedPort);

        return resolvedPort;
    }

    private static int GetAvailablePort()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(DefaultLoopbackEndpoint);
        return ((IPEndPoint)socket.LocalEndPoint)!.Port;
    }
}