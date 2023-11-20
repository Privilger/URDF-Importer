using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Robotics;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter.Control
{
    public enum ControlType { PositionControl };

    public class Controller : MonoBehaviour
    {
        private List<string> jointName = new List<string>
        {
            "link_FR1", "link_FR2", "link_FR3",
            "link_FL1", "link_FL2", "link_FL3",
            "link_HR1", "link_HR2", "link_HR3",
            "link_HL1", "link_HL2", "link_HL3",
        };
        private static float[] constJointVal_ =
        {
            - 0.04842926f, -  0.7984689f, 1.8f,
            - 0.03348614f, - 0.74565935f, 1.8f,
            - 0.04227986f, - 0.67181027f, 1.8f,
            - 0.08783916f, - 0.67963165f, 1.8f,
        };
        private float[] constJointVal = constJointVal_.Select(radian => (float)(radian * (180 / Math.PI))).ToArray();
        private JointControl[] jointControlList;
        private ArticulationBody[] articulationChain;
        private int previousIndex;

        [InspectorReadOnly(hideInEditMode: true)]
        public string selectedJoint;
        [HideInInspector]

        public ControlType control = ControlType.PositionControl;
        public float stiffness = 50f;
        public float damping = 0.5f;
        public float forceLimit = 20f; // Units: Nm or N

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
            }

            Invoke("OneTimeAction", 0.1f); // 0.1秒后执行
        }

        void OneTimeAction()
        {
            List<JointControl> jointList = new List<JointControl>();
            foreach (ArticulationBody joint in articulationChain)
            {
                var jointControl = joint.gameObject.GetComponent<JointControl>();
                jointList.Add(jointControl);
                UpdateControlType(jointControl);
            }
            jointControlList = jointList
                               .Where(joint => jointName.Contains(joint.name)) // 过滤掉不存在于 referenceList 中的元素
                               .OrderBy(joint => jointName.IndexOf(joint.name)) // 根据 referenceList 中的顺序进行排序
                               .ToArray();
        }

        void Update()
        {
            if (jointControlList != null && constJointVal.Length == jointControlList.Length)
            {
                for (int i = 0; i < constJointVal.Length; i++)
                {
                    jointControlList[i].target = constJointVal[i];
                }
            }
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
