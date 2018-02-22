[System.Serializable]
public class PID {
	public float pFactor, iFactor, dFactor, IntLimit=500;
		
	float integral;
	float lastError;
	
	
	public PID(float pFactor, float iFactor, float dFactor,float lim) {
		this.pFactor = pFactor;
		this.iFactor = iFactor;
		this.dFactor = dFactor;
		this.IntLimit = lim;
	}
	
	
	public float Update(float setpoint, float actual, float timeFrame) {
		float currentError = setpoint - actual;
		integral += currentError * timeFrame;
		integral = integral>IntLimit?IntLimit:integral;
		integral = integral<IntLimit?integral:IntLimit;
		float deriv = (currentError - lastError) / timeFrame;
		lastError = currentError;
		return currentError * pFactor + integral * iFactor + deriv * dFactor;
	}
}
