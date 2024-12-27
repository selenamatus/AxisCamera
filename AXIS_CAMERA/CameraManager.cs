using CommonLib;
using System.Reflection;

namespace AXIS_CAMERA;

public class CameraManager
{
    public List<AxisCamera> Cameras;
    readonly string ImagePath;
    readonly int GateNumber;
    static public bool Manual;

    public delegate void LprEventHandler();
    public delegate void CameraStatusEventHandler();
    public event LprEventHandler LprEventReceived;
    public event CameraStatusEventHandler CameraStatusReceived;

    public CameraManager(List<CameraConfig> cameras)
    {
        ImagePath = DeclareModule.ReadFromDynamTechRegistry("Common", "PicPath");
        Manual = DeclareModule.GetConfigString("Manual") == "Y";

        if (string.IsNullOrEmpty(ImagePath))
        {
            HandleError("PicPath is missing in registry, please create it.");
        }

        GateNumber = DeclareModule.GetConfigInt("Gate");

        Cameras = new List<AxisCamera>();
        foreach (var cam in cameras)
        {
            var newCam = new AxisCamera(cam);
            newCam.LprEventReceived += HandleLprEventReceived;
            newCam.SendCameraStatus += HandleCameraStatus;
            Cameras.Add(newCam);
        }
    }

    protected virtual void OnLprEventRecived()
    {
        LprEventReceived?.Invoke();
    }
    protected virtual void OnCameraStatusRecived()
    {
        CameraStatusReceived?.Invoke();
    }

    private void HandleCameraStatus()
    {
        //OnCameraStatusRecived(args);
    }

    private void HandleLprEventReceived()
    {
        //UpdateLprEvent(ref lprEvent);
        //WriteLprEventToLog(lprEvent);
        //SaveLprEventImage(ref lprEvent);

        //OnLprEventRecived(lprEvent);
    }

    //private void UpdateLprEvent(ref AW_LprEventData lprEvent)
    //{
    //    int CameraID = lprEvent.DeviceID;
    //    CameraConfig CameraInfo = Cameras.Where(cm => cm.CameraInfo.UniqueID == CameraID).FirstOrDefault().CameraInfo;
    //    lprEvent.Lane = CameraInfo.Lane;
    //    lprEvent.DeviceName = CameraInfo.DeviceName;
    //}
    //private void WriteLprEventToLog(AW_LprEventData lprEvent)
    //{
    //    string logLine = string.Format("Lane: {0}\tPlate Number: {1}\tEventTime: {2}",
    //        lprEvent.Lane, lprEvent.PlateNumber, lprEvent.EventDateTime);
    //    ErrorLog.WriteTrace("LPR Event", logLine, "EventsLog.log", DateTime.Now.ToString());
    //}
    //private void SaveLprEventImage(ref AW_LprEventData lprEvent)
    //{
    //    lprEvent.ImagePath = TodayPicPath(lprEvent.Lane) + ImageName(lprEvent);
    //    lprEvent.LPRImage = CompressImageAndSave(lprEvent.LPRImage, 20, lprEvent.ImagePath);
    //}

    private string TodayPicPath(int laneNumber)
    {
        string LPRTodayPath = ImagePath + "\\Lane_" + laneNumber + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
        if (!Directory.Exists(LPRTodayPath))
            Directory.CreateDirectory(LPRTodayPath);
        return LPRTodayPath;
    }
    //private string ImageName(AW_LprEventData lprEvent)
    //{
    //    return GateNumber.ToString("D2") + lprEvent.Lane + lprEvent.EventDateTimeToString() + lprEvent.PlateNumber + ".jpg";
    //}

    //private Image CompressImageAndSave(Image sourceImage, int imageQuality, string savePath)
    //{
    //    try
    //    {
    //        //Create an ImageCodecInfo-object for the codec information
    //        ImageCodecInfo jpegCodec = null;

    //        //Set quality factor for compression
    //        EncoderParameter imageQualitysParameter = new EncoderParameter(Encoder.Quality, imageQuality);

    //        //List all avaible codecs (system wide)
    //        ImageCodecInfo[] alleCodecs = ImageCodecInfo.GetImageEncoders();

    //        EncoderParameters codecParameter = new EncoderParameters(1);
    //        codecParameter.Param[0] = imageQualitysParameter;

    //        //Find and choose JPEG codec
    //        for (int i = 0; i < alleCodecs.Length; i++)
    //        {
    //            if (alleCodecs[i].MimeType == "image/jpeg")
    //            {
    //                jpegCodec = alleCodecs[i];
    //                break;
    //            }
    //        }

    //        //Save compressed and croped image
    //        Bitmap temp = new Bitmap(sourceImage);
    //        temp = CropImage(temp);
    //        temp.Save(savePath, jpegCodec, codecParameter);
    //        Image retImage = temp.Clone() as Image;
    //        sourceImage.Dispose();

    //        return retImage;
    //    }
    //    catch (Exception e)
    //    {
    //        throw e;
    //    }
    //}

    //public Bitmap CropImage(Bitmap source)
    //{
    //    Rectangle CropSection = new Rectangle(new Point(0, 65), new Size(source.Size.Width, source.Size.Height - 65));

    //    var bitmap = new Bitmap(CropSection.Width, CropSection.Height);
    //    using (var g = Graphics.FromImage(bitmap))
    //    {
    //        g.DrawImage(source, 0, 0, CropSection, GraphicsUnit.Pixel);
    //        return bitmap;
    //    }
    //}

    public static void HandleTrace(string trace)
    {
        ErrorLog.WriteErrLog(MethodBase.GetCurrentMethod(), trace);
    }

    public static void HandleError(string error)
    {
        ErrorLog.WriteErrLog(MethodBase.GetCurrentMethod(), error);
    }
}
