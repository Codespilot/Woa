namespace Woa.Common;

/// <summary>
/// The delegate to inject or resolve service with specified name.
/// </summary>
/// <param name="name">The service name.</param>
/// <typeparam name="TService">The service type.</typeparam>
public delegate TService NamedService<out TService>(string name);