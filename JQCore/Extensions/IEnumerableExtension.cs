﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JQCore.Extensions
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：IEnumerableExtension.cs
    /// 类属性：公共类（静态）
    /// 类功能描述：IEnumerable扩展类
    /// 创建标识：yjq 2017/9/4 17:29:12
    /// </summary>
    public static partial class IEnumerableExtension
    {
        /// <summary>
        /// 遍历执行方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action">要执行的方法</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }

        /// <summary>
        /// 遍历异步执行方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action">要执行的方法</param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            return Task.WhenAll(from item in enumerable select Task.Run(() => action(item)));
        }

        /// <summary>
        /// 不为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) return false;
            if (enumerable.Any())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !(enumerable.IsNotEmpty());
        }
    }
}