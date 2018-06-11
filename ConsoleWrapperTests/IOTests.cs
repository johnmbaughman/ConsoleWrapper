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
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out));
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                const string DATA = "This data shall be echoed";
                string echoData = String.Empty;
                //wrapper.OutputDataReceived += (s, e) => echoData = e.Data;
                //wrapper.OutputDataReceived += (s, e) => Assert.Equal(DATA, e.Data);

                wrapper.Execute(EchoConsole.Program.FLOOD_KEY);

                //wrapper.WriteToConsole(DATA);
                /*DateTime startTime = DateTime.UtcNow;
                while (startTime.AddMilliseconds(10) > DateTime.UtcNow && echoData?.Length == 0)
                    continue;
                Assert.Equal(DATA, echoData);*/
            }
        }
    }
}
