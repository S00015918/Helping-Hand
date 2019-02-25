package md50f0e548c1279eef0ff184f39bbb5a3f4;


public class AppointmentListAdapterViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HelpingHand.AppointmentListAdapterViewHolder, HelpingHand", AppointmentListAdapterViewHolder.class, __md_methods);
	}


	public AppointmentListAdapterViewHolder ()
	{
		super ();
		if (getClass () == AppointmentListAdapterViewHolder.class)
			mono.android.TypeManager.Activate ("HelpingHand.AppointmentListAdapterViewHolder, HelpingHand", "", this, new java.lang.Object[] {  });
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
