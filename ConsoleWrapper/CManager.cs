using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleWrapper
{
    public class CManager
    {
        private readonly string _pipeHash;

        public CManager(ref string[] args)
        {
            List<string> listArgs = new List<string>(args);
            _pipeHash = listArgs.Last();
            args = listArgs.ToArray();
        }
    }
}
