using System;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D.Droid.Model
{
    public class TestHelperAndroid : ITestHelper
    {
        bool ITestHelper.isAvailable()
        {
            return true;
        }

        string ITestHelper.TestFunc()
        {
            return Org.Apache.Taglibs.String.Util.Metaphone.MetaPhone("test");
        }
    }
}
