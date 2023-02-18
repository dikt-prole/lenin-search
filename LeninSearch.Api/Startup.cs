using FluentValidation;
using FluentValidation.AspNetCore;
using LeninSearch.Api.Services;
using LeninSearch.Api.Validation;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Standard.Core.Search.CorpusSearching;
using LeninSearch.Standard.Core.Search.TokenVarying;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeninSearch.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddApiVersioning();
            services.AddResponseCompression(o =>
            {
                o.EnableForHttps = true;
            });

            services.AddSingleton<ILsiProvider, CachedLsiProvider>();

            services.AddSingleton<ICorpusSearch>(p => new OfflineCorpusSearch(
                p.GetService<ILsiProvider>(), p.GetService<ISearchQueryFactory>(), 50, true));
            services.AddSingleton<IStemmer, RuPorterStemmer>();
            services.AddSingleton<ISearchQueryFactory, SearchQueryFactory>();
            
            services.AddSingleton<IMemoryCache, MemoryCache>();

            services.AddSwaggerGen();

            // fluent validation
            services.AddValidatorsFromAssemblyContaining<ValidationAssemblyMarker>();
            services.AddFluentValidationAutoValidation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseMiddleware<SimpleLoggingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
