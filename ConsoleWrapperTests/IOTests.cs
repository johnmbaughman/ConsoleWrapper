using ConsoleWrapper;
using System;
using Xunit;

namespace ConsoleWrapperTests
{
    public class IOTests
    {
        [Fact]
        public void TestInput()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                const string DATA = "This data shall be echoed";
                string echoData = String.Empty;
                wrapper.OutputDataReceived += (s, e) => echoData = e.Data;
                wrapper.OutputDataReceived += Stub;

                wrapper.Execute();

                wrapper.WriteToConsole(DATA);
                DateTime startTime = DateTime.UtcNow;
                while (startTime.AddMilliseconds(10) > DateTime.UtcNow && echoData?.Length == 0)
                    continue;
                Assert.Equal(DATA, echoData);
            }
        }

        public void Stub(object sender, EventArgs e)
        {
            return;
        }
    }
}
