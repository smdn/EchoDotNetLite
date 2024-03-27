// EchoDotNetLiteLANBridge.dll (EchoDotNetLiteLANBridge)
//   Name: EchoDotNetLiteLANBridge
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0+cb7a08465ac80ce862063d85cf7c3cd6cfb81e91
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release
//   Referenced assemblies:
//     EchoDotNetLite, Version=1.0.0.0, Culture=neutral
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     System.Linq, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Net.NetworkInformation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Net.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Net.Sockets, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Threading, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
#nullable enable annotations

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EchoDotNetLite;
using EchoDotNetLiteLANBridge;

namespace EchoDotNetLiteLANBridge {
  public class LANClient :
    IDisposable,
    IPANAClient
  {
    public event EventHandler<(string, byte[])> OnEventReceived;

    public LANClient(ILogger<LANClient> logger) {}

    public void Dispose() {}
    public async Task RequestAsync(string address, byte[] request) {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi v1.3.0.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.3.0.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)