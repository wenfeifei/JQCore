﻿using JQCore.DataAccess.DbClient;
using JQCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JQCore.DataAccess.Utils
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：SqlWhereBuilder.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：SQL拼接条件
    /// 创建标识：yjq 2017/9/5 20:10:42
    /// </summary>
    public class SqlWhereBuilder
    {
        private DatabaseType _dbType;
        private List<WhereInfo> _whereList;
        private List<DbParameterInfo> _parameterList;

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbType"></param>
        public SqlWhereBuilder(DatabaseType dbType)
        {
            _dbType = dbType;
            _whereList = new List<WhereInfo>();
            _parameterList = new List<DbParameterInfo>();
        }

        /// <summary>
        ///
        /// </summary>
        public SqlWhereBuilder() : this(DatabaseType.MSSQLServer)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="where">条件</param>
        public SqlWhereBuilder(string where) : this(DatabaseType.MSSQLServer)
        {
            Append(where);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="dbType">数据库类型</param>
        public SqlWhereBuilder(string where, DatabaseType dbType) : this(dbType)
        {
            Append(where);
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return !_whereList.Any();
            }
        }

        /// <summary>
        /// 更改数据库类型
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public SqlWhereBuilder ChangeDbType(DatabaseType dbType)
        {
            _dbType = dbType;
            return this;
        }

        /// <summary>
        /// 拼接where条件
        /// </summary>
        /// <param name="where">要拼接的内容</param>
        /// <returns></returns>
        public SqlWhereBuilder Append(string where)
        {
            AddWhereInfoIf(where.IsNotNullAndNotEmptyWhiteSpace(), where, isFormat: false);
            return this;
        }

        /// <summary>
        /// 拼接where条件
        /// </summary>
        /// <param name="format">符合格式字符串</param>
        /// <param name="args">一个对象数组</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendFormat(string format, params object[] args)
        {
            AddWhereInfoIf(format.IsNotNullAndNotEmptyWhiteSpace(), string.Format(format, args), isFormat: false);
            return this;
        }

        /// <summary>
        /// 拼接相等条件语句
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendEqual(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                AddWhereInfo(string.Format(" {0}={1}{2} ", key, "{0}", Clean(paramKey ?? key)), isFormat: true);
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 拼接包含条件语句
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendLike(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                if (_dbType == DatabaseType.MySql)
                {
                    AddWhereInfo(string.Format(" {0} LIKE CONCAT('%',{1}{2},'%') ", key, "{0}", Clean(paramKey ?? key)), isFormat: true);
                }
                else
                {
                    AddWhereInfo(string.Format(" {0} LIKE '%'+{1}{2}+'%' ", key, "{0}", Clean(paramKey ?? key)), isFormat: true);
                }
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 拼接以什么开始条件语句
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendStartWith(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                if (_dbType == DatabaseType.MySql)
                {
                    AddWhereInfo(string.Format(" {0} LIKE CONCAT({1}{2},'%') ", key, "{0}", Clean(paramKey ?? key)));
                }
                else
                {
                    AddWhereInfo(string.Format(" {0} LIKE {1}{2}+'%' ", key, "{0}", Clean(paramKey ?? key)));
                }
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 拼接以什么结束条件语句
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppenEndWith(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                if (_dbType == DatabaseType.MySql)
                {
                    AddWhereInfo(string.Format(" {0} LIKE CONCAT('%',{1}{2}) ", key, "{0}", Clean(paramKey ?? key)));
                }
                else
                {
                    AddWhereInfo(string.Format(" {0} LIKE '%'+{1}{2} ", key, "{0}", Clean(paramKey ?? key)));
                }
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 拼接位于两者之间的条件 只传递最大值时 按小于等于计算，只传递最小值时按大于等于计算
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="startParamKey">起始值参数</param>
        /// <param name="endParamKey">最大值参数</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendBetween(string key, object minValue, object maxValue, string startParamKey, string endParamKey)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (minValue.IsNotNullAndNotEmptyWhiteSpace() && maxValue.IsNotNullAndNotEmptyWhiteSpace())
            {
                AddWhereInfo(string.Format(" {0} BETWEEN {1}{2} AND {1}{3} ", key, "{0}", Clean(startParamKey), Clean(endParamKey)));
                _parameterList.Add(new DbParameterInfo(Clean(startParamKey), minValue, null, null, null, scale: null));
                _parameterList.Add(new DbParameterInfo(Clean(endParamKey), maxValue, null, null, null, scale: null));
            }
            else if (minValue.IsNotNullAndNotEmptyWhiteSpace())
            {
                AppendMoreThanOrEqual(key, minValue, paramKey: startParamKey);
            }
            else if (maxValue.IsNotNullAndNotEmptyWhiteSpace())
            {
                AppendLessThanOrEqual(key, maxValue, paramKey: endParamKey);
            }
            return this;
        }

        /// <summary>
        /// 拼接小于的条件
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendLessThan(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                AddWhereInfo(string.Format(" {0}<{1}{2} ", key, "{0}", Clean(paramKey ?? key)));
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 拼接大于的条件
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendMoreThan(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                AddWhereInfo(string.Format(" {0}>{1}{2} ", key, "{0}", Clean(paramKey ?? key)));
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 拼接小于等于的条件
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendLessThanOrEqual(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                AddWhereInfo(string.Format(" {0}<={1}{2} ", key, "{0}", Clean(paramKey ?? key)));
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 拼接大于等于的条件
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="obj">值</param>
        /// <param name="paramKey">参数名字(不传则默认拼接字段名)</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendMoreThanOrEqual(string key, object obj, string paramKey = null)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (obj.IsNotNullAndNotEmptyWhiteSpace())
            {
                AddWhereInfo(string.Format(" {0}>={1}{2} ", key, "{0}", Clean(paramKey ?? key)));
                _parameterList.Add(new DbParameterInfo(Clean(paramKey ?? key), obj, null, null, null, scale: null));
            }
            return this;
        }

        /// <summary>
        /// 添加假如为空时条件（不建议使用，建议更改数据库设计）
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="replaceValue">为空时要替换的值</param>
        /// <param name="equalValue">比较的值</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendIfNull(string key, string replaceValue, string equalValue)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            if (replaceValue.IsNotNullAndNotEmptyWhiteSpace() && equalValue.IsNotNullAndNotEmptyWhiteSpace())
            {
                string trueReplaceValue = string.Empty;
                bool isFormat = false;
                if (IsStartWithSign(replaceValue))
                {
                    trueReplaceValue = string.Format("{0}{1}", "{0}", Clean(replaceValue));
                    isFormat = true;
                }
                else
                {
                    trueReplaceValue = replaceValue;
                }
                string trueEqualValue = string.Empty;
                if (IsStartWithSign(equalValue))
                {
                    trueEqualValue = string.Format("{0}{1}", "{0}", Clean(equalValue));
                    isFormat = true;
                }
                else
                {
                    trueEqualValue = equalValue;
                }
                AddWhereInfo(string.Format(" {0}={1}", SqlQueryUtil.GetIfNull(_dbType, key, trueReplaceValue), trueEqualValue), isFormat: isFormat);
            }
            return this;
        }

        /// <summary>
        /// 添加假如为空时条件（不建议使用，建议更改数据库设计）
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="replaceValue">为空时要替换的值</param>
        /// <param name="equalValue">比较的值</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendIfNull(string key, int replaceValue, int equalValue)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            else
            {
                AddWhereInfo(string.Format(" {0}={1}", SqlQueryUtil.GetIfNull(_dbType, key, replaceValue.ToString()), equalValue.ToString()), isFormat: false);
            }
            return this;
        }

        /// <summary>
        /// 添加假如为空时条件（不建议使用，建议更改数据库设计）
        /// </summary>
        /// <param name="key">要拼接的查询字段</param>
        /// <param name="replaceValue">为空时要替换的值</param>
        /// <param name="equalValue">比较的值</param>
        /// <returns></returns>
        public SqlWhereBuilder AppendIfNull(string key, int replaceValue, string equalValue)
        {
            if (key.IsNullOrEmptyWhiteSpace())
            {
                return this;
            }
            else
            {
                bool isFormat = false;
                string trueEqualValue = string.Empty;
                if (IsStartWithSign(equalValue))
                {
                    trueEqualValue = string.Format("{0}{1}", "{0}", Clean(equalValue));
                    isFormat = true;
                }
                else
                {
                    trueEqualValue = equalValue;
                }
                AddWhereInfo(string.Format(" {0}={1}", SqlQueryUtil.GetIfNull(_dbType, key, replaceValue.ToString()), trueEqualValue), isFormat: isFormat);
            }
            return this;
        }

        /// <summary>
        /// 清除原来的条件
        /// </summary>
        public void Clear()
        {
            _whereList.Clear();
        }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<DbParameterInfo> ParameterList
        {
            get { return _parameterList; }
        }

        /// <summary>
        /// 将之前所有的条件按照and方式拼接
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_whereList.Count > 2)
            {
                StringBuilder stringBuilder = new StringBuilder();
                var index = 0;
                foreach (var item in _whereList)
                {
                    if (index == 0)
                    {
                        stringBuilder.Append(item.ToString(GetParamSign()));
                    }
                    else
                    {
                        stringBuilder.AppendFormat(" AND {0}", item.ToString(GetParamSign()));
                    }

                    index++;
                }
                return stringBuilder.ToString();
            }
            return string.Join(" AND ", _whereList.Select(m => m.ToString(GetParamSign())));
        }

        /// <summary>
        /// 判断参数是否为sql注入开头
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool IsStartWithSign(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                switch (name[0])
                {
                    case '@':
                    case ':':
                    case '?':
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取参数符号
        /// </summary>
        /// <returns></returns>
        private string GetParamSign()
        {
            return SqlQueryUtil.GetSign(_dbType);
        }

        /// <summary>
        /// 清除符号
        /// </summary>
        /// <param name="name">要清除的字符</param>
        /// <returns>清除后的字符</returns>
        private static string Clean(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                switch (name[0])
                {
                    case '@':
                    case ':':
                    case '?':
                        return name.Substring(1);
                }
            }
            return name;
        }

        #region 添加条件信息

        /// <summary>
        /// 添加条件信息
        /// </summary>
        /// <param name="whereContent">条件内容</param>
        /// <param name="isFormat">是否为复合格式字符串</param>
        /// <returns></returns>
        private SqlWhereBuilder AddWhereInfo(string whereContent, bool isFormat = true)
        {
            _whereList.Add(new WhereInfo(whereContent, isFormat));
            return this;
        }

        /// <summary>
        /// 添加条件信息
        /// </summary>
        /// <param name="condition">判断条件</param>
        /// <param name="whereContent">条件内容</param>
        /// <param name="isFormat">是否为复合格式字符串</param>
        /// <returns></returns>
        private SqlWhereBuilder AddWhereInfoIf(bool condition, string whereContent, bool isFormat = true)
        {
            if (condition)
                _whereList.Add(new WhereInfo(whereContent, isFormat));
            return this;
        }

        /// <summary>
        /// 添加条件信息
        /// </summary>
        /// <param name="condition">判断条件</param>
        /// <param name="whereContent">条件内容</param>
        /// <param name="isFormat">是否为复合格式字符串</param>
        /// <returns></returns>
        private SqlWhereBuilder AddWhereInfoIf(Func<bool> condition, string whereContent, bool isFormat = true)
        {
            if (condition != null && condition())
                _whereList.Add(new WhereInfo(whereContent, isFormat));
            return this;
        }

        #endregion 添加条件信息
    }

    /// <summary>
    /// 条件信息
    /// </summary>
    public struct WhereInfo
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="whereContent">内容</param>
        /// <param name="isFormat"></param>
        public WhereInfo(string whereContent, bool isFormat = true)
        {
            WhereContent = whereContent;
            IsFormat = isFormat;
        }

        /// <summary>
        /// 条件内容
        /// </summary>
        public string WhereContent { get; set; }

        /// <summary>
        /// 是否为字符串文本
        /// </summary>
        public bool IsFormat { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        public string ToString(string sign)
        {
            if (IsFormat)
            {
                return string.Format(WhereContent, sign);
            }
            return WhereContent;
        }
    }
}