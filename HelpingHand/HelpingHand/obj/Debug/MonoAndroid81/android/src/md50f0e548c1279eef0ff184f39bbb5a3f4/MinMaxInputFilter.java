package md50f0e548c1279eef0ff184f39bbb5a3f4;


public class MinMaxInputFilter
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.text.InputFilter
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_filter:(Ljava/lang/CharSequence;IILandroid/text/Spanned;II)Ljava/lang/CharSequence;:GetFilter_Ljava_lang_CharSequence_IILandroid_text_Spanned_IIHandler:Android.Text.IInputFilterInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("HelpingHand.MinMaxInputFilter, HelpingHand", MinMaxInputFilter.class, __md_methods);
	}


	public MinMaxInputFilter ()
	{
		super ();
		if (getClass () == MinMaxInputFilter.class)
			mono.android.TypeManager.Activate ("HelpingHand.MinMaxInputFilter, HelpingHand", "", this, new java.lang.Object[] {  });
	}

	public MinMaxInputFilter (int p0, int p1)
	{
		super ();
		if (getClass () == MinMaxInputFilter.class)
			mono.android.TypeManager.Activate ("HelpingHand.MinMaxInputFilter, HelpingHand", "System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public java.lang.CharSequence filter (java.lang.CharSequence p0, int p1, int p2, android.text.Spanned p3, int p4, int p5)
	{
		return n_filter (p0, p1, p2, p3, p4, p5);
	}

	private native java.lang.CharSequence n_filter (java.lang.CharSequence p0, int p1, int p2, android.text.Spanned p3, int p4, int p5);

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
