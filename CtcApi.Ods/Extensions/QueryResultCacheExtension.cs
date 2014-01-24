//Copyright (C) 2011 Bellevue College and Peninsula College
//Adapted from code Copyright (c) 2010 Pete Montgomery (http://petemontgomery.wordpress.com)
//https://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as
//published by the Free Software Foundation, either version 3 of the
//License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License and GNU General Public License along with this program.
//If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Caching;
using Common.Logging;

namespace Ctc.Ods.Extensions
{
	/// <summary>
	/// </summary>
	/// <remarks>
	/// <para>
	/// This code is adapted from <a href="https://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/">https://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/</a>
	/// </para>
	/// <para>
	/// Copyright (c) 2010 Pete Montgomery.
	/// http://petemontgomery.wordpress.com
	/// <br/>
	/// Licenced under GNU LGPL v3.
	/// http://www.gnu.org/licenses/lgpl.html
	/// </para>
	/// </remarks>
	public static class QueryResultCacheExtension
	{
	  private static readonly CacheItemPriority _cacheItemPriority = CacheItemPriority.Low;
	  private static ILog _log = LogManager.GetCurrentClassLogger();

	  /// <summary>
		/// Returns the result of the query; if possible from the cache, otherwise
		/// the query is materialized and the result cached before being returned.
		/// The cache entry has a one minute sliding expiration with normal priority.
		/// </summary>
		public static IQueryable<T> FromCache<T>(this IQueryable<T> query) where T : class
		{
			return query.FromCache(_cacheItemPriority, TimeSpan.FromMinutes(0));
		}

		/// <summary>
		/// Returns the result of the query; if possible from the cache, otherwise
		/// the query is materialized and the result cached before being returned.
		/// The cache entry has a one minute sliding expiration with normal priority.
		/// </summary>
    public static IQueryable<T> FromCache<T>(this IQueryable<T> query, TimeSpan slidingExpiration) where T : class
		{
      return query.FromCache(_cacheItemPriority, slidingExpiration);
		}

		/// <summary>
		/// Returns the result of the query; if possible from the cache, otherwise
		/// the query is materialized and the result cached before being returned.
		/// </summary>
		public static IQueryable<T> FromCache<T>(this IQueryable<T> query, CacheItemPriority priority, TimeSpan slidingExpiration) where T : class
		{
			if (HttpRuntime.AppDomainAppId != null && slidingExpiration > TimeSpan.FromMinutes(0))
			{
				string key = query.GetCacheKey();

				// try to get the query result from the cache
				var result = HttpRuntime.Cache.Get(key) as IList<T>;

			  if (result != null)
			  {
          _log.Debug(m => m("Using query results already found in HttpRuntime.Cache."));
			  }
			  else
			  {
          _log.Debug(m => m("Query results not found in HttpRuntime.Cache - executing query."));

          // todo: ... ensure that the query results do not
          // hold on to resources for your particular data source
          //
          //////// for entity framework queries, set to NoTracking
          var entityQuery = query as ObjectQuery<T>;
          if (entityQuery != null)
          {
            entityQuery.MergeOption = MergeOption.NoTracking;
          }

          // materialize the query
          result = query.ToList();

          HttpRuntime.Cache.Insert(
            key,
            result,
            null, // no cache dependency
            Cache.NoAbsoluteExpiration,
            slidingExpiration,
            priority,
            null); // no removal notification
        }

			  return result.AsQueryable();
			}

			return query;
		}

		/// <summary>
		/// Gets a cache key for a query.
		/// </summary>
		public static string GetCacheKey(this IQueryable query)
		{
			var expression = query.Expression;

			// locally evaluate as much of the query as possible
			expression = Evaluator.PartialEval(expression, QueryResultCacheExtension.CanBeEvaluatedLocally);

			// support local collections
			expression = LocalCollectionExpander.Rewrite(expression);

			// use the string representation of the expression for the cache key
			string key = expression.ToString();

			// the key is potentially very long, so use an md5 fingerprint
			// (fine if the query result data isn't critically sensitive)
			key = key.ToMd5Fingerprint();

			return key;
		}

		static Func<Expression, bool> CanBeEvaluatedLocally
		{
			get
			{
				return expression =>
				{
					// don't evaluate parameters
					if (expression.NodeType == ExpressionType.Parameter)
						return false;

					// can't evaluate queries
					if (typeof(IQueryable).IsAssignableFrom(expression.Type))
						return false;

					return true;
				};
			}
		}
	}

	/// <summary>
	/// Enables the partial evaluation of queries.
	/// </summary>
	/// <remarks>
	/// From http://msdn.microsoft.com/en-us/library/bb546158.aspx
	/// Copyright notice http://msdn.microsoft.com/en-gb/cc300389.aspx#O
	/// </remarks>
	public static class Evaluator
	{
		/// <summary>
		/// Performs evaluation & replacement of independent sub-trees
		/// </summary>
		/// <param name="expression">The root of the expression tree.</param>
		/// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
		/// <returns>A new tree with sub-trees evaluated and replaced.</returns>
		public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
		{
			return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
		}

		/// <summary>
		/// Performs evaluation & replacement of independent sub-trees
		/// </summary>
		/// <param name="expression">The root of the expression tree.</param>
		/// <returns>A new tree with sub-trees evaluated and replaced.</returns>
		public static Expression PartialEval(Expression expression)
		{
			return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
		}

		private static bool CanBeEvaluatedLocally(Expression expression)
		{
			return expression.NodeType != ExpressionType.Parameter;
		}

		/// <summary>
		/// Evaluates & replaces sub-trees when first candidate is reached (top-down)
		/// </summary>
		class SubtreeEvaluator : ExpressionVisitor
		{
			HashSet<Expression> candidates;

			internal SubtreeEvaluator(HashSet<Expression> candidates)
			{
				this.candidates = candidates;
			}

			internal Expression Eval(Expression exp)
			{
				return this.Visit(exp);
			}

			public override Expression Visit(Expression exp)
			{
				if (exp == null)
				{
					return null;
				}
				if (this.candidates.Contains(exp))
				{
					return this.Evaluate(exp);
				}
				return base.Visit(exp);
			}

			private Expression Evaluate(Expression e)
			{
				if (e.NodeType == ExpressionType.Constant)
				{
					return e;
				}
				LambdaExpression lambda = Expression.Lambda(e);
				Delegate fn = lambda.Compile();
				return Expression.Constant(fn.DynamicInvoke(null), e.Type);
			}

		}

		/// <summary>
		/// Performs bottom-up analysis to determine which nodes can possibly
		/// be part of an evaluated sub-tree.
		/// </summary>
		class Nominator : ExpressionVisitor
		{
			Func<Expression, bool> fnCanBeEvaluated;
			HashSet<Expression> candidates;
			bool cannotBeEvaluated;

			internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
			{
				this.fnCanBeEvaluated = fnCanBeEvaluated;
			}

			internal HashSet<Expression> Nominate(Expression expression)
			{
				this.candidates = new HashSet<Expression>();
				this.Visit(expression);
				return this.candidates;
			}

			public override Expression Visit(Expression expression)
			{
				if (expression != null)
				{
					bool saveCannotBeEvaluated = this.cannotBeEvaluated;
					this.cannotBeEvaluated = false;
					base.Visit(expression);
					if (!this.cannotBeEvaluated)
					{
						if (this.fnCanBeEvaluated(expression))
						{
							this.candidates.Add(expression);
						}
						else
						{
							this.cannotBeEvaluated = true;
						}
					}
					this.cannotBeEvaluated |= saveCannotBeEvaluated;
				}
				return expression;
			}
		}
	}

	/// <summary>
	/// Enables cache key support for local collection values.
	/// </summary>
	public class LocalCollectionExpander : ExpressionVisitor
	{
		public static Expression Rewrite(Expression expression)
		{
			return new LocalCollectionExpander().Visit(expression);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			// pair the method's parameter types with its arguments
			var map = node.Method.GetParameters()
					.Zip(node.Arguments, (p, a) => new { Param = p.ParameterType, Arg = a })
					.ToLinkedList();

			// deal with instance methods
			var instanceType = node.Object == null ? null : node.Object.Type;
			map.AddFirst(new { Param = instanceType, Arg = node.Object });

			// for any local collection parameters in the method, make a
			// replacement argument which will print its elements
			var replacements = (from x in map
													where x.Param != null && x.Param.IsGenericType
													let g = x.Param.GetGenericTypeDefinition()
													where g == typeof(IEnumerable<>) || g == typeof(List<>)
													where x.Arg.NodeType == ExpressionType.Constant
													let elementType = x.Param.GetGenericArguments().Single()
													let printer = MakePrinter((ConstantExpression)x.Arg, elementType)
													select new { x.Arg, Replacement = printer }).ToList();

			if (replacements.Any())
			{
				var args = map.Select(x => (from r in replacements
																		where r.Arg == x.Arg
																		select r.Replacement).SingleOrDefault() ?? x.Arg).ToList();

				node = node.Update(args.First(), args.Skip(1));
			}

			return base.VisitMethodCall(node);
		}

		ConstantExpression MakePrinter(ConstantExpression enumerable, Type elementType)
		{
			var value = (IEnumerable)enumerable.Value;
			var printerType = typeof(Printer<>).MakeGenericType(elementType);
			var printer = Activator.CreateInstance(printerType, value);

			return Expression.Constant(printer);
		}

		/// <summary>
		/// Overrides ToString to print each element of a collection.
		/// </summary>
		/// <remarks>
		/// Inherits List in order to support List.Contains instance method as well
		/// as standard Enumerable.Contains/Any extension methods.
		/// </remarks>
		class Printer<T> : List<T>
		{
			public Printer(IEnumerable collection)
			{
				this.AddRange(collection.Cast<T>());
			}

			public override string ToString()
			{
				return "{" + this.ToConcatenatedString(t => t.ToString(), "|") + "}";
			}
		}
	}
}
