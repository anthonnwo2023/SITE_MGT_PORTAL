using Project.V1.Models;
using System;
using System.Linq.Expressions;

namespace Project.V1.Lib.Helpers;

public class ExpressionReplaceVisitor : ExpressionVisitor
{
    private readonly Expression from, to;
    public ExpressionReplaceVisitor(Expression from, Expression to)
    {
        this.from = from;
        this.to = to;
    }
    public override Expression Visit(Expression node)
    {
        return node == from ? to : base.Visit(node);
    }

    public static Expression<Func<StaticReportModel, bool>> CombineFilters(Expression<Func<StaticReportModel, bool>> filter1, Expression<Func<StaticReportModel, bool>> filter2)
    {
        var rewrittenBody1 = new ExpressionReplaceVisitor(filter1.Parameters[0], filter2.Parameters[0]).Visit(filter1.Body);
        var newFilter = Expression.Lambda<Func<StaticReportModel, bool>>(
            Expression.AndAlso(rewrittenBody1, filter2.Body), filter2.Parameters);
        // newFilter is equivalent to: x => x.A > 1 && x.B > 2.5

        return newFilter;
    }
}
