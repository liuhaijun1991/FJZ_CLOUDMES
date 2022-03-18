using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESStation.MESReturnView.Station
{
    public class CallStationReturn
    {
        public object Station;
        public object NextInput = null;
        //public string[] StationMessage = null;
        public string[] StationLabel = null;
        /// <summary>
        /// Pass,Fail
        /// </summary>
        public string ScanType;
        //public string[] Ctrl = null;
        //public string[] printLabels = null;
    }

    public class StationMessage
    {
        public string Message;
        public StationMessageState State = StationMessageState.Message;
        public StationMessageDisplayType DisplayType = StationMessageDisplayType.Default;

    }

    public enum StationMessageDisplayType
    {
        /// <summary>
        /// 消息將以默認的方式顯示
        /// </summary>
        Default = 0,
        /// <summary>
        /// Swal彈窗
        /// </summary>
        Swal = 1,
        /// <summary>
        /// Toastr彈窗
        /// </summary>
        Toastr = 2
    }

    public enum StationMessageState
    {
        Fail = 0,
        Pass = 1,
        Message = 2,
        Alert= 3,
        Debug = 4,
        CMCMessage=5,
        UserDefined = 6
    }
}
