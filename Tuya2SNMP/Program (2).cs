using McMaster.Extensions.CommandLineUtils;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Reflection.Metadata;
using Tuya2SNMP;
using Tuya2SNMP.Tuya;

namespace com.clusterrr.TuyaNet
{
    class Program
    {
        static int Main(string[] args)
        {

            string localKeyStr = "(xw:66YQLil-h8mj";
            byte[] localKey = Encoding.UTF8.GetBytes(localKeyStr);

    
            TuyaDeviceV34 td = new("192.168.1.122", localKeyStr, deviceId);
            td.ConnectAsync().Wait();

            Thread.Sleep(2000);
            //td.SetDpAsync(104, "RGBW").Wait();
            //td.QueryDps().Wait();

            Thread thr = new(() => { while (true) {
                    //td.SendHeartbeat().Wait();
                    Thread.Sleep(1000);
                } });
            thr.Start();
            return 1;

        }

    }
}
