// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Text.Json;

using NUnit.Framework;

namespace Smdn.Net.EchonetLite.Protocol;

[TestFixture]
public class FrameTests {
  [TestCase(EHD2.Type1)]
  [TestCase(EHD2.Type2)]
  public void Ctor_EDATANull(EHD2 ehd2)
  {
    Assert.Throws<ArgumentNullException>(
      () => new Frame(EHD1.EchonetLite, ehd2, (ushort)0x0000u, null!)
    );
  }

  private class PseudoEData : IEData { }

  private static System.Collections.IEnumerable YieldTestCases_Ctor_EDATATypeMismatch()
  {
    yield return new object?[] { EHD2.Type1, new EData2(default) };
    yield return new object?[] { EHD2.Type1, new PseudoEData() };
    yield return new object?[] { EHD2.Type2, new EData1(default, default, default, Array.Empty<PropertyRequest>()) };
    yield return new object?[] { EHD2.Type2, new PseudoEData() };
  }

  [TestCaseSource(nameof(YieldTestCases_Ctor_EDATATypeMismatch))]
  public void Ctor_EDATATypeMismatch(EHD2 ehd2, IEData edata)
  {
    Assert.Throws<ArgumentException>(
      () => new Frame(EHD1.EchonetLite, ehd2, (ushort)0x0000u, edata)
    );
  }

  [TestCase(EHD1.EchonetLite, "\"EHD1\":\"10\"")]
  [TestCase((EHD1)0x00, "\"EHD1\":\"00\"")]
  [TestCase((EHD1)0x01, "\"EHD1\":\"01\"")]
  [TestCase((EHD1)0xFF, "\"EHD1\":\"FF\"")]
  public void Serialize_EHD1(EHD1 ehd1, string expectedJsonFragment)
  {
    var f = new Frame(ehd1, EHD2.Type2, (ushort)0x0000u, new EData2(default));

    Assert.That(JsonSerializer.Serialize(f), Does.Contain(expectedJsonFragment));
  }

  [Test]
  public void Serialize_EHD2_Type1()
  {
    var f = new Frame(EHD1.EchonetLite, EHD2.Type1, (ushort)0x0000u, new EData1(default, default, default, Array.Empty<PropertyRequest>()));

    Assert.That(JsonSerializer.Serialize(f), Does.Contain("\"EHD2\":\"81\""));
  }

  [Test]
  public void Serialize_EHD2_Type2()
  {
    var f = new Frame(EHD1.EchonetLite, EHD2.Type2, (ushort)0x0000u, new EData2(default));

    Assert.That(JsonSerializer.Serialize(f), Does.Contain("\"EHD2\":\"82\""));
  }

  [TestCase((ushort)0x0000u, "\"TID\":\"0000\"")]
  [TestCase((ushort)0x0001u, "\"TID\":\"0100\"")]
  [TestCase((ushort)0x0100u, "\"TID\":\"0001\"")]
  [TestCase((ushort)0x00FFu, "\"TID\":\"FF00\"")]
  [TestCase((ushort)0xFF00u, "\"TID\":\"00FF\"")]
  [TestCase((ushort)0xFFFFu, "\"TID\":\"FFFF\"")]
  public void Serialize_TID(ushort tid, string expectedJsonFragment)
  {
    var f = new Frame(EHD1.EchonetLite, EHD2.Type2, tid, new EData2(default));

    Assert.That(JsonSerializer.Serialize(f), Does.Contain(expectedJsonFragment));
  }
}
