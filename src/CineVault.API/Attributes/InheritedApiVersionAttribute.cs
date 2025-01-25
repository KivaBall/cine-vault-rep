namespace CineVault.API.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class InheritedApiVersionAttribute(int version) : ApiVersionAttribute(version);