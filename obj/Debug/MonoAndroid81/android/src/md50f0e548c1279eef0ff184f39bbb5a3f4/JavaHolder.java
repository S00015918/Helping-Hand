package md50f0e548c1279eef0ff184f39bbb5a3f4;


public class JavaHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HelpingHand.JavaHolder, HelpingHand", JavaHolder.class, __md_methods);
	}


	public JavaHolder ()
	{
		super ();
		if (getClass () == JavaHolder.class)
			mono.android.TypeManager.Activate ("HelpingHand.JavaHolder, HelpingHand", "", this, new java.lang.Object[] {  });
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
