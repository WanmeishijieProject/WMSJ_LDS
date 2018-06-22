using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CPAS.Models;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using CPAS.Interface;
using CPAS.Classes;
using CPAS.Config;
using CPAS.Vision;

namespace CPAS.ViewModels
{
    public enum USER
    {
        OPERATOR,
        ENGINNER,
        MANAGER
    }
   
    public class MainWindowViewModel : ViewModelBase
    {
    
        #region Fields
        private string _strViewID = "Home";
        private DateTime _myDateTime;
        private string _strUserName="Operator";
        private int _level=0;
        private string _strPLCErrorNumber="", _strSystemErrorNumber="";
        private bool _showPlcErrorListEdit;
        private EnumCamSnapState _amSnapState;
        private ObservableCollection<MessageItem> _plcMessageCollection=new ObservableCollection<MessageItem>();
        private ObservableCollection<MessageItem> _systemMessageCollection = new ObservableCollection<MessageItem>();
        private ObservableCollection<CameraItem> _cameraCollection = new ObservableCollection<CameraItem>();
        private ObservableCollection<RoiItem> _roiCollection = new ObservableCollection<RoiItem>();
        private ObservableCollection<TemplateItem> _templateCollection = new ObservableCollection<TemplateItem>();
        private Dictionary<string, string> LogInfoDic = new Dictionary<string, string>();
        private FileHelper ModelFileHelper = new FileHelper(FileHelper.GetCurFilePathString()+"VisionData\\Model");
        private FileHelper RoiFileHelper = new FileHelper(FileHelper.GetCurFilePathString() + "VisionData\\Roi");
        private Dictionary<int, string> ModelNameDic = new Dictionary<int, string>();
        private Dictionary<int, string> RoiNameDic = new Dictionary<int, string>();
        #endregion


        #region Property
        public DateTime MyDateTime
        {
            get { return _myDateTime; }
            set
            {
                if (_myDateTime != value)
                {
                    _myDateTime = value;
                    RaisePropertyChanged(() => MyDateTime);
                }
            }
        }
        public string StrCurViewID
        {
            set
            {
                if (_strViewID != null && _strViewID != value)
                {
                    _strViewID = value;
                    RaisePropertyChanged(() => StrCurViewID);
                }
            }
            get { return _strViewID; }
        }
        public string StrUserName
        {
            set
            {
                if (_strUserName!= value)
                {
                    _strUserName = value;
                    RaisePropertyChanged();
                }
            }
            get { return _strUserName; }
        }
        public string StrPLCErrorNumber
        {
            get
            {
                return _strPLCErrorNumber;
            }
            set
            {
                if (_strPLCErrorNumber != value)
                {
                    _strPLCErrorNumber = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string StrSystemErrorNumber
        {
            get
            {
                return _strSystemErrorNumber;
            }
            set
            {
                if (_strSystemErrorNumber != value)
                {
                    _strSystemErrorNumber = value;
                    RaisePropertyChanged();
                }
            }
        }
        public int Level
        {
            set
            {
                if (_level != value)
                {
                    _level = value;
                    RaisePropertyChanged();
                }
            }
            get { return _level; }
        }
        public bool ShowPlcErrorListEdit
        {
            set
            {
                if (_showPlcErrorListEdit != value)
                {
                    _showPlcErrorListEdit = value;
                    RaisePropertyChanged();
                }
            }
            get { return _showPlcErrorListEdit; }
        }
        public Enum_REGION_TYPE RegionType
        {
            get { return Vision.Vision.Instance.RegionType; }
            set { Vision.Vision.Instance.RegionType = value; }
        }
        public Enum_REGION_OPERATOR RegionOperator
        {
            get { return Vision.Vision.Instance.RegionOperator; }
            set { Vision.Vision.Instance.RegionOperator = value; }
        }
        public EnumCamSnapState CamSnapState
        {
            set
            {
                if (_amSnapState != value)
                {
                    _amSnapState = value;
                    RaisePropertyChanged();
                }
            }
            get { return _amSnapState;}
        }
        public ObservableCollection<MessageItem> PLCMessageCollection
        {
            get { return _plcMessageCollection; }
            set
            {
                if (_plcMessageCollection != value)
                {
                    _plcMessageCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<MessageItem> SystemMessageCollection
        {
            get { return _systemMessageCollection; }
            set
            {
                if (_systemMessageCollection != value)
                {
                    _systemMessageCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<CameraItem> CameraCollection
        {
            get { return _cameraCollection; }
            set
            {
                if (_cameraCollection != value)
                {
                    _cameraCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<RoiItem> RoiCollection
        {
            get { return _roiCollection; }
            set
            {
                if (_roiCollection != value)
                {
                    _roiCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<TemplateItem> TemplateCollection
        {
            get { return _templateCollection; }
            set
            {
                if (_templateCollection != value)
                {
                    _templateCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<string>[] StepCollection { get; set; }
        #endregion


        #region  Method
        private void ShowMessage(MessageItem msgItem)
        {
            if (PLCMessageCollection.Count > 100)
                PLCMessageCollection.RemoveAt(0);
            if (msgItem != null)
                PLCMessageCollection.Add(msgItem);
        }
        private void UpdateRoiCollect(int nCamID)
        {
            RoiCollection.Clear();
            foreach (var it in Vision.VisionDataHelper.GetTemplateListForSpecCamera(nCamID, RoiFileHelper.GetWorkDictoryProfileList()))
                RoiCollection.Add(new RoiItem() { StrName = it.Replace(string.Format("Cam{0}_", nCamID), ""), StrFullName = it });
        }
        private void UpdateModelCollect(int nCamID)
        {
            TemplateCollection.Clear();
            foreach(var it in Vision.VisionDataHelper.GetTemplateListForSpecCamera(nCamID, ModelFileHelper.GetWorkDictoryProfileList()))
                TemplateCollection.Add(new TemplateItem() { StrName = it.Replace(string.Format("Cam{0}_", nCamID), ""),StrFullName =it });
        }
        private void PLCMessageCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var collect = from msg in PLCMessageCollection where msg.MsgType == MSGTYPE.ERROR select msg;
            if (collect.Count() != 0)
                StrPLCErrorNumber = string.Format("{0}", collect.Count());
            else
                StrPLCErrorNumber = "";
        }
        private void SystemMessageCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var collect = from msg in SystemMessageCollection where msg.MsgType == MSGTYPE.ERROR select msg;
            if (collect.Count() != 0)
                StrSystemErrorNumber = string.Format("{0}", collect.Count());
            else
                StrSystemErrorNumber = "";
        }
        #endregion

        #region Commands
        public RelayCommand<string> RibonCommand { get { return new RelayCommand<string>(str => StrCurViewID = str); } }
        public RelayCommand<string> ClearMessageCommand
        {
            get
            {
                return new RelayCommand<string>(str => {
                    switch (str)
                    { 
                        case "ClearPLCMessage":
                            PLCMessageCollection.Clear();
                            break;
                        case "ClearSystemMessage":
                            SystemMessageCollection.Clear();
                            break;
                        default:
                            break;
                    }
                });
            } 
        
        }
        public RelayCommand StartCommand { get {
                return new RelayCommand(() => {
                    WorkFlow.WorkFlowMgr.Instance.StartAllStation();
                });
            } }
        public RelayCommand StopCommand
        {
            get
            {
                return new RelayCommand(() => {
                    WorkFlow.WorkFlowMgr.Instance.StopAllStation();
                });
            }
        }
        public RelayCommand<Tuple<string,string>> LogInCommand { get { return new RelayCommand<Tuple<string,string>>(t=> {
            Tuple<string, string> tuple = t as Tuple<string, string>;
            foreach (var it in ConfigMgr.UserCfgMgr.Users)
            {
                if (it.User == tuple.Item1 && it.Password == tuple.Item2)
                {
                    Level = it.Level;
                    StrUserName = it.User;
                }
            }
 
        }); } }
        public RelayCommand LogOutCommand { get { return new RelayCommand(() => { Level = 0; StrUserName = "Operator"; }); } }
        public RelayCommand<string> SetSnapStateCommand { get { return new RelayCommand<string>(
            str => {
                Messenger.Default.Send<string>(str, "SetCamState");
                if (str.ToLower() == "snapcontinues")
                    CamSnapState = EnumCamSnapState.BUSY;
                else
                    CamSnapState = EnumCamSnapState.IDLE;
            } ); } }
        public RelayCommand<int> UpdateRoiTemplate { get { return new RelayCommand<int>(nCamID=> {
            UpdateModelCollect(nCamID);
            UpdateRoiCollect(nCamID);
        }); } }
        public RelayCommand<string> ShowErrorListEditCommand
        { get { return new RelayCommand<string>(str=>{
            switch (str)
            {
                case "PLC":
                    ShowPlcErrorListEdit = true;
                    break;
                case "Sys":
                    ShowPlcErrorListEdit = false;
                    break;
                default:
                    break;
            }
        }); } }
        #endregion

        #region Ctor and DeCtor
        public MainWindowViewModel()
        {
            PLCMessageCollection.CollectionChanged += PLCMessageCollection_CollectionChanged;
            SystemMessageCollection.CollectionChanged += SystemMessageCollection_CollectionChanged;

            #region Messages
            Messenger.Default.Register<string>(this, "UpdateRoiFiles", str =>UpdateRoiCollect(Convert.ToInt16(str.Substring(3,1))));
            Messenger.Default.Register<string>(this, "UpdateTemplateFiles", str => UpdateModelCollect(Convert.ToInt16(str.Substring(3, 1))));
            Messenger.Default.Register<Tuple<string, string>>(this, "ShowStepInfo", tuple =>
                {
                    switch (tuple.Item1)
                    {
                        case "WorkRecord":
                            Application.Current.Dispatcher.Invoke(()=>StepCollection[0].Add(tuple.Item2));
                            break;
                        case "WorkTune1":
                            Application.Current.Dispatcher.Invoke(() => StepCollection[1].Add(tuple.Item2));
                            break;
                        case "WorkTune2":
                            Application.Current.Dispatcher.Invoke(() => StepCollection[2].Add(tuple.Item2));
                            break;
                        case "WorkCalib":
                            Application.Current.Dispatcher.Invoke(() => StepCollection[3].Add(tuple.Item2));
                            break;
                        default:
                            break;
                    }
                });

            Messenger.Default.Register<string>(this, "ShowError", str => {
                Application.Current.Dispatcher.Invoke(() => SystemMessageCollection.Add(new MessageItem() { MsgType = MSGTYPE.ERROR, StrMsg = str }));
            });
            Messenger.Default.Register<string>(this, "ShowInfo", str => {
                Application.Current.Dispatcher.Invoke(() => SystemMessageCollection.Add(new MessageItem() { MsgType = MSGTYPE.INFO, StrMsg = str }));
            });
            Messenger.Default.Register<string>(this, "ShowWarning", str => {
                Application.Current.Dispatcher.Invoke(() => SystemMessageCollection.Add(new MessageItem() { MsgType = MSGTYPE.WARNING, StrMsg = str }));
            });
            #endregion

            //StepInfo  init
            StepCollection = new ObservableCollection<string>[]{
                    new ObservableCollection<string>(),
                    new ObservableCollection<string>(),
                    new ObservableCollection<string>(),
                    new ObservableCollection<string>()
                };

            //Roi Model
            UpdateModelCollect(0);
            UpdateRoiCollect(0);

            //Camera
            CameraCollection.Add(new CameraItem() { CameraName = "CameraView_Cam1", StrCameraState="Connected"});
            CameraCollection.Add(new CameraItem() { CameraName = "CameraView_Cam2" , StrCameraState = "Disconnected" });
            CameraCollection.Add(new CameraItem() { CameraName = "CameraView_Cam3" , StrCameraState = "Connected" });


            //Load Config
            ConfigMgr.Instance.LoadConfig();
        }
        ~MainWindowViewModel()
        {
            Messenger.Default.Unregister<string>("ShowError");
            Messenger.Default.Unregister<string>("ShowInfo");
            Messenger.Default.Unregister<string>("ShowWarning");
        }
        #endregion

       
      
    }
}