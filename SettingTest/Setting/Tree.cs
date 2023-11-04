using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SettingTest.Setting
{
    public class Tree : ISettingContainer
    {
        #region == Accessors ==

        public bool AAA = false;
        public FFF.FFF FFF = new FFF.FFF();

        #endregion

        #region == Properties ==

        #endregion

        #region == Updaters ==

        #endregion

        #region == InternalAccessors ==

        public IEnumerable<ISettingContainer> Containers => new ISettingContainer[]
        {
            FFF,
        };

        public IEnumerable<string> Keys => _Keys;
        private IEnumerable<string> _Keys = new string[]
        {
            "AAA",
        };

        public string GetValue(string key) => key switch
        {
            "AAA" => AAA.ToSettingString(),
            _ => null
        };

        public bool SetValue(string key, string value) => key switch
        {
            "AAA" => AAA.FromSettingString(value),
            _ => false
        };

        #endregion
    }
}