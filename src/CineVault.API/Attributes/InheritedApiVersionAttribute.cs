namespace CineVault.API.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class InheritedApiVersionAttribute(int version) : ApiVersionAttribute(version);