﻿using EchoDotNetLite;
using EchoDotNetLite.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoDotNetLiteLANBridge
{
    public class LANClient : IEchonetLiteFrameHandler, IDisposable
    {
        private readonly UdpClient receiveUdpClient;
        private readonly ILogger _logger;
        private const int DefaultUdpPort = 3610;
        public LANClient(ILogger<LANClient> logger)
        {
            var selfAddresses = NetworkInterface.GetAllNetworkInterfaces().SelectMany(ni => ni.GetIPProperties().UnicastAddresses.Select(ua => ua.Address));
            _logger = logger;
            try
            {
                receiveUdpClient = new UdpClient(DefaultUdpPort)
                {
                    EnableBroadcast = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception");
                throw;
            }
            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        var receivedResults = await receiveUdpClient.ReceiveAsync();
                        if (selfAddresses.Contains(receivedResults.RemoteEndPoint.Address))
                        {
                            //ブロードキャストを自分で受信(無視)
                            continue;
                        }
                        _logger.LogDebug($"UDP受信:{receivedResults.RemoteEndPoint.Address} {BytesConvert.ToHexString(receivedResults.Buffer)}");
                        DataReceived?.Invoke(this, (receivedResults.RemoteEndPoint.Address, receivedResults.Buffer.AsMemory()));
                    }
                }
                catch (System.ObjectDisposedException)
                {
                    //握りつぶす
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Exception");
                }
            });
        }

        public event EventHandler<(IPAddress, ReadOnlyMemory<byte>)> DataReceived;

        public void Dispose()
        {
            _logger.LogDebug("Dispose");
            try
            {
                receiveUdpClient?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception");
            }
        }

#nullable enable
        public async Task RequestAsync(IPAddress? address, ReadOnlyMemory<byte> request, CancellationToken cancellationToken)
        {
            IPEndPoint remote;
            if (address == null)
            {
                remote = new IPEndPoint(IPAddress.Broadcast, DefaultUdpPort);
            }
            else
            {
                remote = new IPEndPoint(address, DefaultUdpPort);
            }

            _logger.LogDebug($"UDP送信:{remote.Address} {BytesConvert.ToHexString(request.Span)}");

            var sendUdpClient = new UdpClient()
            {
                EnableBroadcast = true
            };
            sendUdpClient.Connect(remote);
#if NET6_0_OR_GREATER
            await sendUdpClient.SendAsync(request, cancellationToken);
#else
            await sendUdpClient.SendAsync(request.ToArray(), request.Length);
#endif
            sendUdpClient.Close();
        }
    }
#nullable restore
}
