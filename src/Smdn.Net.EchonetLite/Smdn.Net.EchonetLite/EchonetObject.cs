// SPDX-FileCopyrightText: 2018 HiroyukiSakoh
// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Smdn.Net.EchonetLite.ComponentModel;
using Smdn.Net.EchonetLite.Protocol;

namespace Smdn.Net.EchonetLite;

/// <summary>
/// ECHONET Lite オブジェクトインスタンス
/// </summary>
public abstract partial class EchonetObject {
  public static EchonetObject Create(IEchonetObjectSpecification objectDetail, byte instanceCode)
    => new DetailedEchonetObject(
      objectDetail ?? throw new ArgumentNullException(nameof(objectDetail)),
      instanceCode
    );

  /// <summary>
  /// プロパティの一覧<see cref="Properties"/>に変更があったときに発生するイベント。
  /// </summary>
  /// <remarks>
  /// ECHONET Lite サービス「INF_REQ:プロパティ値通知要求」(ESV <c>0x63</c>)などによって
  /// 現在のオブジェクトにECHONET Lite プロパティが追加・削除された際にイベントが発生します。
  /// 変更の詳細は、イベント引数<see cref="NotifyCollectionChangedEventArgs"/>を参照してください。
  /// </remarks>
  public event EventHandler<NotifyCollectionChangedEventArgs>? PropertiesChanged;

  /// <summary>
  /// このオブジェクトが属するECHONET Liteノードを表す<see cref="EchonetNode"/>を取得します。
  /// </summary>
  public EchonetNode Node {
    get {
#if DEBUG
      if (OwnerNode is null)
        throw new InvalidOperationException($"{nameof(OwnerNode)} is null");
#endif

      return OwnerNode!;
    }
  }

  internal EchonetNode? OwnerNode { get; set; }

  /// <summary>
  /// このインスタンスでイベントを発生させるために使用される<see cref="IEventInvoker"/>を取得します。
  /// </summary>
  /// <exception cref="InvalidOperationException"><see cref="IEventInvoker"/>を取得することができません。</exception>
  protected virtual IEventInvoker EventInvoker
    => OwnerNode?.EventInvoker ?? throw new InvalidOperationException($"{nameof(EventInvoker)} can not be null.");

  /// <summary>
  /// プロパティマップが取得済みであるかどうかを表す<see langword="bool"/>型の値を取得します。
  /// </summary>
  /// <value>
  /// 現在のECHONET オブジェクトの詳細仕様が参照可能な場合は、常に<see langword="true"/>を返します。
  /// </value>
  /// <seealso cref="EchonetClient.PropertyMapAcquiring"/>
  /// <seealso cref="EchonetClient.PropertyMapAcquired"/>
  public abstract bool HasPropertyMapAcquired { get; internal set; }

  /// <summary>
  /// クラスグループコードを表す<see langword="byte"/>型の値を取得します。
  /// </summary>
  public abstract byte ClassGroupCode { get; }

  /// <summary>
  /// クラスコードを表す<see langword="byte"/>型の値を取得します。
  /// </summary>
  public abstract byte ClassCode { get; }

  /// <summary>
  /// インスタンスコードを表す<see langword="byte"/>型の値を取得します。
  /// </summary>
  public abstract byte InstanceCode { get; }

  /// <summary>
  /// EOJ
  /// </summary>
  internal EOJ EOJ => new(
    classGroupCode: ClassGroupCode,
    classCode: ClassCode,
    instanceCode: InstanceCode
  );

  /// <summary>
  /// プロパティの一覧
  /// </summary>
  public abstract IReadOnlyCollection<EchonetProperty> Properties { get; }

  /// <summary>
  /// GETプロパティの一覧
  /// </summary>
  public virtual IEnumerable<EchonetProperty> GetProperties => Properties.Where(static p => p.CanGet);

  /// <summary>
  /// SETプロパティの一覧
  /// </summary>
  public virtual IEnumerable<EchonetProperty> SetProperties => Properties.Where(static p => p.CanSet);

  /// <summary>
  /// ANNOプロパティの一覧
  /// </summary>
  public virtual IEnumerable<EchonetProperty> AnnoProperties => Properties.Where(static p => p.CanAnnounceStatusChange);

  /// <summary>
  /// このオブジェクトが属するECHONET Liteノードを指定せずにインスタンスを作成します。
  /// </summary>
  /// <remarks>
  /// このコンストラクタを使用してインスタンスを作成した場合、インスタンスが使用されるまでの間に、
  /// <see cref="OwnerNode"/>プロパティへ明示的に<see cref="EchonetNode"/>を設定する必要があります。
  /// </remarks>
  private protected EchonetObject()
  {
  }

  /// <summary>
  /// このオブジェクトが属するECHONET Liteノードを指定してインスタンスを作成します。
  /// </summary>
  /// <param name="node">このオブジェクトが属するECHONET Liteノードを表す<see cref="EchonetNode"/>を指定します。</param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="node"/>が<see langword="null"/>です。
  /// </exception>
  private protected EchonetObject(EchonetNode node)
  {
    OwnerNode = node ?? throw new ArgumentNullException(nameof(node));
  }

  private protected void OnPropertiesChanged(NotifyCollectionChangedEventArgs e)
    => EventInvoker.InvokeEvent(this, PropertiesChanged, e);
}
