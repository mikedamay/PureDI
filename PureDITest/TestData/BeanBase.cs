﻿using PureDI;

namespace PureDITest.TestData
{

    public class IBean
    {

    }

    [Bean]
    public class BeanBase : IBean
    {

    }

    [Bean]
    public class ABean : BeanBase
    {

    }
}