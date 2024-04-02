// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;

namespace Smdn.Net.EchonetLite;

#pragma warning disable IDE0040
partial class EchonetClient
#pragma warning restore IDE0040
{
    /// <summary>
    /// 新しいECHONET Lite ノードが発見されたときに発生するイベント。
    /// </summary>
    public event EventHandler<EchonetNode>? NodeJoined;

    /// <summary>
    /// インスタンスリスト通知の受信による更新を開始するときに発生するイベント。
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   イベント引数には、インスタンスリスト通知の送信元のECHONET Lite ノードを表す<see cref="EchonetNode"/>が設定されます。
    ///   </para>
    ///   <para>
    ///   インスタンスリスト通知を受信した場合、以下の順でイベントが発生します。
    ///     <list type="number">
    ///       <item><description><see cref="InstanceListUpdating"/></description></item>
    ///       <item><description><see cref="InstanceListPropertyMapAcquiring"/></description></item>
    ///       <item><description><see cref="InstanceListUpdated"/></description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// <seealso cref="InstanceListPropertyMapAcquiring"/>
    /// <seealso cref="InstanceListUpdated"/>
    public event EventHandler<EchonetNode>? InstanceListUpdating;

    /// <summary>
    /// インスタンスリスト通知を受信した際に、プロパティマップの取得を開始するときに発生するイベント。
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   イベント引数には、<see cref="ValueTuple{T1,T2}"/>が設定されます。
    ///   イベント引数は、インスタンスリスト通知の送信元のECHONET Lite ノードを表す<see cref="EchonetNode"/>、
    ///   および通知されたインスタンスリストを表す<see cref="IReadOnlyList{EchonetObject}"/>を保持します。
    ///   </para>
    ///   <para>
    ///   インスタンスリスト通知を受信した場合、以下の順でイベントが発生します。
    ///     <list type="number">
    ///       <item><description><see cref="InstanceListUpdating"/></description></item>
    ///       <item><description><see cref="InstanceListPropertyMapAcquiring"/></description></item>
    ///       <item><description><see cref="InstanceListUpdated"/></description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// <seealso cref="InstanceListUpdating"/>
    /// <seealso cref="InstanceListUpdated"/>
    public event EventHandler<(EchonetNode, IReadOnlyList<EchonetObject>)>? InstanceListPropertyMapAcquiring;

    /// <summary>
    /// インスタンスリスト通知の受信による更新が完了したときに発生するイベント。
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   イベント引数には、<see cref="ValueTuple{EchonetNode,T2}"/>が設定されます。
    ///   イベント引数は、インスタンスリスト通知の送信元のECHONET Lite ノードを表す<see cref="EchonetNode"/>、
    ///   および通知されたインスタンスリストを表す<see cref="IReadOnlyList{EchonetObject}"/>を保持します。
    ///   </para>
    ///   <para>
    ///   インスタンスリスト通知を受信した場合、以下の順でイベントが発生します。
    ///     <list type="number">
    ///       <item><description><see cref="InstanceListUpdating"/></description></item>
    ///       <item><description><see cref="InstanceListPropertyMapAcquiring"/></description></item>
    ///       <item><description><see cref="InstanceListUpdated"/></description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// <seealso cref="InstanceListUpdating"/>
    /// <seealso cref="InstanceListPropertyMapAcquiring"/>
    public event EventHandler<(EchonetNode, IReadOnlyList<EchonetObject>)>? InstanceListUpdated;

    protected virtual void OnInstanceListUpdating(EchonetNode node)
        => InstanceListUpdating?.Invoke(this, node);

    protected virtual void OnInstanceListPropertyMapAcquiring(EchonetNode node, IReadOnlyList<EchonetObject> instances)
        => InstanceListPropertyMapAcquiring?.Invoke(this, (node, instances));

    protected virtual void OnInstanceListUpdated(EchonetNode node, IReadOnlyList<EchonetObject> instances)
        => InstanceListUpdated?.Invoke(this, (node, instances));

    /// <summary>
    /// プロパティマップの取得を開始するときに発生するイベント。
    /// </summary>
    /// <remarks>
    /// イベント引数には、<see cref="ValueTuple{EchonetNode,EchonetObject}"/>が設定されます。
    /// イベント引数は、対象オブジェクトが属するECHONET Lite ノードを表す<see cref="EchonetNode"/>、
    /// およびプロパティマップ取得対象のECHONET Lite オブジェクトを表す<see cref="EchonetObject"/>を保持します。
    /// </remarks>
    /// <seealso cref="PropertyMapAcquired"/>
    /// <seealso cref="EchonetObject.HasPropertyMapAcquired"/>
    public event EventHandler<(EchonetNode, EchonetObject)>? PropertyMapAcquiring;

    /// <summary>
    /// プロパティマップの取得を完了したときに発生するイベント。
    /// </summary>
    /// <remarks>
    /// イベント引数には、<see cref="ValueTuple{EchonetNode,EchonetObject}"/>が設定されます。
    /// イベント引数は、対象オブジェクトが属するECHONET Lite ノードを表す<see cref="EchonetNode"/>、
    /// およびプロパティマップ取得対象のECHONET Lite オブジェクトを表す<see cref="EchonetObject"/>を保持します。
    /// </remarks>
    /// <seealso cref="PropertyMapAcquiring"/>
    /// <seealso cref="EchonetObject.HasPropertyMapAcquired"/>
    public event EventHandler<(EchonetNode, EchonetObject)>? PropertyMapAcquired;

    protected virtual void OnPropertyMapAcquiring(EchonetNode node, EchonetObject device)
        => PropertyMapAcquiring?.Invoke(this, (node, device));

    protected virtual void OnPropertyMapAcquired(EchonetNode node, EchonetObject device)
        => PropertyMapAcquired?.Invoke(this, (node, device));
}
