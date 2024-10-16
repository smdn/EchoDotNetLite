// Smdn.Net.EchonetLite.RouteB.BP35XX.dll (Smdn.Net.EchonetLite.RouteB.BP35XX-2.0.0-preview4)
//   Name: Smdn.Net.EchonetLite.RouteB.BP35XX
//   AssemblyVersion: 2.0.0.0
//   InformationalVersion: 2.0.0-preview4+c7a206f316403313ee223391269df5de9d965d32
//   TargetFramework: .NETStandard,Version=v2.1
//   Configuration: Release
//   Referenced assemblies:
//     Microsoft.Extensions.DependencyInjection.Abstractions, Version=8.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Smdn.Devices.BP35XX, Version=1.0.0.0, Culture=neutral
//     Smdn.Net.EchonetLite.RouteB.Primitives, Version=2.0.0.0, Culture=neutral
//     Smdn.Net.EchonetLite.RouteB.SkStackIP, Version=2.0.0.0, Culture=neutral
//     Smdn.Net.SkStackIP, Version=1.2.0.0, Culture=neutral
//     netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
#nullable enable annotations

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Smdn.Devices.BP35XX;
using Smdn.Net.EchonetLite.RouteB.Transport;
using Smdn.Net.EchonetLite.RouteB.Transport.SkStackIP;
using Smdn.Net.SkStackIP;

namespace Smdn.Net.EchonetLite.RouteB.Transport.BP35XX {
  public sealed class BP35A1RouteBEchonetLiteHandlerFactory : SkStackRouteBEchonetLiteHandlerFactory {
    public BP35A1RouteBEchonetLiteHandlerFactory(IServiceCollection services, Action<BP35A1Configurations> configure) {}

    protected override SkStackRouteBTransportProtocol TransportProtocol { get; }

    protected override async ValueTask<SkStackClient> CreateClientAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken) {}
  }

  public static class IRouteBEchonetLiteHandlerBuilderExtensions {
    public static ISkStackRouteBEchonetLiteHandlerFactory AddBP35A1(this IRouteBEchonetLiteHandlerBuilder builder, Action<BP35A1Configurations> configure) {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.4.1.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.3.1.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
