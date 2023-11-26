﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EchoDotNetLite.Models
{

    /// <summary>
    /// 電文形式２（任意電文形式）
    /// </summary>
    public class EDATA2 : IEDATA
    {
        /// <summary>
        /// ECHONET Liteフレームの電文形式２（任意電文形式）の電文を記述する<see cref="EDATA2"/>を作成します。
        /// </summary>
        /// <param name="message"><see cref="Message"/>に指定する値。</param>
        public EDATA2(ReadOnlyMemory<byte> message)
        {
            Message = message;
        }

        /// <summary>
        /// 任意電文形式の電文を表す<see cref="ReadOnlyMemory{byte}"/>。
        /// </summary>
        public ReadOnlyMemory<byte> Message { get; }
    }
}
