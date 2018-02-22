
using System;
using System.Runtime.InteropServices;

public class VelodyneWrapper : IDisposable {
	const String DLL_LOCATION = "libvlp";

	[DllImport (DLL_LOCATION)]
	private static extern IntPtr CreateVLPObject(short ip1, short ip2, short ip3, short ip4, int port, int resolution,
    	int returnMode, int dataSource, int sensorFrequency, int velType);

	[DllImport (DLL_LOCATION)]
	private static extern void DeleteVLPObject(IntPtr pVlp);

	[DllImport (DLL_LOCATION)]
	private static extern void Run(IntPtr pVlp);

	[DllImport (DLL_LOCATION)]
	private static extern void SetAzimuth(IntPtr pVlp, double azimuth);

	[DllImport (DLL_LOCATION)]
	private static extern void SetTimeStamp(IntPtr pVlp, int timeStamp);

	[DllImport (DLL_LOCATION)]
	private static extern void SetChannel(IntPtr pVlp, double distance, short reflectivity);

	[DllImport (DLL_LOCATION)]
	private static extern void SendData(IntPtr pVlp);

	private IntPtr m_nativeObject;

	public VelodyneWrapper(short ip1, short ip2, short ip3, short ip4, int port, int resolution,
    	int returnMode, int dataSource, int sensorFrequency, int velType) {
			this.m_nativeObject = CreateVLPObject(ip1, ip2, ip3, ip4, port, resolution, returnMode, dataSource, sensorFrequency, velType);
	}

	~VelodyneWrapper() {Dispose(false);}
	
	public void Dispose() { Dispose(true);}

    protected virtual void Dispose(bool bDisposing) {
        if (this.m_nativeObject != IntPtr.Zero) {
            // DeleteVLPObject(this.m_nativeObject);
            // this.m_nativeObject = IntPtr.Zero;
        }

        if (bDisposing) {
            GC.SuppressFinalize(this);
        }
    }

	public void Run() {
		Run(this.m_nativeObject);
	}

	public void SetAzimuth(double azimuth) {
		SetAzimuth(this.m_nativeObject, azimuth);
	}

	public void SetTimeStamp(int timeStamp) {
		SetTimeStamp(this.m_nativeObject, timeStamp);
	}

	public void SetChannel(double distance, short reflectivity) {
		SetChannel(this.m_nativeObject, distance, reflectivity);
	}

	public void SendData() {
		SendData(this.m_nativeObject);
	}
}