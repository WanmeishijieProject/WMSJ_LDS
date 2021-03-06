﻿using CPAS.Classes;
using CPAS.UserCtrl;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAS.Models
{
    public class TemplateItem : RoiModelBase
    {

        public override RelayCommand<RoiModelBase> OperateDelete
        {
            get
            {
                return new RelayCommand<RoiModelBase>(item => {
                    var model= item as TemplateItem;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(FileHelper.GetCurFilePathString());
                    sb.Append("VisionData\\Model\\");
                    sb.Append(item.StrFullName);
                    int nCamID = Convert.ToInt16(item.StrFullName.Substring(3, 1));
                    if (UC_MessageBox.ShowMsgBox(string.Format("确定要删除{0}吗?", item.StrName)) == System.Windows.MessageBoxResult.Yes)
                    {

                        //三个文件同时删除
                        FileHelper.DeleteFile(sb.ToString()+".shm");
                        FileHelper.DeleteFile(sb.ToString() + ".reg");
                        FileHelper.DeleteFile(sb.ToString() + ".tup");
                        Messenger.Default.Send<int>(nCamID, "UpdateTemplateFiles");
                    }   
                });
            }
        }
    }
}
