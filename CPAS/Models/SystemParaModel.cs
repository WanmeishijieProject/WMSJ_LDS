﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAS.Models
{
    public enum EnumBadBarcodeExpiration
    {
        OneWeek,
        HalfMonth,
        OneMonth
    }
    public class SystemParaModel : INotifyPropertyChanged
    {
        private string _curSelectedPrescription = "";
        private EnumBadBarcodeExpiration _badBarcodeExpiration;
        private EnumBadBarcodeExpiration _imageSaveExpiration;

        [ReadOnly(true)]
        public string CurPrescriptionName
        {
            get { return _curSelectedPrescription; }
            set
            {
                _curSelectedPrescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurPrescriptionName"));
            }
        }
        public EnumBadBarcodeExpiration BadBarcodeExpiration
        {
            get { return _badBarcodeExpiration; }
            set
            {
                if (_badBarcodeExpiration != value)
                {
                    _badBarcodeExpiration = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BadBarcodeExpiration"));
                }
            }
        }
        public EnumBadBarcodeExpiration ImageSaveExpiration
        {
            get { return _imageSaveExpiration; }
            set
            {
                if (_imageSaveExpiration != value)
                {
                    _imageSaveExpiration = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSaveExpiration"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
    }
}
