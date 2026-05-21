using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Delay.Tests
{
    // CLASS 1
    public class FirstTestGroup
    {
        private readonly ITestOutputHelper _output;
        private readonly Guid _instanceId;

        public FirstTestGroup(ITestOutputHelper output)
        {
            _output = output;
            _instanceId = Guid.NewGuid(); // Generates a unique ID for this instance
        }

        [Fact]
        public void Group1_TestA() => LogDetails("Group1_TestA");

        [Fact]
        public void Group1_TestB() => LogDetails("Group1_TestB");

        private void LogDetails(string testName)
        {
            _output.WriteLine($"[{testName}] " +
                             $"Timestamp: {DateTime.Now:HH:mm:ss.fff} | " +
                             $"Thread ID: {Environment.CurrentManagedThreadId} | " +
                             $"Instance GUID: {_instanceId}");
            Thread.Sleep(500); // Hold the thread slightly to visualize overlapping
        }
    }

    // CLASS 2 (Runs in parallel with Class 1)
    public class SecondTestGroup
    {
        private readonly ITestOutputHelper _output;
        private readonly Guid _instanceId;

        public SecondTestGroup(ITestOutputHelper output)
        {
            _output = output;
            _instanceId = Guid.NewGuid();
        }

        [Fact]
        public void Group2_TestA() => LogDetails("Group2_TestA");

        private void LogDetails(string testName)
        {
            _output.WriteLine($"[{testName}] " +
                             $"Timestamp: {DateTime.Now:HH:mm:ss.fff} | " +
                             $"Thread ID: {Environment.CurrentManagedThreadId} | " +
                             $"Instance GUID: {_instanceId}");
            Thread.Sleep(500);
        }
    }
}