// <copyright file="OcelotGatewayMode.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

public static class OcelotGatewayMode
{
    public static bool IsEnabled(IConfiguration configuration) =>
        configuration.GetValue<bool>("Ocelot:Enabled");
}
