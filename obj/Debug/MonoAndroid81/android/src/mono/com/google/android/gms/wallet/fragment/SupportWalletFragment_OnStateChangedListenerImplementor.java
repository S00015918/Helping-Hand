package mono.com.google.android.gms.wallet.fragment;


public class SupportWalletFragment_OnStateChangedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.wallet.fragment.SupportWalletFragment.OnStateChangedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStateChanged:(Lcom/google/android/gms/wallet/fragment/SupportWalletFragment;IILandroid/os/Bundle;)V:GetOnStateChanged_Lcom_google_android_gms_wallet_fragment_SupportWalletFragment_IILandroid_os_Bundle_Handler:Android.Gms.Wallet.Fragment.SupportWalletFragment/IOnStateChangedListenerInvoker, Xamarin.GooglePlayServices.Wallet\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Wallet.Fragment.SupportWalletFragment+IOnStateChangedListenerImplementor, Xamarin.GooglePlayServices.Wallet", SupportWalletFragment_OnStateChangedListenerImplementor.class, __md_methods);
	}


	public SupportWalletFragment_OnStateChangedListenerImplementor ()
	{
		super ();
		if (getClass () == SupportWalletFragment_OnStateChangedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Wallet.Fragment.SupportWalletFragment+IOnStateChangedListenerImplementor, Xamarin.GooglePlayServices.Wallet", "", this, new java.lang.Object[] {  });
	}


	public void onStateChanged (com.google.android.gms.wallet.fragment.SupportWalletFragment p0, int p1, int p2, android.os.Bundle p3)
	{
		n_onStateChanged (p0, p1, p2, p3);
	}

	private native void n_onStateChanged (com.google.android.gms.wallet.fragment.SupportWalletFragment p0, int p1, int p2, android.os.Bundle p3);

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
