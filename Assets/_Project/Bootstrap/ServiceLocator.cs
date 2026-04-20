using UnityEngine;

/// <summary>
/// Provides global access to persistent bootstrap services.
/// </summary>
public static class ServiceLocator
{
    public static NGOTransport Transport { get; private set; }
    public static ServicesInitialiser Services { get; private set; }

    public static void Register(NGOTransport transport)
    {
        Transport = transport;
    }

    public static void Register(ServicesInitialiser services)
    {
        Services = services;
    }
}