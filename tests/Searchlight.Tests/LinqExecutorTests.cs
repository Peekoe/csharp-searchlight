﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Searchlight;
using Searchlight.Parsing;
using Searchlight.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Searchlight.Tests
{
    [TestClass]
    public class LinqExecutorTests
    {
        private SearchlightDataSource src;

        public class EmployeeObj
        {
            public string name { get; set; }
            public int id { get; set; }
            public DateTime hired { get; set; }
            public decimal paycheck { get; set; }
            public bool onduty { get; set; }
        }

        public List<EmployeeObj> GetTestList()
        {
            List<EmployeeObj> list = new List<EmployeeObj>();
            list.Add(new EmployeeObj() { hired = DateTime.Today, id = 1, name = "Alice Smith", onduty = true, paycheck = 1000.00m });
            list.Add(new EmployeeObj() { hired = DateTime.Today.AddMonths(-1), id = 2, name = "Bob Rogers", onduty = true, paycheck = 1000.00m });
            list.Add(new EmployeeObj() { hired = DateTime.Today.AddMonths(-6), id = 3, name = "Charlie Prentiss", onduty = false, paycheck = 800.0m });
            list.Add(new EmployeeObj() { hired = DateTime.Today.AddMonths(-12), id = 4, name = "Danielle O'Shea", onduty = false, paycheck = 1200.0m });
            list.Add(new EmployeeObj() { hired = DateTime.Today.AddMonths(1), id = 5, name = "Ernest Nofzinger", onduty = true, paycheck = 1000.00m });
            return list;
        }

        public LinqExecutorTests()
        {
            this.src = SearchlightDataSource.Create(typeof(EmployeeObj), ModelFieldMode.Loose);
        }

        [TestMethod]
        public void QueryListCollection()
        {
            var list = GetTestList();

            // Construct a simple query and check that it comes out correct
            var query = src.ParseFilter("id gt 1 and paycheck le 1000");
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(ConjunctionType.AND, query[0].Conjunction);
            Assert.AreEqual("id", ((CriteriaClause)query[0]).Column.FieldName);
            Assert.AreEqual(OperationType.GreaterThan, ((CriteriaClause)query[0]).Operation);
            Assert.AreEqual(1, ((CriteriaClause)query[0]).Value);
            Assert.AreEqual("paycheck", ((CriteriaClause)query[1]).Column.FieldName);
            Assert.AreEqual(OperationType.LessThanOrEqual, ((CriteriaClause)query[1]).Operation);
            Assert.AreEqual(1000.0m, ((CriteriaClause)query[1]).Value);

            // Execute the query and ensure that each result matches
            var results = LinqExecutor.QueryCollection<EmployeeObj>(src, query, list);
            Assert.IsTrue(results.Count() == 3);
            foreach (var e in results)
            {
                Assert.IsTrue(e.id > 1);
                Assert.IsTrue(e.paycheck <= 1000.0m);
            }
        }


        [TestMethod]
        public void NestedClauseQuery()
        {
            var list = GetTestList();

            // Construct a simple query and check that it comes out correct
            var query = src.ParseFilter("id gt 1 and (paycheck lt 1000 or paycheck gt 1000)");
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(ConjunctionType.AND, query[0].Conjunction);
            Assert.AreEqual("id", ((CriteriaClause)query[0]).Column.FieldName);
            Assert.AreEqual(OperationType.GreaterThan, ((CriteriaClause)query[0]).Operation);
            Assert.AreEqual(1, ((CriteriaClause)query[0]).Value);

            // Did we get a nested clause?
            var cc = query[1] as CompoundClause;
            Assert.IsNotNull(cc);
            Assert.AreEqual(2, cc.Children.Count);
            Assert.AreEqual("paycheck", ((CriteriaClause)cc.Children[0]).Column.FieldName);
            Assert.AreEqual(OperationType.LessThan, ((CriteriaClause)cc.Children[0]).Operation);
            Assert.AreEqual(1000.0m, ((CriteriaClause)cc.Children[0]).Value);
            Assert.AreEqual("paycheck", ((CriteriaClause)cc.Children[1]).Column.FieldName);
            Assert.AreEqual(OperationType.GreaterThan, ((CriteriaClause)cc.Children[1]).Operation);
            Assert.AreEqual(1000.0m, ((CriteriaClause)cc.Children[1]).Value);

            // Execute the query and ensure that each result matches
            var results = LinqExecutor.QueryCollection<EmployeeObj>(src, query, list);
            Assert.IsTrue(results.Count() == 2);
            foreach (var e in results)
            {
                Assert.IsTrue(e.id > 1);
                Assert.IsTrue(e.paycheck == 800.0m || e.paycheck == 1200.0m);
            }
        }

        [TestMethod]
        public void BetweenQuery()
        {
            var list = GetTestList();

            // Note that the "between" clause is inclusive
            var query = src.ParseFilter("id between 2 and 4");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(ConjunctionType.NONE, query[0].Conjunction);
            Assert.AreEqual("id", ((BetweenClause)query[0]).Column.FieldName);
            Assert.AreEqual(2, ((BetweenClause)query[0]).LowerValue);
            Assert.AreEqual(4, ((BetweenClause)query[0]).UpperValue);

            // Execute the query and ensure that each result matches
            var results = LinqExecutor.QueryCollection<EmployeeObj>(src, query, list);
            Assert.IsTrue(results.Count() == 3);
            foreach (var e in results)
            {
                Assert.IsTrue(e.id > 1);
                Assert.IsTrue(e.id < 5);
            }
        }

        [TestMethod]
        public void StartsWithQuery()
        {
            var list = GetTestList();

            // Note that the "between" clause is inclusive
            var query = src.ParseFilter("name startswith 'A'");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(ConjunctionType.NONE, query[0].Conjunction);
            Assert.AreEqual("name", ((CriteriaClause)query[0]).Column.FieldName);
            Assert.AreEqual(OperationType.StartsWith, ((CriteriaClause)query[0]).Operation);
            Assert.AreEqual("A", ((CriteriaClause)query[0]).Value);

            // Execute the query and ensure that each result matches
            var results = LinqExecutor.QueryCollection<EmployeeObj>(src, query, list);
            Assert.IsTrue(results.Count() == 1);
            foreach (var e in results)
            {
                Assert.IsTrue(e.name[0] == 'A');
            }
        }

        [TestMethod]
        public void EndsWithQuery()
        {
            var list = GetTestList();

            // Note that the "between" clause is inclusive
            var query = src.ParseFilter("name endswith 's'");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(ConjunctionType.NONE, query[0].Conjunction);
            Assert.AreEqual("name", ((CriteriaClause)query[0]).Column.FieldName);
            Assert.AreEqual(OperationType.EndsWith, ((CriteriaClause)query[0]).Operation);
            Assert.AreEqual("s", ((CriteriaClause)query[0]).Value);

            // Execute the query and ensure that each result matches
            var results = LinqExecutor.QueryCollection<EmployeeObj>(src, query, list);
            Assert.IsTrue(results.Count() == 2);
            foreach (var e in results)
            {
                Assert.IsTrue(e.name.EndsWith("s"));
            }
        }


        [TestMethod]
        public void ContainsQuery()
        {
            var list = GetTestList();

            // Note that the "between" clause is inclusive
            var query = src.ParseFilter("name contains 's'");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(ConjunctionType.NONE, query[0].Conjunction);
            Assert.AreEqual("name", ((CriteriaClause)query[0]).Column.FieldName);
            Assert.AreEqual(OperationType.Contains, ((CriteriaClause)query[0]).Operation);
            Assert.AreEqual("s", ((CriteriaClause)query[0]).Value);

            // Execute the query and ensure that each result matches
            var results = LinqExecutor.QueryCollection<EmployeeObj>(src, query, list);
            Assert.IsTrue(results.Count() == 3);
            foreach (var e in results)
            {
                Assert.IsTrue(e.name.Contains("s"));
            }
        }
    }
}
