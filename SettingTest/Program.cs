using System;
using System.Linq;

namespace SettingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Setting setting = new Setting("setting.xml");
            setting.Save();
        }
    }
}
