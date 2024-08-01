using System.Reflection;

namespace anc.webapi;
public static class ApiAssemblyReference
{
    public static readonly Assembly Assembly = typeof(ApiAssemblyReference).Assembly;
}