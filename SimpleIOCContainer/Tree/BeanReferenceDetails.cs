using System;

namespace com.TheDisappointedProgrammer.IOCC.Tree
{
        internal class BeanReferenceDetails
        {
            private readonly Type declaringType;
            private readonly string memberName;
            private readonly string memberBeanName;
            public bool IsRoot { get; }

            public Type DeclaringType
                => IsRoot
                    ? throw new IOCCInternalException("there are no reference details for the root bean of the tree")
                    : declaringType;

            public string MemberName
                => IsRoot
                    ? throw new IOCCInternalException("there are no reference details for the root bean of the tree")
                    : memberName;

            public string MemberBeanName
                => IsRoot
                    ? throw new IOCCInternalException("there are no reference details for the root bean of the tree")
                    : memberBeanName;


            public BeanReferenceDetails()
            {
                this.IsRoot = true;
            }

            public BeanReferenceDetails(Type declaringType, string memberName, string memberBeanName)
            {
                this.declaringType = declaringType;
                this.memberName = memberName;
                this.memberBeanName = memberBeanName;
            }

            public void Deconstruct(out Type DeclaringType, out string MemberName, out string MemberBeanName)
            {
                if (this.IsRoot)
                {
                    throw new IOCCInternalException("there are no reference details for the root bean of the tree");
                }
                DeclaringType = declaringType;
                MemberName = memberName;
                MemberBeanName = memberBeanName;
            }
        }
    }