// SPDX-FileCopyrightText: 2018 HiroyukiSakoh
// SPDX-FileCopyrightText: 2024 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Net.EchonetLite.Protocol;

public enum EHD2 : byte
{
    //図 ３-３ EHD2 詳細規定
    /// <summary>
    /// 形式1
    /// </summary>
    Type1 = 0x81,
    /// <summary>
    /// 形式2
    /// </summary>
    Type2 = 0x82,
    //その他:future reserved
    //ただし、b7=1固定
}
