// SPDX-FileCopyrightText: 2024 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using Smdn.Net.EchonetLite.Protocol;

namespace Smdn.Net.EchonetLite;

[TestFixture]
public partial class EchonetClientOperationsTests {
  private class PseudoDevice : EchonetDevice {
    public PseudoDevice()
      : base(
        classGroupCode: 0x00,
        classCode: 0x00,
        instanceCode: 0x00
      )
    {
    }

    public PseudoDevice(
      byte classGroupCode,
      byte classCode,
      byte instanceCode
    )
      : base(
        classGroupCode: classGroupCode,
        classCode: classCode,
        instanceCode: instanceCode
      )
    {
    }
  }

  private class ValidateRequestEchonetLiteHandler(Action<IPAddress?, ReadOnlyMemory<byte>> validate) : IEchonetLiteHandler {
    public ValueTask SendAsync(IPAddress? address, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
      validate(address, data);

      return default;
    }

    public Func<IPAddress, ReadOnlyMemory<byte>, CancellationToken, ValueTask>? ReceiveCallback { get; set; }
  }
}