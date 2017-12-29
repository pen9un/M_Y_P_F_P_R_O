﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewModels.Extraction
{
    /// <summary>
    /// 数据提取操作码。
    /// </summary>
    public enum ExtractionCode
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        Init = 0,
        /// <summary>
        /// 开始。
        /// </summary>
        Start = 1,
        /// <summary>
        /// 停止。
        /// </summary>
        Stop = 2,
        /// <summary>
        /// 进度改变。
        /// </summary>
        ProgressChanged = 3,
        /// <summary>
        /// 某一些项提取结束。
        /// </summary>
        ItemTerminate = 4,
        /// <summary>
        /// 任务状态改变。
        /// </summary>
        StateChanged = 5,
    }
}