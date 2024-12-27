using CommonLib;
using System.Configuration;
using System.Timers;

namespace AXIS_CAMERA;

public class AxisCamera
{
    public readonly CameraConfig CameraInfo;

    private bool Connected;
    private System.Timers.Timer ConnectionTimer;

    public delegate void LprAlarmEventHandler();
    public delegate void CameraStatusEventHandler(object sender, CameraStatusEventArgs args);
    public event LprAlarmEventHandler LprEventReceived;
    public event CameraStatusEventHandler SendCameraStatus;

    public AxisCamera(CameraConfig config)
    {
        CameraInfo = config;

        InitTimer();
    }

    private void TakeSnapshot()
    {

    }

    private void OnTimerTick(object sender, ElapsedEventArgs e)
    {
        if (Connected && PingCamera())
            OnCameraStatusSend();
        else
        {
            if (!PingCamera())
            {
                ErrorLog.WriteTraceFile("Communication", String.Format("Camera {0} is not connected.", CameraInfo.Lane));
                Connected = false;
                //UnRegisterAlarm();
            }
            else
            {
                ErrorLog.WriteTraceFile("Communication", String.Format("Trying to reconnect Camera {0} after power lose.", CameraInfo.Lane));
                //Login();
                if (Connected)
                {
                    ////RegisterAlarm();
                }
            }
            OnCameraStatusSend();
        }
    }

    private bool PingCamera()
    {
        Ping ping = new Ping();
        return ping.IPAvailable(CameraInfo.IP_Address).IsAvailable;
    }
    protected virtual void OnCameraStatusSend()
    {
        SendCameraStatus?.Invoke(this,
            new CameraStatusEventArgs { LaneId = CameraInfo.UniqueID, Status = Connected });
    }

    public class CameraStatusEventArgs : EventArgs
    {
        public int LaneId { get; set; }
        public bool Status { get; set; }
    }


    private void InitTimer()
    {
        ConnectionTimer = new System.Timers.Timer();
        ConnectionTimer.Elapsed += OnTimerTick;
        ConnectionTimer.Interval = DeclareModule.GetConfigInt("ConnectionTestInterval");
        ConnectionTimer.Start();
    }


}
