using Xamarin.Forms;


namespace SuperNotesWiFi3D.Pages
{
    public partial class RootNavigationPage : NavigationPage
    {
        public RootNavigationPage() : base(new HomePage())
        {
            //NOTE: this constructor is for XAML previewer only

			InitializeComponent();
            CommonInit();
        }

        public RootNavigationPage(Page root) : base(root)
        {
            InitializeComponent();
            CommonInit();
        }

        private void CommonInit()
        {
            SetHasNavigationBar(this, false);
        }
    }
}
