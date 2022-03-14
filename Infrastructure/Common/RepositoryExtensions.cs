using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models.Results;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common
{
    public static class RepositoryExtensions
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> results,
            IDictionary<string, string> queryParams)
        {
            var sort = string.Empty;
            var order = string.Empty;
            
            if(queryParams.ContainsKey("sort"))
                sort = queryParams["sort"];
            if(queryParams.ContainsKey("order"))
                order = queryParams["order"];

            if (!string.IsNullOrWhiteSpace(sort))
            {
                
                var type = typeof(T);
                var sortProperty = type.GetProperty(sort.FirstCharToUpper());
                
                if (sortProperty != null)
                {
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, sortProperty);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    string orderMethodName = "OrderByDescending"; 
                    if (!string.IsNullOrWhiteSpace(order) && order.Equals("asc"))
                    {
                        
                        orderMethodName = "OrderBy";
                    }

                    var resultExp = Expression.Call(
                        typeof(Queryable), 
                        orderMethodName, 
                        new Type[] { type, orderByExp.ReturnType }, 
                        results.Expression, 
                        Expression.Quote(orderByExp));
                    
                    results = (IOrderedQueryable<T>)results.Provider.CreateQuery<T>(resultExp);
                }
                else
                {
                    throw new Exception($"Atributo {sort} não encontrado na entidade {typeof(T).Name} ao tentar ordenar elementos");    
                }
            }
            return results;
        }

        public static IQueryable<T> Search<T>(this IQueryable<T> results,
            IDictionary<string, string> queryParams,
            IList<string> nonSearchableParameters = null)
        {
            nonSearchableParameters ??= new List<string> {"max", "offset", "sort", "order"};

            var searchParameters = queryParams.ToList();
            searchParameters.RemoveAll(kv => nonSearchableParameters.Contains(kv.Key));

            foreach (var keyValuePair in searchParameters)
            {
                var type = typeof(T);
                var whereProperty = type.GetProperty(keyValuePair.Key.FirstCharToUpper());
                if (whereProperty != null)
                {
                    var parameter = Expression.Parameter(type, "t");
                    var property = Expression.Property(parameter, whereProperty);
                    Expression body;
                    var converter = TypeDescriptor.GetConverter(whereProperty.PropertyType);

                    if (!converter.CanConvertFrom(typeof(string)))
                    {
                        throw new NotSupportedException();
                    }

                    var propertyValue = converter.ConvertFromInvariantString(keyValuePair.Value.ToString().ToLower());
                    var constant = Expression.Constant(propertyValue);
                    var valueExpression = Expression.Convert(constant, whereProperty.PropertyType);
                    if (whereProperty.PropertyType.IsEnum || !(whereProperty.PropertyType == typeof(string)))
                    {
                        body = Expression.Equal(property, valueExpression);
                    }
                    else
                    {
                        MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        body = Expression.Call(property, method, valueExpression);
                    }

                    Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(body, parameter);
                    return results.Where(expression);
                }

                throw new Exception($"Atributo {keyValuePair.Key} não encontrado na entidade {typeof(T).Name} ao tentar filtrar elementos");
            }
            return results;
        }

        public static async Task<PaginatedResult<T>> GetPage<T>(
            this IQueryable<T> results,
            IDictionary<string, string> queryParams, 
            CancellationToken cancellationToken = default)
        {
            int pageSize = 10;
            if (queryParams.ContainsKey("max"))
            {
                pageSize = int.Parse(queryParams["max"]);
            }
            int offset = 0;
            if (queryParams.ContainsKey("offset"))
            {
                offset = int.Parse(queryParams["offset"]);
            }

            return new PaginatedResult<T>
            {
                TotalCount = results.Count(),
                Results = await results
                    .Skip(offset * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}