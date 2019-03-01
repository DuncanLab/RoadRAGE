// This class is used to stablize the car's AI steering system,
// without it there will be significant speed wobble when attempting
// to follow a path (especially one with many nodes). Read more
// about PID here: https://en.m.wikipedia.org/wiki/PID_controller
[System.Serializable]
public class PID {
	public float pFactor, iFactor, dFactor;
		
	float integral;
	float lastError;
	
	
	public PID(float pFactor, float iFactor, float dFactor) {
		this.pFactor = pFactor;
		this.iFactor = iFactor;
		this.dFactor = dFactor;
	}
	
	
	public float Update(float setpoint, float actual, float timeFrame) {
		float present = setpoint - actual;
		integral += present * timeFrame;
		float deriv = (present - lastError) / timeFrame;
		lastError = present;
		return present * pFactor + integral * iFactor + deriv * dFactor;
	}
}
