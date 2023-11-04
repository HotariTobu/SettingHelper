                    { "$Name$", new SettingValueString($Value$) },
            public string $Name$ { get => (string)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueString : ISettingValue
        {
            public SettingValueString(string value = "") => Value = value;
            private string Value;

            public object Obj { get => Value; set => Value = (string)value; }
            public string Str { get => Value; set => Value = value; }
        }