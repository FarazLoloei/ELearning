namespace ELearning.API.Infrastructure;

public static class OcelotGatewayMode
{
    public static bool IsEnabled(IConfiguration configuration) =>
        configuration.GetValue<bool>("Ocelot:Enabled");
}
