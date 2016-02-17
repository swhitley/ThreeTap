using System;

namespace ThreeTap
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public EventArgs(T val)
        {
            Value = val;
        }
    }
    public class EventArgs<T1, T2> : EventArgs
    {
        public T1 Value1 { get; private set; }
        public T2 Value2 { get; private set; }

        public EventArgs(T1 val1, T2 val2)
        {
            Value1 = val1;
            Value2 = val2;
        }
    }
}
