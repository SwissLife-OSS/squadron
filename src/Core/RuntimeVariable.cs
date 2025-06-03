using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron;

public class Variable(
    string name,
    VariableType type)
{
    public string Name { get; } = name;
    public VariableType Type { get; } = type;
}