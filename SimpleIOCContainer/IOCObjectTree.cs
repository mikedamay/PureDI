﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class IOCObjectTree
    {
        private readonly string profile;
        private readonly IDictionary<(Type, string), Type> typeMap; 

        public IOCObjectTree(string profile
          , IDictionary<(Type type, string name), Type> typeMap)
        {
            this.profile = profile;
            this.typeMap = typeMap;
        }
       // TODO complete the documentation item 3 below if and when factory types are implemented
        /// <summary>
        /// 1. mainly used to create the complete object tree at program startup
        /// 2. may be used to create object tree fragments when running tests
        /// 3. may be used to create an object or link to an existing object
        /// </summary>
        /// <typeparam name="TRootType">The concrete class (not an interface) of the top object in the tree</typeparam>
        /// <returns>an ojbect of root type</returns>
        public TRootType GetOrCreateObjectTree<TRootType>(ref IOCCDiagnostics diagnostics)
        {
            IDictionary<(Type, string), object> mapObjectsCreatedSoFar =
                new Dictionary<(Type, string), object>();
            object rootObject = Construct(typeof(TRootType));
            mapObjectsCreatedSoFar[(typeof(TRootType), IOCC.DEFAULT_DEPENDENCY_NAME)] = rootObject;
            CreateObjectTree(rootObject, mapObjectsCreatedSoFar);
            if (!(rootObject is TRootType))
            {
                throw new Exception($"object created by IOC container is not {typeof(TRootType).Name} as expected");
            }
            return (TRootType)rootObject;
        }

        /// <summary>
        /// see documentation for GetOrCreateObjectTree
        /// </summary>
        private object CreateObjectTree(object bean
          , IDictionary<(Type type, string beanName)
          , object> mapObjectsCreatedSoFar)
        {
            FieldInfo[] fieldInfos = bean.GetType().GetFields(
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fieldInfos)
            {
                IOCCInjectedDependencyAttribute attr;
                if ((attr =fieldInfo.GetCustomAttribute<IOCCInjectedDependencyAttribute>()) != null)
                {
                    (Type type, string beanName) beanId =
                      (fieldInfo.FieldType, attr.Name);
                    if (typeMap.ContainsKey(beanId))
                    {
                        Type implementation = typeMap[beanId];
                        object memberBean;
                        if (mapObjectsCreatedSoFar.ContainsKey((implementation, beanId.beanName)))
                        {
                            memberBean = mapObjectsCreatedSoFar[(implementation, beanId.beanName)];
                            fieldInfo.SetValue(bean, memberBean);
                        }
                        else
                        {
                            // reference has not yet been resolved
                            memberBean = Construct(implementation);
                            mapObjectsCreatedSoFar[(implementation, beanId.beanName)] = memberBean;
                            fieldInfo.SetValue(bean, memberBean);
                            CreateObjectTree(memberBean, mapObjectsCreatedSoFar);
                        }
                    }

                }
            }
            return bean;
        }
        /// <summary>checks if the type to be instantiated has an empty constructor and if so constructs it</summary>
        /// <param name="rootType">a concrete clasws typically part of the object tree being instantiated</param>
        /// <exception>InvalidArgumentException</exception>  
        private object Construct(Type rootType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var constructorInfos = rootType.GetConstructors(flags);
            var noArgConstructorInfo = constructorInfos.FirstOrDefault(ci => ci.GetParameters().Length == 0);
            if (noArgConstructorInfo == null)
            {
                throw new Exception($"There is no no-arg constructor for {rootType.Name}.  A no-arg constructor is required.");
            }
            return noArgConstructorInfo.Invoke(flags | BindingFlags.CreateInstance, null, new object[0], null);

        }
    }
}
