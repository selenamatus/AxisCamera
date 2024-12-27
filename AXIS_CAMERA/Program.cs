using CommonLib;
using System.Configuration;
using System.Reflection;

namespace AXIS_CAMERA
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppMain oApp = new AppMain(args);

            if (oApp.IsRunnable)
                Application.Run(oApp);
        }

        private class AppMain : ApplicationContext
        {
            public bool IsRunnable { get; }
            private DynamTechMessaging.DynamTechInQ InQ;
            private DynamTechMessaging.DynamTechOutQ OutQ;
            private List<CameraConfig> cameraList;
            private CameraManager CameraManager;
            private MainForm AppForm;
            private bool ShowForm;

            public AppMain(string[] args)
            {
                try
                {
                    ErrorLog.sAppPath = Application.StartupPath;
                    ErrorLog.bTrace = DeclareModule.GetConfigString("Trace") == "Y";
                    ErrorLog.bAddStackTrace = DeclareModule.GetConfigString("AddStackTrace") == "Y";
                    ErrorLog.bWithMiliSeconds = DeclareModule.GetConfigString("WithMiliSeconds") == "Y";
                    ShowForm = DeclareModule.GetConfigString("ShowForm") == "Y";

                    cameraList = new List<CameraConfig>();

                    if (!ParseCmdLine(args))
                    {
                        ErrorLog.WriteErrLog(MethodBase.GetCurrentMethod(), "Parsing Failed");
                        return;
                    }

                    AppForm = new MainForm(cameraList.Count);
                    AppForm.FormClosing += FormClosingHandler;

                    try
                    {
                        CameraManager = new CameraManager(cameraList);
                        CameraManager.LprEventReceived += LprEventHandler;
                        CameraManager.CameraStatusReceived += HandleCameraStatus;

                    }
                    catch (Exception ex)
                    {
                        ErrorLog.WriteErrLog(ex);
                    }

                    if (ShowForm)
                        AppForm.Show();

                    IsRunnable = true;
                }
                catch (Exception ex)
                {
                    IsRunnable = false;
                    ErrorLog.WriteErrLog(ex);
                }
            }


            private void FormClosingHandler(object sender, FormClosingEventArgs e)
            {
                ErrorLog.WriteErrLog(MethodBase.GetCurrentMethod(), "Application was closed by user.");
                this.ExitThread();
            }

            private void HandleCameraStatus()
            {
                //if (ShowForm)
                    //AppForm.UpdateCameraStatus(args);
            }

            private void LprEventHandler()
            {
                //String Msg = lprEvent.QppMessage();
                //ErrorLog.WriteTraceFile(MethodBase.GetCurrentMethod(), "Send Msg to QPP [{0}]", Msg);

                //SendMsgToQPP(Msg);
                //if (ShowForm)
                //    AppForm.UpdateEventOnForm(lprEvent);
            }

            private bool ParseCmdLine(string[] args)
            {
                bool bRetVal = false;
                try
                {
                    if (args == null) return bRetVal;
                    if (args.Length < 1) return bRetVal;
                    if (string.IsNullOrEmpty(args[0])) return bRetVal;

                    string sCmdLine = args[0];

                    ErrorLog.WriteTraceFile(MethodBase.GetCurrentMethod()?.Name, $"Cmd Line is {sCmdLine}");

                    //"[iomex;picolo]{g1l1lpr1:1-1}{g1l2lpr1:2-2}{g1l3lpr1:3-3}{g1l4lpr1:4-4}"
                    string[] parts = sCmdLine.Split(new char[] { ']', '[' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts == null || parts.Length != 2)
                    {
                        ErrorLog.WriteErrLog(MethodBase.GetCurrentMethod(), "Command Line was not split correctly, searched for ][");
                        return bRetVal;
                    }

                    string qNamesStr = parts[0];
                    CreateMsgQs(qNamesStr);

                    parts = parts[1].Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string sCamera in parts)
                    {
                        string[] inner = sCamera.Split(new char[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);

                        int nCameraNum;
                        int nLane;
                        string sDeviceName = inner[0];

                        int.TryParse(inner[1], out nCameraNum);
                        int.TryParse(inner[2], out nLane);

                        //Array of Cameras
                        CameraConfig? cameraConfig = ConfigurationManager.GetSection("Camera" + nCameraNum) as CameraConfig;

                        if (cameraConfig != null)
                        {
                            cameraConfig.UniqueID = nCameraNum;
                            cameraConfig.DeviceName = sDeviceName;
                            cameraConfig.Lane = nLane;
                            cameraList.Add(cameraConfig);

                            ErrorLog.WriteTraceFile(MethodBase.GetCurrentMethod(), "Camera Added Name ->[" + sDeviceName + "] Lane -> [" + nLane + "]");
                        }
                    }
                    bRetVal = true;
                }
                catch (Exception ex)
                {
                    ErrorLog.WriteErrLog(ex);
                }

                return bRetVal;
            }

            private void CreateMsgQs(string sCmdPart)
            {

                string[] parts = sCmdPart.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts == null || parts.Length != 2)
                {
                    ErrorLog.WriteErrLog(MethodBase.GetCurrentMethod(), "Command Line was not split correctly, searched for ;");
                    return;
                }

                //In Q
                string InQName = parts[1];
                if (string.IsNullOrEmpty(InQName))
                    return;

                InQ = new DynamTechMessaging.DynamTechInQ(InQName, new Type[] { typeof(System.String) });

                InQ.MessageRecieved += MsgRecieved;
                InQ.StringMessageRecieved += MsgRecieved;

                //Out Q
                OutQ = new DynamTechMessaging.DynamTechOutQ(parts[0]);
            }

            private void MsgRecieved(string msg)
            {
                ErrorLog.WriteTraceFile(MethodBase.GetCurrentMethod(), "Msg is [{0}]", msg);
                string DeviceName;
                //[g2l1lpr][7][o][lpr][{goes_high}{{tcp}{g2l1lpr}{dolpr}}]
                string[] MessageParts = msg.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                DeviceName = MessageParts[0];

                MessageParts = MessageParts[4].Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

                //if (MessageParts[0] == "goes_high")
                //    SDKManager.DoLPR(DeviceName);
            }

            private void MsgRecieved(DynamTechMessaging.DynamTechMessage msg)
            {

            }

            private void SendMsgToQPP(string msg)
            {
                OutQ.SendMessage(msg);
            }
        }
    }
}