// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Text.Json;

using NUnit.Framework;

namespace Smdn.Net.EchonetLite.Protocol;

[TestFixture]
public class EOJTests {
  [TestCase(0x00, "\"ClassGroupCode\":\"00\"")]
  [TestCase(0x01, "\"ClassGroupCode\":\"01\"")]
  [TestCase(0x0F, "\"ClassGroupCode\":\"0F\"")]
  [TestCase(0x10, "\"ClassGroupCode\":\"10\"")]
  [TestCase(0xFF, "\"ClassGroupCode\":\"FF\"")]
  public void Serialize_ClassGroupCode(byte classGroupCode, string expectedJsonFragment)
  {
    var eoj = new EOJ(classGroupCode, 0x00, 0x00);

    Assert.That(JsonSerializer.Serialize(eoj), Does.Contain(expectedJsonFragment));
  }

  [TestCase(0x00, "\"ClassCode\":\"00\"")]
  [TestCase(0x01, "\"ClassCode\":\"01\"")]
  [TestCase(0x0F, "\"ClassCode\":\"0F\"")]
  [TestCase(0x10, "\"ClassCode\":\"10\"")]
  [TestCase(0xFF, "\"ClassCode\":\"FF\"")]
  public void Serialize_ClassCode(byte classCode, string expectedJsonFragment)
  {
    var eoj = new EOJ(0x00, classCode, 0x00);

    Assert.That(JsonSerializer.Serialize(eoj), Does.Contain(expectedJsonFragment));
  }

  [TestCase(0x00, "\"InstanceCode\":\"00\"")]
  [TestCase(0x01, "\"InstanceCode\":\"01\"")]
  [TestCase(0x0F, "\"InstanceCode\":\"0F\"")]
  [TestCase(0x10, "\"InstanceCode\":\"10\"")]
  [TestCase(0xFF, "\"InstanceCode\":\"FF\"")]
  public void Serialize_InstanceCode(byte instanceCode, string expectedJsonFragment)
  {
    var eoj = new EOJ(0x00, 0x00, instanceCode);

    Assert.That(JsonSerializer.Serialize(eoj), Does.Contain(expectedJsonFragment));
  }
}
