using System;
using Foundation;
using ObjCRuntime;


namespace SuperNotesWiFi3D.iOS.SMTPLite
{
	// typedef void (^SMTPProgressCallback)(SMTPMessage *, double, double);
	delegate void SMTPProgressCallback(SMTPMessage arg0, double arg1, double arg2);

	// typedef void (^SMTPSuccessCallback)(SMTPMessage *);
	delegate void SMTPSuccessCallback(SMTPMessage arg0);

	// typedef void (^SMTPFailureCallback)(SMTPMessage *, NSError *);
	delegate void SMTPFailureCallback(SMTPMessage arg0, NSError arg1);

	// @interface SMTPAttachment : NSObject
	[BaseType(typeof(NSObject))]
	interface SMTPAttachment
	{
		// @property (copy, nonatomic) NSString * name;
		[Export("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * filePath;
		[Export("filePath")]
		string FilePath { get; set; }
	}

	// @interface SMTPMessage : NSObject
	[BaseType(typeof(NSObject))]
	interface SMTPMessage
	{
		// @property (copy, nonatomic) NSString * from;
		[Export("from")]
		string From { get; set; }

		// @property (copy, nonatomic) NSString * to;
		[Export("to")]
		string To { get; set; }

		// @property (copy, nonatomic) NSArray * ccs;
		[Export("ccs", ArgumentSemantic.Copy)]
		string[] CCs { get; set; }

		// @property (copy, nonatomic) NSArray * bccs;
		[Export("bccs", ArgumentSemantic.Copy)]
		string[] BCCs { get; set; }

		// @property (copy, nonatomic) NSString * subject;
		[Export("subject")]
		string Subject { get; set; }

		// @property (copy, nonatomic) NSString * content;
		[Export("content")]
		string Content { get; set; }

		// @property (copy, nonatomic) NSString * contentType;
		[Export("contentType")]
		string ContentType { get; set; }

		// @property (copy, nonatomic) NSArray * attachments;
		[Export("attachments", ArgumentSemantic.Copy)]
		SMTPAttachment[] Attachments { get; set; }

		// @property (copy, nonatomic) NSString * account;
		[Export("account")]
		string Account { get; set; }

		// @property (copy, nonatomic) NSString * pwd;
		[Export("pwd")]
		string Pwd { get; set; }

		// @property (copy, nonatomic) NSString * host;
		[Export("host")]
		string Host { get; set; }

		// @property (copy, nonatomic) NSString * port;
		[Export("port")]
		string Port { get; set; }

		// @property (assign, nonatomic) BOOL ssl;
		[Export("ssl")]
		bool Ssl { get; set; }

		// @property (assign, nonatomic) long timeout;
		[Export("timeout")]
		nint Timeout { get; set; }

		// @property (assign, nonatomic) long connectTimeout;
		[Export("connectTimeout")]
		nint ConnectTimeout { get; set; }

		// @property (copy, nonatomic) SMTPProgressCallback progressCallback;
		[Export("progressCallback", ArgumentSemantic.Copy)]
		SMTPProgressCallback ProgressCallback { get; set; }

		// @property (copy, nonatomic) SMTPSuccessCallback successCallback;
		[Export("successCallback", ArgumentSemantic.Copy)]
		SMTPSuccessCallback SuccessCallback { get; set; }

		// @property (copy, nonatomic) SMTPFailureCallback failureCallback;
		[Export("failureCallback", ArgumentSemantic.Copy)]
		SMTPFailureCallback FailureCallback { get; set; }

		// @property (assign, nonatomic) BOOL cancel;
		[Export("cancel")]
		bool Cancel { get; set; }

		// @property (nonatomic, strong) NSMutableData * response;
		[Export("response", ArgumentSemantic.Strong)]
		NSMutableData Response { get; set; }

		// -(void)send:(void (^)(SMTPMessage *, double, double))progressCallback success:(void (^)(SMTPMessage *))successCallback failure:(void (^)(SMTPMessage *, NSError *))failureCallback;
		[Export("send:success:failure:")]
		void Send(Action<SMTPMessage, double, double> progressCallback, Action<SMTPMessage> successCallback, Action<SMTPMessage, NSError> failureCallback);
	}

    // The first step to creating a binding is to add your native library ("libNativeLibrary.a")
    // to the project by right-clicking (or Control-clicking) the folder containing this source
    // file and clicking "Add files..." and then simply select the native library (or libraries)
    // that you want to bind.
    //
    // When you do that, you'll notice that MonoDevelop generates a code-behind file for each
    // native library which will contain a [LinkWith] attribute. MonoDevelop auto-detects the
    // architectures that the native library supports and fills in that information for you,
    // however, it cannot auto-detect any Frameworks or other system libraries that the
    // native library may depend on, so you'll need to fill in that information yourself.
    //
    // Once you've done that, you're ready to move on to binding the API...
    //
    //
    // Here is where you'd define your API definition for the native Objective-C library.
    //
    // For example, to bind the following Objective-C class:
    //
    //     @interface Widget : NSObject {
    //     }
    //
    // The C# binding would look like this:
    //
    //     [BaseType (typeof (NSObject))]
    //     interface Widget {
    //     }
    //
    // To bind Objective-C properties, such as:
    //
    //     @property (nonatomic, readwrite, assign) CGPoint center;
    //
    // You would add a property definition in the C# interface like so:
    //
    //     [Export ("center")]
    //     CGPoint Center { get; set; }
    //
    // To bind an Objective-C method, such as:
    //
    //     -(void) doSomething:(NSObject *)object atIndex:(NSInteger)index;
    //
    // You would add a method definition to the C# interface like so:
    //
    //     [Export ("doSomething:atIndex:")]
    //     void DoSomething (NSObject object, int index);
    //
    // Objective-C "constructors" such as:
    //
    //     -(id)initWithElmo:(ElmoMuppet *)elmo;
    //
    // Can be bound as:
    //
    //     [Export ("initWithElmo:")]
    //     IntPtr Constructor (ElmoMuppet elmo);
    //
    // For more information, see http://developer.xamarin.com/guides/ios/advanced_topics/binding_objective-c/
    //
}
