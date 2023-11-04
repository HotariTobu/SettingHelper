                    { "$Name$", new SettingValueInt($Value$) },
            public int $Name$ { get => (int)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueInt : ISettingValue
        {
            public SettingValueInt(int value = 0) => Value = value;
            private int Value;

            public object Obj { get => Value; set => Value = (int)value; }
            public string Str { get => Value.ToString(); set => int.TryParse(value, out Value); }
        }