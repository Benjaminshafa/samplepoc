using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pocSolution
{
    public class Subscriber
    {
        private Subscription<SampleMessage> _myMessageToken;
        Subscription<int> _intSubsriber;
        readonly EventHub _eventAggregator;
        public Subscriber(EventHub eventHub)
        {
            _eventAggregator = eventHub;
            eventHub.Subscribe<SampleMessage>(this.Test);
            eventHub.Subscribe<int>(this.IntTest);
            eventHub.Subscribe<string>(this.StringTest);
        }

        private void IntTest(int obj)
        {
            Console.WriteLine(obj);
            _eventAggregator.UnSbscribe(_intSubsriber);
        }

        private void Test(SampleMessage test)
        {
            Console.WriteLine(test.ToString());
            _eventAggregator.UnSbscribe(_myMessageToken);
        }
        private void StringTest(string test)
        {
            Console.WriteLine(test);
            _eventAggregator.UnSbscribe(_myMessageToken);
        }
    }
}
