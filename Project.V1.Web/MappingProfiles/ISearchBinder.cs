using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.UriParser;
using System.Linq.Expressions;

namespace Project.V1.Web.MappingProfiles
{
    public interface ISearchBinder
    {
        Expression BindSearch(SearchClause searchClause, QueryBinderContext context);
    }

    public class RequestSearchBinder : QueryBinder, ISearchBinder
    {
        public Expression BindSearch(SearchClause searchClause, QueryBinderContext context)
        {
            SearchTermNode node = searchClause.Expression as SearchTermNode;

            Expression<Func<RequestViewModelDTO, bool>> exp = p => p.SiteId.ToString() == node.Text;

            return exp;

            //SearchTermNode node = searchClause.Expression as SearchTermNode;

            //Expression source = context.CurrentParameter;

            //Expression categoryProperty = Expression.Property(source, "Category");

            //Expression categoryPropertyString = Expression.Call(categoryProperty, "ToString", typeArguments: null, arguments: null);

            //Expression body = Expression.Call(null, StringEqualsMethodInfo, categoryPropertyString, Expression.Constant(node.Text, typeof(string)), Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison)));

            //LambdaExpression lambdaExp = Expression.Lambda(body, context.CurrentParameter);

            //return lambdaExp;
        }
    }
}
