                    { "$Name$", new SettingValueColor($Value$) },
            public Color $Name$ { get => (Color)___$Container$.Items["$Name$"].Obj; set => ___$Container$.Items["$Name$"].Obj = value; }
        private class SettingValueColor : ISettingValue
        {
            public SettingValueColor(Color value) => Value = value;
            private Color Value;

            public object Obj { get => Value; set => Value = (Color)value; }
            public string Str
            {
                get => $"{Value.A}, {Value.R}, {Value.G}, {Value.B}";
                set
                {
                    string[] vs = value?.Split(", ");
                    if (vs != null && vs.Length == 4 && byte.TryParse(vs[0], out byte a) && byte.TryParse(vs[1], out byte r) && byte.TryParse(vs[2], out byte g) && byte.TryParse(vs[3], out byte b))
                    {
                        Value = Color.FromArgb(a, r, g, b);
                    }
                }
            }
        }