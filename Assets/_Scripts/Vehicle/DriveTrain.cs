using UnityEngine;
using System.Collections;

// This class simulates a car's engine and drivetrain, generating
// torque, and applying the torque to the wheels.
public class DriveTrain : MonoBehaviour
{

    // All the wheels the drivetrain should power
    public HingeJoint[] poweredWheels;

    // The gear ratios, including neutral (0) and reverse (negative) gears
    public float[] gearRatios;

    // The final drive ratio, which is multiplied to each gear ratio
    public float finalDriveRatio = 3.23f;

    // The engine's torque curve characteristics. Since actual curves are often hard to come by,
    // we approximate the torque curve from these values instead.

    // powerband RPM range
    public float minRPM = 800;
    public float maxRPM = 6400;

    // engine's maximal torque (in Nm) and RPM.
    public float maxTorque = 515;
    public float torqueRPM = 1700;

    // engine's maximal power (in Watts) and RPM.
    public float maxPower = 142000;
    public float powerRPM = 3400;
    public float EngineInertia = 0.3f;

    // engine inertia (how fast the engine spins up), in kg * m^2BetterWheel	public float EngineInertia = 0.3f;

    // engine's friction coefficients - these cause the engine to slow down, and cause engine braking.

    // constant friction coefficient
    public float engineBaseFriction = 25f;
    // linear friction coefficient (higher friction when engine spins higher)
    public float engineRPMFriction = 0.04f;
    public float WheelVelocityDamping = 0.01f;
    // Engine orientation (typically either Vector3.forward or Vector3.right). 
    // This determines how the car body moves as the engine revs up.	
    public Vector3 engineOrientation = Vector3.forward;

    // Coefficient determining how muchg torque is transfered between the wheels when they move at 
    // different speeds, to simulate differential locking.
    public float differentialLockCoefficient = 0;

    // inputs
    // engine throttle
    public float throttle = 0;
    // engine throttle without traction control (used for automatic gear shifting)
    public float throttleInput = 0;
    public float BreakTorque = 0,
                 BreakPedal = 0,
                 MaxBreakingTorque = 5000,
                 TireLockRotSpeed = 1;

    // shift gears automatically?
    public bool automatic = true;

    // state
    [Range(0, 4)]
    public int gear = 2;
    public float RPM;
    public float slipRatio = 0.0f;
    float engineAngularVelo;

    Rigidbody rb;
    Rigidbody[] Wheel;

    public float OutputTorque;
    float Sqr(float x) { return x * x; }
    float averageAngularVelo = 0;
    // Calculate engine torque for current rpm and throttle values.
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Wheel = new Rigidbody[poweredWheels.Length];
        for (int i = 0; i < poweredWheels.Length; i++)
        {
            Wheel[i] = poweredWheels[i].GetComponent<Rigidbody>();
        }
    }
    float CalcEngineTorque()
    {
        float result;
        if (RPM < torqueRPM)
            result = maxTorque * (-Sqr(RPM / torqueRPM - 1) + 1);
        else
        {
            float maxPowerTorque = maxPower / (powerRPM * 2 * Mathf.PI / 60);
            float aproxFactor = (maxTorque - maxPowerTorque) / (2 * torqueRPM * powerRPM - Sqr(powerRPM) - Sqr(torqueRPM));
            float torque = aproxFactor * Sqr(RPM - torqueRPM) + maxTorque;
            result = torque > 0 ? torque : 0;
        }
        if (RPM > maxRPM)
        {
            result *= 1 - ((RPM - maxRPM) * 0.006f);
            if (result < 0)
                result = 0;
        }
        if (RPM < 0)
            result = 0;
        return result;
    }
    float engineTorque, engineFrictionTorque;
    void FixedUpdate()
    {
        Debug.Log(gearRatios.Length);
        float ratio = gearRatios[gear] * finalDriveRatio;
        float inertia = EngineInertia * Sqr(ratio);
        engineFrictionTorque = engineBaseFriction + RPM * engineRPMFriction;
        engineTorque = (CalcEngineTorque() + Mathf.Abs(engineFrictionTorque)) * throttle;
        slipRatio = 0.0f;
        averageAngularVelo = 0;
        if (ratio == 0)
        {
            // Neutral gear - just rev up engine
            float engineAngularAcceleration = (engineTorque - engineFrictionTorque) / EngineInertia;
            engineAngularVelo += engineAngularAcceleration * Time.deltaTime;

            // Apply torque to car body
            rb.AddTorque(-engineOrientation * engineTorque);
        }
        float drivetrainFraction = 1.0f / poweredWheels.Length;

        foreach (HingeJoint w in poweredWheels)
            averageAngularVelo += w.velocity * Mathf.Deg2Rad * drivetrainFraction;

        BreakTorque = BreakPedal * MaxBreakingTorque; //breaks on
                                                      // Apply torque to wheels
        for (int i = 0; i < poweredWheels.Length; i++)
        {
            float lockingTorque = (averageAngularVelo - poweredWheels[i].velocity) * differentialLockCoefficient;
            // w.drivetrainInertia = inertia * drivetrainFraction;
            //
            float friction = engineFrictionTorque * Mathf.Abs(ratio) * drivetrainFraction * Mathf.Sign(averageAngularVelo);
            if (Mathf.Abs(poweredWheels[i].velocity * 0.1666f) < TireLockRotSpeed && BreakPedal > 0.7f) Wheel[i].angularDrag = BreakPedal * MaxBreakingTorque; //wheel lock
            else Wheel[i].angularDrag = WheelVelocityDamping;
            float breakTorque = BreakPedal * MaxBreakingTorque * Mathf.Sign(averageAngularVelo);
            OutputTorque = engineTorque * ratio * drivetrainFraction + lockingTorque - friction - breakTorque;
            Wheel[i].AddRelativeTorque(OutputTorque, 0, 0);
            // slipRatio += w.slipRatio * drivetrainFraction;



            // update engine angular velo
            engineAngularVelo = ratio!=0? averageAngularVelo * ratio:engineAngularVelo;
        }

        // update state
        slipRatio *= Mathf.Sign(ratio);
        RPM = engineAngularVelo * (60.0f / (2 * Mathf.PI));

        // very simple simulation of clutch - just pretend we are at a higher rpm.
        float minClutchRPM = minRPM;
        if (gear == 2)
            minClutchRPM += throttle * 3000;
        if (RPM < minClutchRPM)
            RPM = minClutchRPM;

        // Automatic gear shifting. Bases shift points on throttle input and rpm.
        if (automatic)
        {
            if (RPM >= maxRPM * (0.5f + 0.4f * throttleInput))
                ShiftUp();
            else if (RPM <= maxRPM * (0.2f + 0.2f * throttleInput) && gear > 2)
                ShiftDown();
            if (throttleInput < 0 && RPM <= minRPM)
                gear = (gear == 0 ? 2 : 0);
        }
    }

    public void ShiftUp()
    {
        if (gear < gearRatios.Length - 1)
            gear++;
    }

    public void ShiftDown()
    {
        if (gear > 0)
            gear--;
    }

    // Debug GUI. Disable when not needed.
    // void OnGUI()
    // {
    //     GUILayout.Label("RPM: " + RPM);
    //     GUILayout.Label("Gear: " + (gear - 1));
    //     automatic = GUILayout.Toggle(automatic, "Automatic Transmission");
    // }
}
