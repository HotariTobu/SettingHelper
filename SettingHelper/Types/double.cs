                    { "$Name$", new SettingValueDouble($Value$) },
            public double $Name$ { get => (double)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueDouble : ISettingValue
        {
            public SettingValueDouble(double value = 0) => Value = value;
            private double Value;

            public object Obj { get => Value; set => Value = (double)value; }
            public string Str { get => Value.ToString(); set => double.TryParse(value, out Value); }
        }