using System.Configuration;

namespace AXIS_CAMERA;

public class CameraConfig : ConfigurationSection
{
    public int UniqueID { get; set; }
    public string DeviceName { get; set; }
    public int Lane { get; set; }
    public bool LoggedIn { get; set; }

    [ConfigurationProperty("IP_Address")]
    public string IP_Address
    {
        get
        {
            return Convert.ToString(base["IP_Address"]);
        }
    }

    [ConfigurationProperty("Port")]
    public ushort Port
    {
        get
        {
            return Convert.ToUInt16(base["Port"]);
        }
    }

    [ConfigurationProperty("UserName")]
    public string UserName
    {
        get
        {
            return Convert.ToString(base["UserName"]);
        }
    }

    [ConfigurationProperty("Password")]
    public string Password
    {
        get
        {
            return Convert.ToString(base["Password"]);
        }
    }
}
