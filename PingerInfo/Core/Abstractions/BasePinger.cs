using PingerInfo.Core.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;

namespace PingerInfo.Core.Abstractions
{
    public abstract class BasePinger
    {
        public int Period { get; private set; }

        public int Timeout { get; set; }

        protected BasePinger(int period, int timeout)
        {
            Period = period == 0 ? 10000 : period;
            Timeout = timeout == 0 ? 500 : timeout;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                await UpdateAsync();
                await Task.Delay(Period);
            }
        }
        
        /// <summary>
        /// Метод который будет вызваться таймером каждые N секунд, в нем идет обработка  
        /// </summary>
        /// <returns></returns>
        protected abstract Task UpdateAsync();

        protected abstract Task PingDoneAsync(List<PingObject> pingObjects);

        protected abstract Task ReceivePacketAsync(PingObject pingObject, PingReply pingReply);
    }
}
