﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Spring.TestFixture.Collections.Generic
{
    /// <summary>
    /// Basic functionality test cases for implementation of <see cref="ICollection{T}"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public abstract class CollectionTestFixture<T> : EnumerableTestFixture<T>
    {
        private int _sampleSize = 9;
        protected int SampleSize
        {
            get { return _sampleSize; }
            set { _sampleSize = value; }
        }
        protected T[] Samples { get; set; }
        protected bool IsUnique
        {
            get { return Options.Has(CollectionOptions.Unique); }
            set { Options = Options.Set(CollectionOptions.Unique, value); }
        }
        protected bool IsReadOnly
        {
            get { return Options.Has(CollectionOptions.ReadOnly); }
            set { Options = Options.Set(CollectionOptions.ReadOnly, value); }
        }

        protected CollectionOptions Options { get; set; }

        public static void ToStringContainsToStringOfElements(ICollection<T> c)
        {
            String s = c.ToString();
            foreach (var element in c)
            {
                Assert.IsTrue(s.IndexOf(element.ToString()) >= 0);
            }
        }

        /// <summary>
        /// Only evaluates option <see cref="CollectionOptions.Unique"/> and
        /// <see cref="CollectionOptions.ReadOnly"/>.
        /// </summary>
        /// <param name="options"></param>
        protected CollectionTestFixture(CollectionOptions options)
        {
            Options = options;
        }

        protected override sealed IEnumerable<T> NewEnumerable()
        {
            return NewCollectionFilledWithSample();
        }

        protected virtual ICollection<T> NewCollectionFilledWithSample()
        {
            ICollection<T> collection = NewCollection();
            foreach (T o in Samples)
            {
                collection.Add(o);
            }
            return collection;
        }

        protected virtual T[] NewSamples()
        {
            return TestData<T>.MakeTestArray(SampleSize);
        }

        /// <summary>
        /// Return a new empty <see cref="ICollection{T}"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract ICollection<T> NewCollection();

        [SetUp]
        public virtual void SetUpSamples()
        {
            Samples = NewSamples();
        }

        [Test] public virtual void CountAccurately()
        {
            Assert.That(NewCollection().Count, Is.EqualTo(0));
            Assert.That(NewCollectionFilledWithSample().Count, Is.EqualTo(SampleSize));
        }

        [Test] public virtual void CopyToChokesWithNullArray()
        {
            Assert.Throws<ArgumentNullException>(
                () => NewCollectionFilledWithSample().CopyTo(null, 0));
        }

        [Test] public virtual void CopyToChokesWithNegativeIndex()
        {
            ICollection<T> c = NewCollectionFilledWithSample();
            Assert.Throws<ArgumentOutOfRangeException>(
                () => c.CopyTo(new T[c.Count], -1));
        }

        [Test] public virtual void CopyToChokesWhenArrayIsTooSmallToHold()
        {
            ICollection<T> c = NewCollectionFilledWithSample();
            if(c.Count == 0) Assert.Pass("Skip because of empty colleciton");
            Assert.Throws<ArgumentException>(() => c.CopyTo(new T[c.Count - 1], 0));
            Assert.Throws<ArgumentException>(() => c.CopyTo(new T[c.Count], 1));
        }

        [Test] public virtual void CopyToZeroLowerBoundArray()
        {
            ICollection<T> c = NewCollectionFilledWithSample();
            T[] target = new T[c.Count];
            c.CopyTo(target, 0);
            CollectionAssert.AreEqual(target, c);
        }

        [Test] public virtual void CopyToDoesNothingWhenCollectionIsEmpty()
        {
            ICollection<T> c = NewCollection();
            c.CopyTo(new T[0], 0);
        }

        [Test] public virtual void ContainsReturnFalseOnEmptyCollection()
        {
            ICollection<T> c = NewCollection();
            Assert.IsFalse(c.Contains(TestData<T>.Zero));
            AssertContainsNoSample(c);
        }

        [Test] public virtual void ContainsSunnyDay()
        {
            ICollection<T> c = NewCollectionFilledWithSample();
            AssertContainsAllSamples(c);
            T notexists = TestData<T>.MakeData(SampleSize);
            Assert.IsFalse(c.Contains(notexists));
        }

        [Test] public virtual void ContainsReportsTrueWhenElementsAddedButNotYetRemoved() {
            var q = NewCollectionFilledWithSample();
			
            for (int i = 0; i < SampleSize; ++i) {
                var item = TestData<T>.MakeData(i);
                Assert.IsTrue(q.Contains(item));
                q.Remove(item);
                Assert.IsFalse(q.Contains(item));
            }
        }

        [Test] public virtual void ClearEmptiesTheCollectionWhenSupported()
        {
            ICollection<T> c = NewCollectionFilledWithSample();
            try { c.Clear(); }
            catch (NotSupportedException e)
            {
                if (IsReadOnly) return;
                throw SystemExtensions.PreserveStackTrace(e);
            }
            Assert.That(c.Count, Is.EqualTo(0));
            AssertContainsNoSample(c);
        }

        [Test] public virtual void AddAllSamplesSuccessfullyWhenSupported()
        {
            ICollection<T> c = NewCollection();
            foreach (T sample in Samples)
            {
                try { c.Add(sample); }
                catch (NotSupportedException e)
                {
                    if (IsReadOnly) return;
                    throw SystemExtensions.PreserveStackTrace(e);
                }
            }
            Assert.That(c.Count, Is.EqualTo(SampleSize));
            AssertContainsAllSamples(c);
        }

        [Test] public virtual void AddDuplicateSuccessfullyWhenSupported()
        {
            if (IsUnique) Assert.Pass("Skip because collection doesn't allow duplicates.");
            if (SampleSize<=0) Assert.Pass("Skip because 0 capacity collection");
            ICollection<T> c = NewCollection();
            T sample = Samples[0];
            try { c.Add(sample); }
            catch (NotSupportedException e)
            {
                if (IsReadOnly) return;
                throw SystemExtensions.PreserveStackTrace(e);
            }
            Assert.That(c.Count, Is.EqualTo(1));
            Assert.IsTrue(c.Contains(sample));
            c.Add(sample);
            Assert.That(c.Count, Is.EqualTo(2));
        }

        [Test] public virtual void RemoveAllSamplesSuccessfullyWhenSupported()
        {
            ICollection<T> c = NewCollectionFilledWithSample();
            Assert.That(c.Count, Is.EqualTo(SampleSize));
            AssertContainsAllSamples(c);
            for (int i = 1; i < SampleSize; i += 2)
            {
                try {Assert.IsTrue(c.Remove(TestData<T>.MakeData(i)));}
                catch (NotSupportedException e)
                {
                    if (IsReadOnly) return;
                    throw SystemExtensions.PreserveStackTrace(e);
                }
            }
            for (int i = 0; i < SampleSize; i += 2)
            {
                try
                {
                    Assert.IsTrue(c.Remove(TestData<T>.MakeData(i)));
                    Assert.IsFalse(c.Remove(TestData<T>.MakeData(i + 1)));
                }
                catch (NotSupportedException e)
                {
                    if (IsReadOnly) return;
                    throw SystemExtensions.PreserveStackTrace(e);
                }
            }
            AssertContainsNoSample(c);
        }

        [Test] public virtual void RemoveOnlyOneOfDuplicatesWhenSupported()
        {
            if (IsUnique) Assert.Pass("Skip because collection doesn't allow duplicates.");
            ICollection<T> c = NewCollection();
            T sample = TestData<T>.One;
            try { c.Add(sample); }
            catch (NotSupportedException e)
            {
                if (IsReadOnly) return;
                throw SystemExtensions.PreserveStackTrace(e);
            }
            Assert.That(c.Count, Is.EqualTo(1));
            Assert.IsTrue(c.Contains(sample));
            c.Add(sample);
            Assert.That(c.Count, Is.EqualTo(2));
            try { c.Remove(sample); }
            catch (NotSupportedException e)
            {
                if (IsReadOnly) return;
                throw SystemExtensions.PreserveStackTrace(e);
            }
            Assert.IsTrue(c.Contains(sample));
            Assert.That(c.Count, Is.EqualTo(1));
        }

        [Test] public virtual void IsReadOnlyAsExpected()
        {
            ICollection<T> c = NewCollection();
            Assert.That(c.IsReadOnly, Is.EqualTo(IsReadOnly));
        }

        [Test] public virtual void EnumerateThroughAllElements() 
        {
            var c = NewCollectionFilledWithSample();
            CollectionAssert.AreEquivalent(Samples, c);
        }

        [Test] public virtual void EnumeratorFailsWhenCollectionIsModified()
        {
            EnumeratorFailsWhen(c => c.Remove(TestData<T>.Two));
            EnumeratorFailsWhen(c => c.Add(TestData<T>.Three));
        }

        [Test] public virtual void ToStringContainsToStringOfAllElements()
        {
            Options.SkipWhenNot(CollectionOptions.ToStringPrintItems);
            ToStringContainsToStringOfElements(NewCollectionFilledWithSample());
        }

        private void EnumeratorFailsWhen(Action<ICollection<T>> action)
        {
            var c = NewCollection();
            c.Add(TestData<T>.One);
            c.Add(TestData<T>.Two);
            Assert.Throws<InvalidOperationException>(
                delegate
                    {
                        var it = c.GetEnumerator();
                        for (int i = AntiHangingLimit - 1; i >= 0 && it.MoveNext(); i--)
                        {
                            action(c);
                        }
                    });
            c.Add(TestData<T>.Two);
            var e = c.GetEnumerator();
            action(c);
            Assert.Throws<InvalidOperationException>(e.Reset);
        }

        protected void AssertContainsAllSamples(ICollection<T> c)
        {
            foreach (T sample in Samples)
            {
                Assert.IsTrue(c.Contains(sample));
            }
        }

        protected void AssertContainsNoSample(ICollection<T> c)
        {
            foreach (T sample in Samples)
            {
                Assert.IsFalse(c.Contains(sample));
            }
        }

        protected void SkipForCurrentQueueImplementation()
        {
            Assert.Pass("Skip test that is not applicable to current implmenetation.");
        }
    }
}