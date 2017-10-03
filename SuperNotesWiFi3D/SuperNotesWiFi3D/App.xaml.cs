using Xamarin.Forms;
using SuperNotesWiFi3D.Pages;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D
{
    public partial class App : Application
    {
        public static string AppName { get { return "com.shakuro.SuperNotesWiFi3D"; } }


        private static bool _initialized = false;
        public static ICameraPageHelper CameraPageHelper { get; private set; }
        public static IFileHelper FileHelper { get; private set; }
        public static StyleHelper StyleHelper { get; private set; }
        public static IFacebookApiClient FacebookApiClient { get; private set; }
        public static IPDFHelper PDFHelper { get; private set; }
        public static IEMailHelper EMailHelper { get; private set; }
        public static ITestHelper TestHelper { get; private set; }

        public static void Init(ICameraPageHelper cameraPageHelper,
                                IFileHelper fileHelper,
                                StyleHelper styleHelper,
                                IFacebookApiClient facebookApiClient,
                                IPDFHelper pdfHelper,
                                IEMailHelper eMailHelper,
                                ITestHelper testHelper)
        {
            if (!_initialized)
            {
                _initialized = true;
                CameraPageHelper = cameraPageHelper;
                FileHelper = fileHelper;
                StyleHelper = styleHelper;
                FacebookApiClient = facebookApiClient;
                PDFHelper = pdfHelper;
                EMailHelper = eMailHelper;
                TestHelper = testHelper;
            }
        }


        public App()
        {
            InitializeComponent();

            // display root page
            MainPage = new RootNavigationPage(new HomePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
