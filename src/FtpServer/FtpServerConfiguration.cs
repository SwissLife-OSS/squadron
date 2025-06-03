using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron;

public class FtpServerConfiguration
{
    internal FtpServerConfiguration(
        string host,
        int port,
        string username,
        string password)
    {
        Host = host;
        Port = port;
        Username = username;
        Password = password;
    }

    public string Host { get; }
    public int Port { get; }
    public string Username { get; }
    public string Password { get; }
}