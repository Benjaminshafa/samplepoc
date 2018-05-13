using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pocSolution
{
    public class Publisher
    {
        EventHub EventAggregator;
        public Publisher(EventHub EA)
        {
            EventAggregator = EA;
        }

        public void PublishMessage()
        {
            EventAggregator.Publish(new SampleMessage());
            EventAggregator.Publish("This is String!");
            EventAggregator.Publish(10);
        }
    }
}
