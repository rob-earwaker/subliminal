using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Subliminal.Tests.CSharp
{
    public class TestLog
    {
        [Fact]
        public void EmitsDataWhenLogged()
        {
            var log = new Log<object>();
            var subscriber = new Queue<object>();
            log.Subscribe(subscriber.Enqueue);
            var data1 = new object();
            var data2 = new object();
            var data3 = new object();
            log.Log(data1);
            log.Log(data2);
            log.Log(data3);
            subscriber.ToArray().Should().Equal(data1, data2, data3);
        }

        [Fact]
        public void SubscribersReceiveTheSameData()
        {
            var log = new Log<object>();
            var subscriber1 = new Queue<object>();
            var subscriber2 = new Queue<object>();
            log.Subscribe(subscriber1.Enqueue);
            log.Subscribe(subscriber2.Enqueue);
            var data1 = new object();
            var data2 = new object();
            var data3 = new object();
            log.Log(data1);
            log.Log(data2);
            log.Log(data3);
            subscriber1.ToArray().Should().Equal(data1, data2, data3);
            subscriber2.ToArray().Should().Equal(data1, data2, data3);
        }
    }
}
