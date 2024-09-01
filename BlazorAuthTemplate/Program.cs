using BlazorAuthTemplate.client.Services.Interfaces;
using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Components;
using BlazorAuthTemplate.Components.Account;
using BlazorAuthTemplate.Data;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services;
using BlazorAuthTemplate.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddControllers();
builder.Services.AddOutputCache();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = DataUtility.GetConnectionString(builder.Configuration) ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, EmailService>();
//builder.Services.AddSingleton<IEmailSender, EmailService>();

builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, GoogleEmailService>();
builder.Services.AddSingleton<IEmailSender, GoogleEmailService>();


//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(opts =>
//{
//    opts.SwaggerDoc("v1", new() { Title = "Blog", Version = "v1" });

//    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = HeaderNames.Authorization,
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,
//        Scheme = "Bearer",
//    });

//    opts.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
//    {
//        Name = ".AspNetCore.Identity.Application",
//        In = ParameterLocation.Cookie,
//        Type = SecuritySchemeType.Http,
//        Scheme = "Cookie",
//    });

//    opts.OperationFilter<SecurityRequirementsOperationFilter>();

//});


builder.Services.AddCors(builder =>
{
	builder.AddPolicy("DefaultPolicy", policy =>
	{
		policy.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

var app = builder.Build();

app.UseCors("DefaultPolicy");

// ...

var scope = app.Services.CreateScope();
await DataUtility.ManageDataAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();

    //app.UseSwagger(o => o.RouteTemplate = "/openapi/{documentName}.json");
    //app.MapScalarApiReference();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorAuthTemplate.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.MapGet("api/blogposts", async ([FromServices] IBlogPostService blogService,
								   [FromQuery] int page = 1,
								   [FromQuery] int pageSize = 4) =>
{
	try
	{
		PagedList<BlogPostDTO> blogPosts = await blogService.GetPublishedPostsAsync(page, pageSize);
		return Results.Ok(blogPosts);
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex);
		return Results.Problem();
	}
});

app.Run();
