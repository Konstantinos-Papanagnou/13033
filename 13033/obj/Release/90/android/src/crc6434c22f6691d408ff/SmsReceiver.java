package crc6434c22f6691d408ff;


public class SmsReceiver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("_13033.SmsReceiver, 13033", SmsReceiver.class, __md_methods);
	}


	public SmsReceiver ()
	{
		super ();
		if (getClass () == SmsReceiver.class)
			mono.android.TypeManager.Activate ("_13033.SmsReceiver, 13033", "", this, new java.lang.Object[] {  });
	}

	public SmsReceiver (android.content.Context p0)
	{
		super ();
		if (getClass () == SmsReceiver.class)
			mono.android.TypeManager.Activate ("_13033.SmsReceiver, 13033", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
