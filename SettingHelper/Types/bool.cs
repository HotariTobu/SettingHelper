                    { "$Name$", new SettingValueBool($Value$) },
            public bool $Name$ { get => (bool)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueBool : ISettingValue
        {
            public SettingValueBool(bool value = false) => Value = value;
            private bool Value;

            public object Obj { get => Value; set => Value = (bool)value; }
            public string Str { get => Value.ToString(); set => bool.TryParse(value, out Value); }
        }