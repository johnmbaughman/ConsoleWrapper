using ConsoleWrapper;
using ConsoleWrapper.Settings;
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

                wrapper.Execute();
                wrapper.WriteToConsole(DATA);
                wrapper.OutputDataMRE.Wait();
                Assert.Equal(DATA, wrapper.OutputBuffer.ReadLine());
            }
        }

        [Fact]
        public void TestOutput()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                wrapper.Execute(EchoConsole.Program.FLOOD_KEY);
                wrapper.OutputDataMRE.Wait();
                Assert.Equal("ping", wrapper.OutputBuffer.ReadLine());
            }
        }
    }
}
