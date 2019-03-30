package md536a853cc2e898c26443f7774e6ffb1be;


public class CustomGridViewAdapterViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HelpingHand.Adapter.CustomGridViewAdapterViewHolder, HelpingHand", CustomGridViewAdapterViewHolder.class, __md_methods);
	}


	public CustomGridViewAdapterViewHolder ()
	{
		super ();
		if (getClass () == CustomGridViewAdapterViewHolder.class)
			mono.android.TypeManager.Activate ("HelpingHand.Adapter.CustomGridViewAdapterViewHolder, HelpingHand", "", this, new java.lang.Object[] {  });
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
