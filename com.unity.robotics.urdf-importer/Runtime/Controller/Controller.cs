using System;
using Unity.Robotics;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter.Control
{
    public enum ControlType { PositionControl };

    public class Controller : MonoBehaviour
    {
        private ArticulationBody[] articulationChain;
        private int previousIndex;

        [InspectorReadOnly(hideInEditMode: true)]
        public string selectedJoint;
        [HideInInspector]

        public ControlType control = ControlType.PositionControl;
        public float stiffness = 100f;
        public float damping = 1f;
        public float forceLimit = 10f; // Units: Nm or N

        void Start()
        {
            this.gameObject.AddComponent<FKRobot>();
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            foreach (ArticulationBody joint in articulationChain)
            {
                joint.gameObject.AddComponent<JointControl>();
                joint.jointFriction = 0;  // 摩擦力
                joint.angularDamping = 0;  // 阻尼
                ArticulationDrive currentDrive = joint.xDrive;
                currentDrive.forceLimit = forceLimit;
                joint.xDrive = currentDrive;
                // JointControl jointControl = joint.gameObject.GetComponent<JointControl>();
                // jointControl.Init();
                // UpdateControlType(jointControl);
            }

            Invoke("OneTimeAction", 0.1f); // 0.1秒后执行
        }

        void OneTimeAction()
        {
            foreach (ArticulationBody joint in articulationChain)
            {
                UpdateControlType(joint.gameObject.GetComponent<JointControl>());
            }
        }

        void Update()
        {

        }

        public void UpdateControlType(JointControl joint)
        {
            joint.controltype = control;
            if (control == ControlType.PositionControl)
            {
                ArticulationDrive drive = joint.joint.xDrive;
                drive.stiffness = stiffness;
                drive.damping = damping;
                joint.joint.xDrive = drive;
            }
        }
    }
}
