﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoDotNetLite
{

    public interface IEchonetLiteFrameHandler
    {
        Task RequestAsync(IPAddress? address, ReadOnlyMemory<byte> request, CancellationToken cancellationToken);

        event EventHandler<(IPAddress Address, ReadOnlyMemory<byte> Data)> DataReceived;
    }
}
