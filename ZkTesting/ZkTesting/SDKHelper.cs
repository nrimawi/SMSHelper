using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZkTesting
{
    public class SDKHelper
    {
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private static bool bIsConnected = false;//the boolean value identifies whether the device is connected

        public void sta_ConnectTCP()
        {

            axCZKEM1.SetCommPassword(Convert.ToInt32(0));

            SetConnectState(true);

            if (axCZKEM1.Connect_Net("192.168.1.201", Convert.ToInt32("4370")) == true)
            {
                MessageBox.Show("Connected");
                regEvents();
                MessageBox.Show("Events Successfully ");

            }
            else
            {
                MessageBox.Show("Not Connected");

            }
        }
        public void setGroupTZ(int groupId, int T0, int T1, int T2)
        {
            int[] tz= new int[3];
            tz[0] = T0;
            tz[1] = T1;
            tz[2] = T2;
            var x= axCZKEM1.SetGroupTZs(1,groupId, tz[0]);
            string res = "";
            int xx = -1;
            axCZKEM1.GetGroupTZs(1, groupId, ref xx);
            MessageBox.Show($"Access Group{groupId}= {xx}");
        }
        public void setUserAccessGroup(int userId,int groupId)
        {
            int res = -1;

            axCZKEM1.SetUserGroup(1, userId, groupId);
            axCZKEM1.GetUserGroup(1, userId, ref res);
            MessageBox.Show($"access group for {userId}= {res}");

        }

        public void getUserAccessGroup(int userId)
        {
            int res = -1;
            axCZKEM1.GetUserGroup(1, userId,ref res);
            MessageBox.Show($"access group for {userId}= {res}");
        }
        public int sta_OnlineEnroll()
        {

            int idwErrorCode = 0;
            axCZKEM1.CancelOperation();
            if (axCZKEM1.StartEnroll(5, 5))
            {

            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
            }

            return 1;
        }
        public bool GetConnectState()
        {
            return bIsConnected;
        }
        public void SetConnectState(bool state)
        {
            bIsConnected = state;
            //connected = state;
        }

        public void regEvents()
        {
            if (axCZKEM1.RegEvent(1, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            {
                //common interface
                this.axCZKEM1.OnFinger += new zkemkeeper._IZKEMEvents_OnFingerEventHandler(OnFingerHandler);
                // this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                //  this.axCZKEM1.OnFingerFeature += new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnFingerFeature);
                //  this.axCZKEM1.OnDeleteTemplate += new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnDeleteTemplate);
                this.axCZKEM1.OnNewUser += new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
                // this.axCZKEM1.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
                this.axCZKEM1.OnAlarm += new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
                this.axCZKEM1.OnDoor += new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnDoor);

                //only for color device
                this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
                this.axCZKEM1.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);

                //only for black&white device
                this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction);

                this.axCZKEM1.OnEnrollFinger += new zkemkeeper._IZKEMEvents_OnEnrollFingerEventHandler(axCZKEM1_OnEnrollFinger);
            }
        }

        private void axCZKEM1_OnEnrollFinger(int EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
        {
            MessageBox.Show("axCZKEM1_OnEnrollFinger");
            if (axCZKEM1.StartIdentify())
            {
            }
                ;//After enrolling templates,you should let the device into the 1:N verification condition
        }

        private void axCZKEM1_OnAttTransaction(int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second)
        {
            MessageBox.Show("axCZKEM1_OnAttTransaction");
        }

        private void axCZKEM1_OnEnrollFingerEx(string EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
        {
            MessageBox.Show("axCZKEM1_OnEnrollFingerEx");
        }

        private void axCZKEM1_OnDoor(int EventType)
        {
            MessageBox.Show("axCZKEM1_OnDoor");
        }

        private void axCZKEM1_OnAlarm(int AlarmType, int EnrollNumber, int Verified)
        {
            MessageBox.Show("axCZKEM1_OnAlarm");

        }

        private void axCZKEM1_OnNewUser(int EnrollNumber)
        {
            MessageBox.Show("axCZKEM1_OnNewUser");
        }

        private void axCZKEM1_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {
            MessageBox.Show($"axCZKEM1_OnAttTransactionEx user id {EnrollNumber}");
        }



        void OnFingerHandler()
        {
            MessageBox.Show("OnFingerHandler");
        }
    }
}
