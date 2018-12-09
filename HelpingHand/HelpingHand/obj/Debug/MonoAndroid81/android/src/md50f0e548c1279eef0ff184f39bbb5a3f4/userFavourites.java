package md50f0e548c1279eef0ff184f39bbb5a3f4;


public class userFavourites
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HelpingHand.userFavourites, HelpingHand", userFavourites.class, __md_methods);
	}


	public userFavourites ()
	{
		super ();
		if (getClass () == userFavourites.class)
			mono.android.TypeManager.Activate ("HelpingHand.userFavourites, HelpingHand", "", this, new java.lang.Object[] {  });
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
