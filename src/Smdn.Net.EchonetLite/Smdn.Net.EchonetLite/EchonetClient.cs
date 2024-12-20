// SPDX-FileCopyrightText: 2018 HiroyukiSakoh
// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1506 // CA1506: Rewrite or refactor the code to decrease its class coupling below '96'.
#pragma warning disable CA1848 // CA1848: パフォーマンスを向上させるには、LoggerMessage デリゲートを使用します -->

using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly;

using Smdn.Net.EchonetLite.Protocol;
using Smdn.Net.EchonetLite.Transport;

namespace Smdn.Net.EchonetLite;

public partial class EchonetClient : IEchonetClientService, IDisposable, IAsyncDisposable {
  private readonly bool shouldDisposeEchonetLiteHandler;
  private IEchonetLiteHandler echonetLiteHandler; // null if disposed
  private readonly IEchonetDeviceFactory? deviceFactory;

  protected ILogger? Logger { get; }

  /// <summary>
  /// 現在の<see cref="EchonetClient"/>インスタンスが扱うECHONET Lite ノード(自ノード)を表す<see cref="SelfNode"/>を取得します。
  /// </summary>
  public EchonetNode SelfNode { get; }

  /// <summary>
  /// 現在のインスタンスと関連付けられている、既知のECHONET Lite ノード(他ノード)を管理する<see cref="EchonetNodeRegistry"/>を取得します。
  /// </summary>
  public EchonetNodeRegistry NodeRegistry {
    get {
      ThrowIfDisposed();

      return nodeRegistry;
    }
  }

  private EchonetNodeRegistry nodeRegistry;

  ILogger? IEchonetClientService.Logger => Logger;

#if SYSTEM_TIMEPROVIDER
  private readonly TimeProvider? timeProvider;

  TimeProvider? IEchonetClientService.TimeProvider => timeProvider;
#endif

  /// <inheritdoc cref="EchonetClient(EchonetNode, IEchonetLiteHandler, bool, EchonetNodeRegistry, IEchonetDeviceFactory, ResiliencePipeline, ILogger, IServiceProvider)"/>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="echonetLiteHandler"/>が<see langword="null"/>です。
  /// </exception>
  public EchonetClient(
    IEchonetLiteHandler echonetLiteHandler,
    bool shouldDisposeEchonetLiteHandler = false,
    IServiceProvider? serviceProvider = null
  )
    : this(
      selfNode: EchonetNode.CreateSelfNode(devices: Array.Empty<EchonetObject>()),
      echonetLiteHandler: echonetLiteHandler ?? throw new ArgumentNullException(nameof(echonetLiteHandler)),
      shouldDisposeEchonetLiteHandler: shouldDisposeEchonetLiteHandler,
      nodeRegistry: null,
      deviceFactory: null,
      resiliencePipelineForSendingResponseFrame: null,
      logger: serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<EchonetClient>(),
      serviceProvider: serviceProvider
    )
  {
  }

  /// <inheritdoc cref="EchonetClient(EchonetNode, IEchonetLiteHandler, bool, EchonetNodeRegistry, IEchonetDeviceFactory, ResiliencePipeline, ILogger, IServiceProvider)"/>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="echonetLiteHandler"/>が<see langword="null"/>です。
  /// </exception>
  public EchonetClient(
    IEchonetLiteHandler echonetLiteHandler,
    bool shouldDisposeEchonetLiteHandler,
    EchonetNodeRegistry? nodeRegistry,
    IEchonetDeviceFactory? deviceFactory,
    IServiceProvider? serviceProvider = null
  )
    : this(
      selfNode: EchonetNode.CreateSelfNode(devices: Array.Empty<EchonetObject>()),
      echonetLiteHandler: echonetLiteHandler ?? throw new ArgumentNullException(nameof(echonetLiteHandler)),
      shouldDisposeEchonetLiteHandler: shouldDisposeEchonetLiteHandler,
      nodeRegistry: nodeRegistry,
      deviceFactory: deviceFactory,
      resiliencePipelineForSendingResponseFrame: null,
      logger: serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<EchonetClient>(),
      serviceProvider: serviceProvider
    )
  {
  }

  /// <summary>
  /// <see cref="EchonetClient"/>クラスのインスタンスを初期化します。
  /// </summary>
  /// <param name="selfNode">
  /// 自ノードを表す<see cref="EchonetNode"/>。
  /// </param>
  /// <param name="echonetLiteHandler">
  /// このインスタンスがECHONET Lite フレームを送受信するために使用する<see cref="IEchonetLiteHandler"/>。
  /// </param>
  /// <param name="shouldDisposeEchonetLiteHandler">
  /// オブジェクトが破棄される際に、<paramref name="echonetLiteHandler"/>も破棄するかどうかを表す値。
  /// 省略した場合は、<see langword="false"/>をデフォルト値として使用します。
  /// </param>
  /// <param name="nodeRegistry">
  /// 既知のECHONET Lite ノード(他ノード)を管理する<see cref="EchonetNodeRegistry"/>。
  /// 省略した場合は、<see langword="null"/>をデフォルト値として使用します。
  /// </param>
  /// <param name="deviceFactory">
  /// 機器オブジェクトのファクトリとして使用される<see cref="IEchonetDeviceFactory"/>。
  /// 省略した場合は、<see langword="null"/>をデフォルト値として使用します。
  /// </param>
  /// <param name="resiliencePipelineForSendingResponseFrame">
  /// サービス要求に対する応答のECHONET Lite フレームを送信する際に発生した例外から回復するための動作を規定する<see cref="ResiliencePipeline"/>。
  /// 省略した場合は、<see langword="null"/>をデフォルト値として使用します。
  /// </param>
  /// <param name="logger">
  /// このインスタンスの動作を記録する<see cref="ILogger"/>。
  /// 省略した場合は、<see langword="null"/>をデフォルト値として使用します。
  /// </param>
  /// <param name="serviceProvider">
  /// このインスタンスで使用するサービスを取得するための<see cref="IServiceProvider"/>。
  /// </param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="selfNode"/>が<see langword="null"/>です。
  /// あるいは、<paramref name="echonetLiteHandler"/>が<see langword="null"/>です。
  /// </exception>
  [CLSCompliant(false)] // ResiliencePipeline is not CLS compliant
  public EchonetClient(
    EchonetNode selfNode,
    IEchonetLiteHandler echonetLiteHandler,
    bool shouldDisposeEchonetLiteHandler = false,
    EchonetNodeRegistry? nodeRegistry = null,
    IEchonetDeviceFactory? deviceFactory = null,
    ResiliencePipeline? resiliencePipelineForSendingResponseFrame = null,
    ILogger? logger = null,
    IServiceProvider? serviceProvider = null
  )
  {
    this.shouldDisposeEchonetLiteHandler = shouldDisposeEchonetLiteHandler;
    this.echonetLiteHandler = echonetLiteHandler ?? throw new ArgumentNullException(nameof(echonetLiteHandler));
    this.echonetLiteHandler.ReceiveCallback = HandleReceivedDataAsync;
    this.resiliencePipelineForSendingResponseFrame = resiliencePipelineForSendingResponseFrame ?? ResiliencePipeline.Empty;
    this.deviceFactory = deviceFactory;
    Logger = logger;

    SelfNode = selfNode ?? throw new ArgumentNullException(nameof(selfNode));
    SelfNode.SetOwner(this);

    this.nodeRegistry = nodeRegistry ?? new EchonetNodeRegistry();
    this.nodeRegistry.SetOwner(this);

#if SYSTEM_TIMEPROVIDER
    timeProvider = serviceProvider?.GetService<TimeProvider>();
#endif
  }

  /// <summary>
  /// 現在の<see cref="EchonetClient"/>インスタンスによって使用されているリソースを解放して、インスタンスを破棄します。
  /// </summary>
  public void Dispose()
  {
    Dispose(disposing: true);

    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// 現在の<see cref="EchonetClient"/>インスタンスによって使用されているリソースを非同期に解放して、インスタンスを破棄します。
  /// </summary>
  /// <returns>非同期の破棄操作を表す<see cref="ValueTask"/>。</returns>
  public async ValueTask DisposeAsync()
  {
    await DisposeAsyncCore().ConfigureAwait(false);

    Dispose(disposing: false);

    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// 現在の<see cref="EchonetClient"/>インスタンスが使用しているアンマネージド リソースを解放します。　オプションで、マネージド リソースも解放します。
  /// </summary>
  /// <param name="disposing">
  /// マネージド リソースとアンマネージド リソースの両方を解放する場合は<see langword="true"/>。
  /// アンマネージド リソースだけを解放する場合は<see langword="false"/>。
  /// </param>
  protected virtual void Dispose(bool disposing)
  {
    if (disposing) {
      nodeRegistry?.UnsetOwner();
      nodeRegistry = null!;

      requestSemaphore?.Dispose();
      requestSemaphore = null!;

      if (echonetLiteHandler is not null) {
        echonetLiteHandler.ReceiveCallback = null;

        if (shouldDisposeEchonetLiteHandler && echonetLiteHandler is IDisposable disposableEchonetLiteHandler)
          disposableEchonetLiteHandler.Dispose();

        echonetLiteHandler = null!;
      }
    }
  }

  /// <summary>
  /// 管理対象リソースの非同期の解放、リリース、またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
  /// </summary>
  /// <returns>非同期の破棄操作を表す<see cref="ValueTask"/>。</returns>
  protected virtual async ValueTask DisposeAsyncCore()
  {
    nodeRegistry?.UnsetOwner();
    nodeRegistry = null!;

    requestSemaphore?.Dispose();
    requestSemaphore = null!;

    if (echonetLiteHandler is not null) {
      echonetLiteHandler.ReceiveCallback = null;

      if (shouldDisposeEchonetLiteHandler && echonetLiteHandler is IAsyncDisposable disposableEchonetLiteHandler)
        await disposableEchonetLiteHandler.DisposeAsync().ConfigureAwait(false);

      echonetLiteHandler = null!;
    }
  }

  /// <summary>
  /// 現在の<see cref="EchonetClient"/>インスタンスが破棄されている場合に、<see cref="ObjectDisposedException"/>をスローします。
  /// </summary>
  /// <exception cref="ObjectDisposedException">現在のインスタンスはすでに破棄されています。</exception>
  protected void ThrowIfDisposed()
  {
    if (echonetLiteHandler is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <summary>
  /// 自ノードのアドレスを取得します。
  /// </summary>
  /// <remarks>
  /// 既定の実装では、現在のインスタンスに割り当てられている<see cref="IEchonetLiteHandler"/>からアドレスの取得を試みます。
  /// </remarks>
  /// <returns>
  /// 自ノードのアドレスを表す<see cref="IPAddress"/>。　自ノードのアドレスを規定できない場合は、<see langword="null"/>。
  /// </returns>
  /// <exception cref="ObjectDisposedException">現在のインスタンスはすでに破棄されています。</exception>
  protected internal IPAddress? GetSelfNodeAddress()
  {
    ThrowIfDisposed();

    if (echonetLiteHandler is EchonetLiteHandler handler)
      return handler.LocalAddress;

    return null;
  }

  /// <summary>
  /// <see cref="IPAddress"/>に対応する他ノードを取得または作成します。
  /// </summary>
  /// <param name="address">他ノードのアドレス。</param>
  /// <param name="esv">この要求を行う契機となったECHONETサービスを表す<see cref="ESV"/> 。</param>
  /// <returns>
  /// <paramref name="address"/>に対応する既知の他ノードを表す<see cref="EchonetOtherNode"/>、
  /// または新たに作成した未知の他ノードを表す<see cref="EchonetOtherNode"/>。
  /// </returns>
  private EchonetOtherNode GetOrAddOtherNode(IPAddress address, ESV esv)
  {
    if (NodeRegistry.TryFind(address, out EchonetOtherNode? otherNode))
      return otherNode;

    // 未知の他ノードの場合、ノードを生成
    // (ノードプロファイルのインスタンスコードは仮で0x00を指定しておき、後続のプロパティ値通知等で実際の値に更新されることを期待する)
    var newNode = new EchonetOtherNode(
      address: address,
      nodeProfile: EchonetObject.CreateNodeProfile(instanceCode: 0x00)
    );

    if (NodeRegistry.TryAdd(address, newNode, out otherNode)) {
      Logger?.LogInformation(
        "New node added (Address: {Address}, ESV: {ESV})",
        otherNode.Address,
        esv.ToSymbolString()
      );
    }

    return otherNode;
  }

  /// <summary>
  /// 他ノードに属するECHONET オブジェクトを表す<see cref="EchonetObject"/>を取得または作成します。
  /// </summary>
  /// <param name="address">他ノードのアドレス。</param>
  /// <param name="obj">他ノードのECHONET オブジェクトの<see cref="EOJ"/>。</param>
  /// <param name="esv">この要求を行う契機となったECHONETサービスを表す<see cref="ESV"/> 。</param>
  /// <returns>
  /// 該当するECHONET オブジェクトを表すsee cref="EchonetObject"/>。
  /// </returns>
  private EchonetObject GetOrAddOtherNodeObject(IPAddress address, EOJ obj, ESV esv)
    => GetOrAddOtherNode(address, esv).GetOrAddDevice(deviceFactory, obj, out _);

  IPAddress? IEchonetClientService.GetSelfNodeAddress() => GetSelfNodeAddress();
}
