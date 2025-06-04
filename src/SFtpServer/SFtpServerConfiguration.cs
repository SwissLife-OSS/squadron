using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron;

public class SFtpServerConfiguration
{
    internal SFtpServerConfiguration(
        string host,
        int port,
        string username,
        string password,
        string directory)
    {
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        Directory = directory;
    }

    public string Host { get; }
    public int Port { get; }
    public string Username { get; }
    public string Password { get; }
    public string Directory { get; }
}