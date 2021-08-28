using System;
using System.Collections.Generic;
using System.Text;

namespace Personnel.Tracker.Common.RestEase
{
    public class RestEaseOptions
    {
        public string LoadBalancer { get; set; }
        public IEnumerable<Service> Services { get; set; }

        public class Service
        {
            public string Name { get; set; }
            public string Scheme { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public string Path { get; set; }
        }
    }
}
