                    { "$Name$", new SettingValueShort($Value$) },
            public short $Name$ { get => (short)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueShort : ISettingValue
        {
            public SettingValueShort(short value = 0) => Value = value;
            private short Value;

            public object Obj { get => Value; set => Value = (short)value; }
            public string Str { get => Value.ToString(); set => short.TryParse(value, out Value); }
        }