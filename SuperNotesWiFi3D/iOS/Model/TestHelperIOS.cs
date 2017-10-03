using System;
using SuperNotesWiFi3D.Model;

namespace SuperNotesWiFi3D.iOS.Model
{
    public class TestHelperIOS : ITestHelper
    {
        bool ITestHelper.isAvailable()
        {
            return false;
        }

        string ITestHelper.TestFunc()
        {
            throw new NotImplementedException();
        }
    }
}
