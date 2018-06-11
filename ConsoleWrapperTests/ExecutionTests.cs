using ConsoleWrapper;
using System;
using Xunit;

namespace ConsoleWrapperTests
{
    public class ExecutionTests
    {
        [Fact]
        public void TestExecute()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                wrapper.Execute();
                Assert.True(wrapper.Executing);
                wrapper.Kill();
            }
        }

        [Fact]
        public void TestKill()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                wrapper.Execute();
                wrapper.Kill();
                Assert.False(wrapper.Executing);
            }
        }

        [Fact]
        public void TestInvalidExecute()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                wrapper.Execute();
                Assert.Throws<InvalidOperationException>(() => wrapper.Execute());
            }
        }

        [Fact]
        public void TestInvalidFilePath()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CWrapper wrapper = new CWrapper("random location");
            });
        }

        [Fact]
        public void TestInvalidKill()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                Assert.Throws<InvalidOperationException>(() => wrapper.Kill());
            }

            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                wrapper.Execute();
                wrapper.Kill();
                Assert.Throws<InvalidOperationException>(() => wrapper.Kill());
            }
        }

        [Fact]
        public void TestKillEventFired()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                bool eventFired = false;
                wrapper.Killed += (s, e) => eventFired = true;
                wrapper.Execute();
                wrapper.Kill();
                Assert.True(eventFired);
            }
        }

        [Fact]
        public void TestExitEventFired()
        {
            using (CWrapper wrapper = new CWrapper(Constants.ECHO_CONSOLE_LOCATION))
            {
                bool eventFired = false;
                wrapper.Exited += (s, e) => eventFired = true;
                wrapper.Execute(EchoConsole.Program.EXIT_KEY);
                wrapper.Kill();
                Assert.True(eventFired);
            }
        }
    }
}
