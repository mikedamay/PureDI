﻿using System;
using System.Runtime.InteropServices.WindowsRuntime;
using PureDI;
using PureDI.Attributes;

namespace SimpleIOCCDemo
{
    [Bean(Name="usage")]
    internal class UsageListProvider : ListProvider
    {
        public TodoList LoadList()
        {
           string contents = "There is no To Do List to display - the usage is dotnet run <filePath>";
           return new TodoList(contents);
        }
    }
}
