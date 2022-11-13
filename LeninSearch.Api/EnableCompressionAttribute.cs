using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace LeninSearch.Api
{
    public class EnableCompressionAttribute : MiddlewareFilterAttribute
    {
        public EnableCompressionAttribute()
            : base(typeof(EnableCompressionAttribute))
        { }

        public void Configure(IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseResponseCompression();
    }
}