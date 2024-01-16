using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.UrdfImporter;

public class JointControl : MonoBehaviour
{
    public Unity.Robotics.UrdfImporter.Control.ControlType controltype;
    public float target ;
    public ArticulationBody joint;

    void Start()
    {
        joint = this.GetComponent<ArticulationBody>();
    }

    void FixedUpdate(){
        if (joint.jointType != ArticulationJointType.FixedJoint)
        {
            if (controltype == Unity.Robotics.UrdfImporter.Control.ControlType.PositionControl)
            {
                ArticulationDrive currentDrive = joint.xDrive;

                if (joint.jointType == ArticulationJointType.RevoluteJoint)
                {
                    if (joint.twistLock == ArticulationDofLock.LimitedMotion)
                    {
                        if (target > currentDrive.upperLimit)
                        {
                            currentDrive.target = currentDrive.upperLimit;
                        }
                        else if (target < currentDrive.lowerLimit)
                        {
                            currentDrive.target = currentDrive.lowerLimit;
                        }
                        else
                        {
                            currentDrive.target = target;
                        }
                    }
                    else
                    {
                        currentDrive.target += target;
                    }
                }
                currentDrive.targetVelocity = 0f;
                joint.xDrive = currentDrive;
            }
        }
    }
}
