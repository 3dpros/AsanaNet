using System;
using Xunit;
using AsanaAPI;
using System.Collections.Generic;

namespace AsanaWrapperTests
{
    public class BasicTests
    {
        [Fact]
        public void CanConnect()
        {
            var sut = new AsanaWorkSpace("Test");

            sut.AddTask($"test task {DateTime.UtcNow.Ticks.ToString()}", "dummy text", new List<string>() { "Test Project" }, DateTime.Now.AddDays(5));

        }
    }
}
