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

                wrapper.Execute();
                wrapper.OutputDataMRE.Wait();
            }
        }
    }
}
