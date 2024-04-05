// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Smdn.Net.SkStackIP;

namespace Smdn.Net.EchonetLite.RouteB.Transport.SkStackIP;

public abstract class SkStackRouteBEchonetLiteHandlerFactory(IServiceCollection services) : ISkStackRouteBEchonetLiteHandlerFactory {
  private readonly IServiceCollection services = services;

  public Action<SkStackRouteBSessionConfiguration>? ConfigureRouteBSessionConfiguration { get; set; }

  /// <summary>
  /// Gets the value specifying the transport protocol to be used by the handler which this factory creates.
  /// </summary>
  protected abstract SkStackRouteBTransportProtocol TransportProtocol { get; }

  protected abstract ValueTask<SkStackClient> CreateClientAsync(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken
  );

  public virtual async ValueTask<RouteBEchonetLiteHandler> CreateAsync(
    CancellationToken cancellationToken
  )
  {
    var sessionConfiguration = new SkStackRouteBSessionConfiguration();

    ConfigureRouteBSessionConfiguration?.Invoke(sessionConfiguration);

    var serviceProvider = services.BuildServiceProvider();

    var client = await CreateClientAsync(
      serviceProvider: serviceProvider,
      cancellationToken: cancellationToken
    ).ConfigureAwait(false);

    client.ReceiveResponseDelay = TimeSpan.FromMilliseconds(20); // TODO: make configurable
    client.ReceiveUdpPollingInterval = TimeSpan.FromMilliseconds(50); // TODO: make configurable

    return TransportProtocol switch {
      SkStackRouteBTransportProtocol.Tcp => new SkStackRouteBTcpEchonetLiteHandler(
        client: client,
        sessionConfiguration: sessionConfiguration,
        shouldDisposeClient: true,
        serviceProvider: serviceProvider
      ),

      SkStackRouteBTransportProtocol.Udp => new SkStackRouteBUdpEchonetLiteHandler(
        client: client,
        sessionConfiguration: sessionConfiguration,
        shouldDisposeClient: true,
        serviceProvider: serviceProvider
      ),

      _ => throw new InvalidOperationException($"There is no hanlder that supports '{TransportProtocol}' for the transport protocol."),
    };
  }
}
