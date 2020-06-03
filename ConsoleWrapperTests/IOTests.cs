using ConsoleWrapper;
using ConsoleWrapper.Settings;
using Xunit;

namespace ConsoleWrapperTests
{
    public class IOTests
    {
        private static readonly WrapperSettings REDIRECTED_SETTINGS = new WrapperSettings
        {
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        [Fact]
        public void TestInput()
        {
            const string ECHO_DATA = "This data shall be echoed";
            string returnData = null;

            using CWrapper wrapper = GetWrapper();
            wrapper.OutputDataReceived += (_, e) => returnData = e.Data;

            wrapper.Execute();
            wrapper.WriteToConsole(ECHO_DATA);
            wrapper.OutputDataMRE.Wait();
            wrapper.Kill();

            Assert.Equal(ECHO_DATA, returnData);
        }

        [Fact]
        public void TestOutput()
        {
            using CWrapper wrapper = GetWrapper();

            string returnData = null;
            wrapper.OutputDataReceived += (_, e) => returnData = e.Data;

            wrapper.Execute(EchoConsole.Program.FLOOD_KEY);
            wrapper.OutputDataMRE.Wait();
            wrapper.Kill();

            Assert.Equal(EchoConsole.Program.FLOOD_OUTPUT, returnData);
        }

        private CWrapper GetWrapper() => new CWrapper(Constants.ECHO_CONSOLE_LOCATION, REDIRECTED_SETTINGS);
    }
}
