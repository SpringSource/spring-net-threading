using System;
using NUnit.Framework;

namespace Spring
{
    public abstract class ThreadingTestFixture
    {
        public const int DEFAULT_COLLECTION_SIZE = 20;
        public const long SHORT_DELAY_MS = 300;
        public const long SMALL_DELAY_MS = SHORT_DELAY_MS * 5;
        public const long MEDIUM_DELAY_MS = SHORT_DELAY_MS * 10;
        public const long LONG_DELAY_MS = SHORT_DELAY_MS * 50;
        public static readonly TimeSpan SHORT_DELAY = TimeSpan.FromMilliseconds(SHORT_DELAY_MS);
        public static readonly TimeSpan SMALL_DELAY = TimeSpan.FromMilliseconds(SMALL_DELAY_MS);
        public static readonly TimeSpan MEDIUM_DELAY = TimeSpan.FromMilliseconds(MEDIUM_DELAY_MS);
        public static readonly TimeSpan LONG_DELAY = TimeSpan.FromMilliseconds(LONG_DELAY_MS);

        protected TestThreadManager ThreadManager { get; private set; }

        [SetUp]
        public virtual void SetUpThreadManager()
        {
            ThreadManager = new TestThreadManager();
        }

        [TearDown]
        public virtual void TearDownThreadManager()
        {
            ThreadManager.TearDown();
        }
    }

    public abstract class ThreadingTestFixture<T> : ThreadingTestFixture
    {
        protected static T eight = TestData<T>.MakeData(8);
        protected static T five = TestData<T>.MakeData(5);
        protected static T four = TestData<T>.MakeData(4);
        protected static T m1 = TestData<T>.MakeData(-1);
        protected static T m10 = TestData<T>.MakeData(-10);
        protected static T m2 = TestData<T>.MakeData(-2);
        protected static T m3 = TestData<T>.MakeData(-3);
        protected static T m4 = TestData<T>.MakeData(-4);
        protected static T m5 = TestData<T>.MakeData(-5);
        protected static T nine = TestData<T>.MakeData(9);
        protected static T one = TestData<T>.MakeData(1);
        protected static T seven = TestData<T>.MakeData(7);

        protected static T six = TestData<T>.MakeData(6);
        protected static T three = TestData<T>.MakeData(3);
        protected static T two = TestData<T>.MakeData(2);
        protected static T zero = TestData<T>.MakeData(0);
    }
}