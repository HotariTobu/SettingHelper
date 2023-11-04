                    { "$Name$", new __$Name$() },
        private class __$Name$ : ISettingContainer
        {
            public __$Name$()
            {
                Containers = new Dictionary<string, ISettingContainer>()
                {
$Containers$
                };

                Items = new Dictionary<string, ISettingValue>()
                {
$Items$
                };
$RootSkip$
                _$Name$ = new _$Name$(this);$RootSkip$
            }
$RootSkip$
            public readonly _$Name$ _$Name$;$RootSkip$
        }