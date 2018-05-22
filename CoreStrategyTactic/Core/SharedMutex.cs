using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core
{
    public static class SharedMutex
    {
        static Mutex mutex = new Mutex();
        
        public static Mutex getMutex()
        {
            return mutex;
        }
    }
}
