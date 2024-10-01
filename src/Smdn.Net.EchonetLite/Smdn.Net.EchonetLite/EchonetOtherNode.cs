// SPDX-FileCopyrightText: 2024 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;

using Smdn.Net.EchonetLite.Protocol;

namespace Smdn.Net.EchonetLite;

/// <summary>
/// 他のECHONET Liteノード(他ノード)を表すクラス。
/// </summary>
internal sealed class EchonetOtherNode : EchonetNode {
  public override IReadOnlyCollection<EchonetObject> Devices => readOnlyDevices.Values;

  private readonly ConcurrentDictionary<EOJ, EchonetObject> devices;
  private readonly ReadOnlyDictionary<EOJ, EchonetObject> readOnlyDevices;

  internal EchonetOtherNode(IPAddress address, EchonetObject nodeProfile)
    : base(address, nodeProfile)
  {
    devices = new();
    readOnlyDevices = new(devices);
  }

  protected internal override EchonetObject? FindDevice(EOJ eoj)
    => devices.TryGetValue(eoj, out var device) ? device : null;

  internal EchonetObject GetOrAddDevice(EOJ eoj, out bool added)
  {
    added = false;

    if (devices.TryGetValue(eoj, out var device))
      return device;

    var newDevice = new EchonetObject(eoj);

    device = devices.GetOrAdd(eoj, newDevice);

    added = ReferenceEquals(device, newDevice);

    if (added)
      OnDevicesChanged(new(NotifyCollectionChangedAction.Add, newDevice));

    return device;
  }
}
