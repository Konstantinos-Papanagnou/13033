package crc64b79f47a6891a9ee4;


public class CodeMeta
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("_13033.Model.CodeMeta, 13033", CodeMeta.class, __md_methods);
	}


	public CodeMeta ()
	{
		super ();
		if (getClass () == CodeMeta.class)
			mono.android.TypeManager.Activate ("_13033.Model.CodeMeta, 13033", "", this, new java.lang.Object[] {  });
	}

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
