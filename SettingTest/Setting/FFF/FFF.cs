using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SettingTest.Setting.FFF
{
    public class FFF : ISettingContainer
    {
        #region == Accessors ==

        public byte BBB = 200;
        public byte[] CCC = { 20, 40, 60, 80, };

        #endregion

        #region == Properties ==

        #endregion

        #region == Updaters ==

        #endregion

        #region == InternalAccessors ==

        public IEnumerable<ISettingContainer> Containers => new ISettingContainer[]
        {

        };

        public IEnumerable<string> Keys => _Keys;
        private IEnumerable<string> _Keys = new string[]
        {
            "BBB",
        };

        public string GetValue(string key) => key switch
        {
            "BBB" => BBB.ToSettingString(),
            "CCC" => CCC.ToSettingString(),
            _ => null
        };

        public bool SetValue(string key, string value) => key switch
        {
            "BBB" => BBB.FromSettingString(value),
            _ => false
        };

        #endregion
    }
}