                    { "$Name$", new SettingValueLong($Value$) },
            public long $Name$ { get => (long)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueLong : ISettingValue
        {
            public SettingValueLong(long value = 0) => Value = value;
            private long Value;

            public object Obj { get => Value; set => Value = (long)value; }
            public string Str { get => Value.ToString(); set => long.TryParse(value, out Value); }
        }