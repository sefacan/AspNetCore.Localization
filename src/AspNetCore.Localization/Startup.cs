using AspNetCore.Localization.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Linq;

namespace AspNetCore.Localization
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddResponseCompression();

            services.TryAddSingleton<IStringLocalizerFactory, SqlStringLocalizerFactory>();
            services.TryAddSingleton<IHtmlLocalizerFactory, HtmlLocalizerFactory>();
            services.TryAddTransient(typeof(IHtmlLocalizer<>), typeof(HtmlLocalizer<>));
            services.TryAddTransient<IViewLocalizer, ViewLocalizer>();
            services.AddScoped<LanguageAccessor>();
            services.AddHttpContextAccessor();

            services.AddRouting(options =>
            {
                //add constraint key for language
                options.ConstraintMap["lang"] = typeof(LanguageParameterTransformer);
            });

            // add app data context
            services.AddDbContextPool<AppDataContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("Default"));
            });

            services.AddControllersWithViews()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseResponseCompression();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization(options =>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dataContext = scope.ServiceProvider.GetService<AppDataContext>();
                    var cultures = dataContext.Languages.Select(l => new CultureInfo(l.Culture)).ToList();

                    options.SupportedCultures = cultures;
                    options.SupportedUICultures = cultures;
                    options.DefaultRequestCulture = new RequestCulture(cultures.First());
                }

                options.RequestCultureProviders.Insert(0, new RouteSegmentCultureProvider());
            });

            app.UseEndpoints(endpoints =>
            {
                var pattern = string.Empty;
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dataContext = scope.ServiceProvider.GetService<AppDataContext>();
                    var defaultLanguage = dataContext.Languages.FirstOrDefault();

                    pattern = $"{{language:lang={defaultLanguage.TwoLetterIsoCode}}}";
                }

                endpoints.MapControllerRoute(
                    name: "HomePage",
                    pattern: $"{pattern}",
                    new { action = "Index", controller = "Home" });

                endpoints.MapControllerRoute(
                    name: "Privacy",
                    pattern: $"{pattern}/privacy",
                    new { action = "Privacy", controller = "Home" });

                endpoints.MapControllerRoute(
                    name: "ChangeLanguage",
                    pattern: $"{pattern}/changelanguage/{{langCode}}",
                    new { action = "ChangeLanguage", controller = "Home" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: $"{pattern}/{{controller=Home}}/{{action=Index}}/{{id?}}");
            });
        }
    }
}
