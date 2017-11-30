﻿/* ==============================================================================
* Description：SettingViewModel  
* Author     ：litao
* Create Date：2017/11/22 10:32:57
* ==============================================================================*/

using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XLY.SF.Project.EarlyWarningView
{
    class SettingViewModel:INotifyPropertyChanged
    {
        public SettingViewModel()
        {
            SetCommand = new RelayCommand(()=> DetectionManager.Instance.ConfigDataManager.UpdateValidateData());
        }

        /// <summary>
        /// SettingManager
        /// </summary>
        public SettingManager SettingManager { get { return DetectionManager.Instance.ConfigDataManager.SettingManager; } }

        /// <summary>
        /// 当前选择的项目
        /// </summary>
        public SettingCollection CurrentSelected
        {
            get
            {
                return SettingManager.CurrentSelected;

            }
            set
            {
                SettingManager.CurrentSelected = value;
                OnPropertyChanged();
            }
        }       

        /// <summary>
        /// 设置生效命令
        /// </summary>
        public ICommand SetCommand { get; private set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}