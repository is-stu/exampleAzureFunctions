using System;
using todoStewar.Function.Functions;
using todoStewar.Tests.Helpers;
using Xunit;

namespace todoStewar.Tests.Tests
{
    public class TimerTest
    {

        [Fact]
        public void TimerFunction_Should_Log_Message()
        {
            // Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);


            // Act
            TimerFunction.Run(null, mockTodos, logger);
            string message = logger.Logs[0];

            // Assert

            Assert.Contains("Deleting completed", message);
        }
    }
}
