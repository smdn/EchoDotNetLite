// SPDX-FileCopyrightText: 2018 HiroyukiSakoh
// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

using Smdn.Net.EchonetLite;
using Smdn.Net.EchonetLite.Appendix;

namespace EchoDotNetLite.Models
{
    /// <summary>
    /// ECHONET Liteノード
    /// </summary>
    public sealed class EchoNode
    {
        public EchoNode(IPAddress address, EchoObjectInstance nodeProfile)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            NodeProfile = nodeProfile ?? throw new ArgumentNullException(nameof(nodeProfile));

            var devices = new ObservableCollection<EchoObjectInstance>();

            devices.CollectionChanged += (_, e) => OnDevicesChanged(e);

            Devices = devices;
        }

        private void OnDevicesChanged(NotifyCollectionChangedEventArgs e)
        {
            DevicesChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 下位スタックのアドレス
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// ノードプロファイルオブジェクト
        /// </summary>
        public EchoObjectInstance NodeProfile { get; }

        /// <summary>
        /// 機器オブジェクトのリスト
        /// </summary>
        public ICollection<EchoObjectInstance> Devices { get;  }

        /// <summary>
        /// 機器オブジェクトのリスト<see cref="Devices"/>に変更があったときに発生するイベント。
        /// </summary>
        /// <remarks>
        /// 現在のノードにECHONET Lite オブジェクトが追加・削除された際にイベントが発生します。
        /// 変更の詳細は、イベント引数<see cref="NotifyCollectionChangedEventArgs"/>を参照してください。
        /// </remarks>
        public event NotifyCollectionChangedEventHandler? DevicesChanged;
    }


    internal static class SpecificationUtil
    {
        public static EchoProperty? FindProperty(byte classGroupCode, byte classCode, byte epc)
        {
            var @class = FindClass(classGroupCode, classCode);
            if (@class is not null)
            {
                EchoProperty? property;
                 property = @class.AnnoProperties.FirstOrDefault(p => p.Code == epc);
                if (property is not null)
                {
                    return property;
                }
                property = @class.GetProperties.FirstOrDefault(p => p.Code == epc);
                if (property is not null)
                {
                    return property;
                }
                property = @class.SetProperties.FirstOrDefault(p => p.Code == epc);
                if (property is not null)
                {
                    return property;
                }
            }
            return null;
        }

        internal static IEchonetObject GenerateUnknownClass(byte classGroupCode, byte classCode)
        {
            return new UnknownEchoObject
            (
                classGroup: new EchoClassGroup
                (
                    classGroupCode: classGroupCode,
                    classGroupName: "Unknown",
                    classGroupNameOfficial: "Unknown",
                    classList: Array.Empty<EchoClass>(),
                    superClass: null
                ),
                @class: new EchoClass
                (
                    classCode: classCode,
                    className: "Unknown",
                    classNameOfficial: "Unknown",
                    status: false
                )
            );
        }
        private class UnknownEchoObject : IEchonetObject
        {
            public UnknownEchoObject(EchoClassGroup classGroup, EchoClass @class)
            {
                ClassGroup = classGroup ?? throw new ArgumentNullException(nameof(classGroup));
                Class = @class ?? throw new ArgumentNullException(nameof(@class));
            }

            public EchoClassGroup ClassGroup { get; }
            public EchoClass Class { get; }

            public IEnumerable<EchoProperty> GetProperties => Enumerable.Empty<EchoProperty>();

            public IEnumerable<EchoProperty> SetProperties => Enumerable.Empty<EchoProperty>();

            public IEnumerable<EchoProperty> AnnoProperties => Enumerable.Empty<EchoProperty>();
        }

        public static IEchonetObject? FindClass(byte classGroupCode, byte classCode)
        {
            var profileClass = プロファイル.クラス一覧.FirstOrDefault(
                                g => g.ClassGroup.ClassGroupCode == classGroupCode
                                && g.Class.ClassCode == classCode);
            if (profileClass != null)
            {
                return profileClass;
            }
            var deviceClass = 機器.クラス一覧.FirstOrDefault(
                                g => g.ClassGroup.ClassGroupCode == classGroupCode
                                && g.Class.ClassCode == classCode);
            if (deviceClass != null)
            {
                return deviceClass;
            }
            return null;
        }
    }
}
