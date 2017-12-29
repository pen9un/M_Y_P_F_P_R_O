﻿/* ==============================================================================
* Description：BaseData  
* Author     ：litao
* Create Date：2017/11/27 15:19:30
* ==============================================================================*/

using ProjectExtend.Context;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using static XLY.SF.Project.ViewModels.Management.Settings.InspectionSettingsViewModel;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 配置文件过滤
    /// 原型图上有5种RootNode，此处把这5种直接预定义与此类中，放在RootNodeManager中，并且只读取这5中类型的数据到相应的节点下configFile.GetAllData(_rootNodeManager)。
    /// 根据配置文件更新有效数据的逻辑为把RootNodeManager中的数据放到名为ValidateDataNodes的列表中
    /// </summary>
    class ConfigDataFilter
    {
        private readonly RootNodeManager _rootNodeManager = new RootNodeManager();       

        /// <summary>
        /// 预警配置文件的所在顶层目录
        /// </summary>
        private string _baseDir;

        /// <summary>
        /// 是否已经初始化。对象要初始化后才能使用
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        ///通过导入获取setting对象。其用于读取数据库中的数据
        /// </summary>
        private IRecordContext<Inspection> _setting;

        /// <summary>
        /// 输出的有效节点列表
        /// </summary>
        public readonly List<DataNode> ValidateDataNodes = new List<DataNode>();

        /// <summary>
        /// 对象要初始化后才能使用
        /// </summary>
        /// <returns></returns>
        public bool Initialize(IRecordContext<Inspection> Setting)
        {
            _isInitialized = false;            

            _baseDir = Path.GetFullPath(@"EarlyWarningConfig\");
            //读取配置文件的数据到_rootNodeManager中
            ConfigFileDir configFileDir = new ConfigFileDir();
            bool ret = configFileDir.Initialize(_baseDir);
            if (!ret)
            {
                return _isInitialized;
            }
            configFileDir.GetAllData(_rootNodeManager);

            _setting = Setting;
            _isInitialized = true;
            return _isInitialized;
        }

        /// <summary>
        /// 根据参数配置，更新有效数据
        /// </summary>
        private void UpdateValidateDataBySetting()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            ValidateDataNodes.Clear();
            //没有设置对象时，有效数据就是全部数据
            if (_setting == null)
            {
                ValidateDataNodes.AddRange(_rootNodeManager.ValidateDataNodes);
                return;
            }

            //如果总开关没有开，则直接返回。ValidateDataNodes为空
            ISettings settings = (ISettings)_setting;           
            string str = settings.GetValue(SystemContext.EnableInspectionKey);
            if (!bool.TryParse(str, out bool b))
            {
                return;
            }
            if(b == false)
            {
                return;
            }

            //获取数据库中的配置，并按之过滤数据
            IEnumerable<InspectionModel> models = _setting.Records.ToArray().Select(x => new InspectionModel(x, _setting));
            foreach (var model in models)
            {
                if(!model.IsSelect)
                {
                    continue;
                }
                string path = model.Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string category = path;

                    if(!_rootNodeManager.Children.Keys.Contains(category))
                    {
                        continue;
                    }
                    RootNode rootNode = (RootNode)_rootNodeManager.Children[category];
                    rootNode.IsEnable = true;
                    foreach (CategoryNode item in rootNode.Children.Values)
                    {
                        ValidateDataNodes.AddRange(item.DataList);
                    }
                }
            }           
        }

        /// <summary>
        /// 根据参数配置，更新有效数据
        /// </summary>
        public void UpdateValidateData()
        {
            UpdateValidateDataBySetting();
        }

        /// <summary>
        /// 把有效的配置拷贝到指定的目录
        /// </summary>
        /// <param name="dir"></param>
        public void CopyValidateConfigTo(string dir)
        {
            if(!_isInitialized)
            {
                return;
            }
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            foreach (var item in _rootNodeManager.Children.Values)
            {
                RootNode rootNode = item as RootNode;
                if(rootNode != null)
                {
                    string filePath=rootNode.Path;
                    File.Copy(filePath, dir+"\\"+Path.GetFileName(filePath),true);
                }
            }
        }
    }
}