namespace AXIS_CAMERA;

public class LprEventData
{
    public string PlateNumber { get; set; }
    public DateTime EventDateTime { get; set; }
    public int Lane { get; set; }
    public int DeviceID { get; set; }
    public string DeviceName { get; set; }
    public Image LPRImage { get; set; }
    public string ImagePath { get; set; }

    public string EventDateTimeToString()
    {
        return EventDateTime.ToString("yyyyMMddHHmmss");
    }
    public string QppMessage()
    {
        // [	g1l1lpr		][1][i][lpr][{	9111637			}{	D:\Files\Images\XpDead.jpg	}{	20190423082000	}]
        //      Device name                 Plate number(LPN)   Pic Path                        Time stamp
        return "[" + DeviceName + "][1][i][lpr][{"
                   + PlateNumber + "}{"
                   + ImagePath + "}{"
                   + EventDateTime.ToString("yyyyMMddHHmmssfff") + "}]";
    }
}
