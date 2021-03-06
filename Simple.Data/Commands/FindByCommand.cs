﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Simple.Data.Commands
{
    class FindByCommand : ICommand
    {
        public bool IsCommandFor(string method)
        {
            return method.StartsWith("FindBy") || method.StartsWith("find_by_", StringComparison.InvariantCultureIgnoreCase);
        }

        public Func<object[], object> CreateDelegate(DataStrategy dataStrategy, DynamicTable table, InvokeMemberBinder binder, object[] args)
        {
            var criteriaExpression = ExpressionHelper.CriteriaDictionaryToExpression(table.GetQualifiedName(), MethodNameParser.ParseFromBinder(binder, args));
            try
            {
                var func = dataStrategy.Adapter.CreateFindOneDelegate(table.GetQualifiedName(), criteriaExpression);
                return a => new SimpleRecord(func(a), table.GetQualifiedName(), dataStrategy);
            }
            catch (NotImplementedException)
            {
                return null;
            }
        }

        public object Execute(DataStrategy dataStrategy, DynamicTable table, InvokeMemberBinder binder, object[] args)
        {
            var criteriaExpression = ExpressionHelper.CriteriaDictionaryToExpression(table.GetQualifiedName(), MethodNameParser.ParseFromBinder(binder, args));
            var data = dataStrategy.FindOne(table.GetQualifiedName(), criteriaExpression);
            return data != null ? new SimpleRecord(data, table.GetQualifiedName(), dataStrategy) : null;
        }
    }

    class QueryByCommand : ICommand
    {
        public bool IsCommandFor(string method)
        {
            return method.StartsWith("QueryBy") || method.StartsWith("query_by_", StringComparison.InvariantCultureIgnoreCase);
        }

        public Func<object[], object> CreateDelegate(DataStrategy dataStrategy, DynamicTable table, InvokeMemberBinder binder, object[] args)
        {
            throw new NotImplementedException();
        }

        public object Execute(DataStrategy dataStrategy, DynamicTable table, InvokeMemberBinder binder, object[] args)
        {
            var criteriaExpression = ExpressionHelper.CriteriaDictionaryToExpression(table.GetQualifiedName(), MethodNameParser.ParseFromBinder(binder, args));
            return new SimpleQuery(dataStrategy.Adapter, table.GetQualifiedName()).Where(criteriaExpression);
        }
    }
}
