                    { "$Name$", new SettingValueByte($Value$) },
            public byte $Name$ { get => (byte)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueByte : ISettingValue
        {
            public SettingValueByte(byte value = 0) => Value = value;
            private byte Value;

            public object Obj { get => Value; set => Value = (byte)value; }
            public string Str { get => Value.ToString(); set => byte.TryParse(value, out Value); }
        }