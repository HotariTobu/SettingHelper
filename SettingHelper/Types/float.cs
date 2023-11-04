                    { "$Name$", new SettingValueFloat($Value$) },
            public float $Name$ { get => (float)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueFloat : ISettingValue
        {
            public SettingValueFloat(float value = 0) => Value = value;
            private float Value;

            public object Obj { get => Value; set => Value = (float)value; }
            public string Str { get => Value.ToString(); set => float.TryParse(value, out Value); }
        }