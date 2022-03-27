using DataAccess.DbAccess;
using BaseAPI;
using BaseAPI.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Issuer"],
        ValidAudience = builder.Configuration["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"]))
    };
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo()
    {
        Description = "Base .NET 6 minimal API setup by M Arslan",
        Title = "Base Api",
        Version = "v1",
        Contact = new OpenApiContact()
        {
            Name = "Muhammad Arslan",
            Url = new Uri("https://muhammadarslangulzar.business.site/")
        }
    });

    setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    setup.OperationFilter<AddAuthorizationHeaderOperationHeader>();
});
builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddSingleton<IUserData, UserData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.ConfigureApi();
app.Run();

app.MapPost("/token", async (HttpContext http, UserModel inputUser) =>
{
    //using (var dbContext = dbContextFactory.CreateDbContext())
    //{
    //    if (!string.IsNullOrEmpty(inputUser.Username) &&
    //        !string.IsNullOrEmpty(inputUser.Password))
    //    {
    //        var loggedInUser = await dbContext.Users
    //            .FirstOrDefaultAsync(user => user.Username == inputUser.Username
    //            && user.Password == inputUser.Password);
    //        if (loggedInUser == null)
    //        {
    //            http.Response.StatusCode = 401;
    //            return;
    //        }

    //        var claims = new[]
    //        {
    //            new Claim(JwtRegisteredClaimNames.Sub, loggedInUser.Username),
    //            new Claim(JwtRegisteredClaimNames.Name, loggedInUser.Username),
    //            new Claim(JwtRegisteredClaimNames.Email, loggedInUser.Email)
    //        };

    //        var token = new JwtSecurityToken
    //        (
    //            issuer: builder.Configuration["Issuer"],
    //            audience: builder.Configuration["Audience"],
    //            claims: claims,
    //            expires: DateTime.UtcNow.AddDays(60),
    //            notBefore: DateTime.UtcNow,
    //            signingCredentials: new SigningCredentials(
    //                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"])),
    //                SecurityAlgorithms.HmacSha256)
    //        );

    //        await http.Response.WriteAsJsonAsync(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    //        return;
    //    }

    //    http.Response.StatusCode = 400;
    //}
}).Produces(200).WithTags("Authentication").Produces(401);



public class AddAuthorizationHeaderOperationHeader : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var isAuthorized = actionMetadata.Any(metadataItem => metadataItem is AuthorizeAttribute);
        var allowAnonymous = actionMetadata.Any(metadataItem => metadataItem is AllowAnonymousAttribute);

        if (!isAuthorized || allowAnonymous)
        {
            return;
        }
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Security = new List<OpenApiSecurityRequirement>();

        //Add JWT bearer type
        operation.Security.Add(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            }
        );
    }
}