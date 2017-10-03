using System.Runtime.InteropServices;

namespace SuperNotesWiFi3D.Droid.libharu
{
    public static class HPDF_Doc
    {
		// extern HPDF_Doc HPDF_New (HPDF_Error_Handler user_error_fn, void *user_data);
		[DllImport("libharu.so", EntryPoint = "HPDF_New")]
		public static extern unsafe void* HPDF_New(void* user_error_fn, void* user_data);

		// extern void HPDF_Free (HPDF_Doc pdf);
		[DllImport("libharu.so", EntryPoint = "HPDF_Free")]
		public static extern unsafe void HPDF_Free(void* pdf);

		// extern HPDF_STATUS HPDF_SaveToFile (HPDF_Doc pdf, const char *file_name);
		[DllImport("libharu.so", EntryPoint = "HPDF_SaveToFile")]
		public static extern unsafe uint HPDF_SaveToFile(void* pdf, sbyte* file_name);

        /// <summary>
        /// Page related functions.
        /// </summary>
		public static class Page
		{
			// extern HPDF_Page HPDF_AddPage (HPDF_Doc pdf);
			[DllImport("libharu.so", EntryPoint = "HPDF_AddPage")]
			public static extern unsafe void* HPDF_AddPage(void* pdf);

			// extern HPDF_STATUS HPDF_Page_SetWidth (HPDF_Page page, HPDF_REAL value);
			[DllImport("libharu.so", EntryPoint = "HPDF_Page_SetWidth")]
			public static extern unsafe uint HPDF_Page_SetWidth(void* page, float value);

			// extern HPDF_STATUS HPDF_Page_SetHeight (HPDF_Page page, HPDF_REAL value);
			[DllImport("libharu.so", EntryPoint = "HPDF_Page_SetHeight")]
			public static extern unsafe uint HPDF_Page_SetHeight(void* page, float value);
		}

		/// Functions related to text
		public static class Text
		{
			// extern HPDF_STATUS HPDF_Page_BeginText (HPDF_Page page);
			[DllImport("libharu.so", EntryPoint = "HPDF_Page_BeginText")]
			public static extern unsafe uint HPDF_Page_BeginText(void* page);

			// extern HPDF_STATUS HPDF_Page_EndText (HPDF_Page page);
			[DllImport("libharu.so", EntryPoint = "HPDF_Page_EndText")]
			public static extern unsafe uint HPDF_Page_EndText(void* page);

			public static class TextState
			{
				// extern HPDF_STATUS HPDF_Page_SetFontAndSize (HPDF_Page page, HPDF_Font font, HPDF_REAL size);
				[DllImport("libharu.so", EntryPoint = "HPDF_Page_SetFontAndSize")]
				static extern unsafe uint HPDF_Page_SetFontAndSize(void* page, void* font, float size);
			}

			public static class Font
			{
				// extern HPDF_Font HPDF_GetFont (HPDF_Doc pdf, const char *font_name, const char *encoding_name);
				[DllImport("libharu.so", EntryPoint = "HPDF_GetFont")]
				public static extern unsafe void* HPDF_GetFont(void* pdf, sbyte* font_name, sbyte* encoding_name);
			}
		}

		public static class Image
		{
			// extern HPDF_Image HPDF_LoadJpegImageFromFile (HPDF_Doc pdf, const char *filename);
			[DllImport("libharu.so", EntryPoint = "HPDF_LoadJpegImageFromFile")]
			public static extern unsafe void* HPDF_LoadJpegImageFromFile(void* pdf, sbyte* filename);

			// extern HPDF_STATUS HPDF_Page_DrawImage (HPDF_Page page, HPDF_Image image, HPDF_REAL x, HPDF_REAL y, HPDF_REAL width, HPDF_REAL height);
			[DllImport("libharu.so", EntryPoint = "HPDF_Page_DrawImage")]
			public static extern unsafe uint HPDF_Page_DrawImage(void* page, void* image, float x, float y, float width, float height);

			// extern HPDF_UINT HPDF_Image_GetWidth (HPDF_Image image);
			[DllImport("libharu.so", EntryPoint = "HPDF_Image_GetWidth")]
			public static extern unsafe uint HPDF_Image_GetWidth(void* image);

			// extern HPDF_UINT HPDF_Image_GetHeight (HPDF_Image image);
			[DllImport("libharu.so", EntryPoint = "HPDF_Image_GetHeight")]
			public static extern unsafe uint HPDF_Image_GetHeight(void* image);
		}
    }
}
