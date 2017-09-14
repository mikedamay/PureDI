using com.TheDisappointedProgrammer.IOCC;
[Bean]
      class MyClass
      {
        [BeanReference(Name="Impl1")] private MyInterface myInterface = null;
        public void UseReference()
        {
          myInterface.DoStuff();
        }
        public static void Main()
        {
            var sic = new SimpleIOCContainer();
            MyClass myClass = sic.CreateAndInjectDependencies<MyClass>();
            myClass.UseReference();     // will display "One"
        }
      }
      public interface MyInterface
      {
        void DoStuff();
      }
      [Bean(Name="Impl1")]
      public class OneImplementation :  MyInterface
      {
        public void DoStuff()
        {
          System.Console.WriteLine("One");
        }
      }
      [Bean(Name="Impl2")]
      public class TwoImplementation : MyInterface
      {
        public void DoStuff()
        {
          System.Console.WriteLine("Two");
        }
      }
  