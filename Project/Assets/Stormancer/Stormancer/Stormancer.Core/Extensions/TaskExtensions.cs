﻿using System;
using System.Threading.Tasks;

namespace Stormancer
{
    public static class TaskExtensions
    {
        public static Task InvokeWrapping(this Func<Task> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public static Task<TResult> InvokeWrapping<TResult>(this Func<Task<TResult>> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }

        public static Task InvokeWrapping<TArg>(this Func<TArg, Task> func, TArg arg)
        {
            try
            {
                return func(arg);
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public static Task<TResult> InvokeWrapping<TArg, TResult>(this Func<TArg, Task<TResult>> func, TArg arg)
        {
            try
            {
                return func(arg);
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }
    }
}
