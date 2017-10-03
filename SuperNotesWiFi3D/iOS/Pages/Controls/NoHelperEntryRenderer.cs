using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using SuperNotesWiFi3D.Pages.Controls;
using SuperNotesWiFi3D.iOS.Pages.Controls;


[assembly: ExportRenderer(typeof(NoHelperEntry), typeof(NoHelperEntryRenderer))]
namespace SuperNotesWiFi3D.iOS.Pages.Controls
{
	public class NoHelperEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			if (Control != null)
			{
				Control.SpellCheckingType = UITextSpellCheckingType.No;             // No Spellchecking
				Control.AutocorrectionType = UITextAutocorrectionType.No;           // No Autocorrection
				Control.AutocapitalizationType = UITextAutocapitalizationType.None; // No Autocapitalization
			}
		}
	}
}
