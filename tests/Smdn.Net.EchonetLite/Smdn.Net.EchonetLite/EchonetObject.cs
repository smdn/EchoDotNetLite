// SPDX-FileCopyrightText: 2024 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using Smdn.Net.EchonetLite.ComponentModel;

using SequenceIs = Smdn.Test.NUnit.Constraints.Buffers.Is;

namespace Smdn.Net.EchonetLite;

[TestFixture]
public class EchonetObjectTests {
  private class PseudoEventInvoker : IEventInvoker {
    public ISynchronizeInvoke? SynchronizingObject { get; set; }

    public void InvokeEvent<TEventArgs>(object? sender, EventHandler<TEventArgs>? eventHandler, TEventArgs e)
      => eventHandler?.Invoke(sender, e);
  }

  private class PseudoDevice : EchonetDevice {
    protected override IEventInvoker EventInvoker { get; } = new PseudoEventInvoker();

    public PseudoDevice()
      : base(
        classGroupCode: 0x00,
        classCode: 0x00,
        instanceCode: 0x00
      )
    {
    }

    public new EchonetProperty CreateProperty(byte propertyCode)
      => base.CreateProperty(
        propertyCode: propertyCode,
        canSet: true,
        canGet: true,
        canAnnounceStatusChange: true
      );
  }

  [Test]
  public void PropertyValueUpdated()
  {
    var device = new PseudoDevice();
    var p = device.CreateProperty(0x00);

    var newValue = new byte[] { 0x00 };
    var countOfValueUpdated = 0;
    var expectedPreviousUpdatedTime = default(DateTime);

    device.PropertyValueUpdated += (sender, e) => {
      Assert.That(sender, Is.SameAs(device), nameof(sender));
      Assert.That(e.Property, Is.SameAs(p), nameof(e.Property));

      switch (countOfValueUpdated) {
        case 0:
          Assert.That(e.OldValue, SequenceIs.EqualTo(default(ReadOnlyMemory<byte>)), nameof(e.OldValue));
          Assert.That(e.NewValue, SequenceIs.EqualTo(newValue), nameof(e.NewValue));
          Assert.That(e.PreviousUpdatedTime, Is.EqualTo(expectedPreviousUpdatedTime), nameof(e.PreviousUpdatedTime));
          Assert.That(e.UpdatedTime, Is.GreaterThan(e.PreviousUpdatedTime), nameof(e.UpdatedTime));

          expectedPreviousUpdatedTime = e.UpdatedTime;

          break;

        case 1:
          Assert.That(e.OldValue, SequenceIs.EqualTo(newValue), nameof(e.OldValue));
          Assert.That(e.NewValue, SequenceIs.EqualTo(newValue), nameof(e.NewValue));
          Assert.That(e.PreviousUpdatedTime, Is.EqualTo(expectedPreviousUpdatedTime), nameof(e.PreviousUpdatedTime));
          Assert.That(e.UpdatedTime, Is.GreaterThan(e.PreviousUpdatedTime), nameof(e.UpdatedTime));
          break;

        default:
          Assert.Fail("extra ValueUpdated event raised");
          break;
      }

      countOfValueUpdated++;
    };

    Assert.DoesNotThrow(() => p.SetValue(newValue.AsMemory(), raiseValueUpdatedEvent: true, setLastUpdatedTime: true));

    Assert.That(countOfValueUpdated, Is.EqualTo(1), $"{nameof(countOfValueUpdated)} #1");

    // set same value again
    Assert.DoesNotThrow(() => p.SetValue(newValue.AsMemory(), raiseValueUpdatedEvent: true, setLastUpdatedTime: true));

    Assert.That(countOfValueUpdated, Is.EqualTo(2), $"{nameof(countOfValueUpdated)} #2");
  }
}