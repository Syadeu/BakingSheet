using System;
using Xunit;
using Cathei.BakingSheet.Internal;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cathei.BakingSheet.Tests
{
    public static class TestExtensions
    {
        public static void VerifyLog(this Mock<ILogger> loggerMock, LogLevel level, string message, Times times)
        {
            loggerMock.Verify(m => m.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((s, _) => VerifyLogState(s, message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ), times);
        }

        private static bool VerifyLogState(object state, string expected)
        {

            return state.ToString() == expected;
        }
    }
}
