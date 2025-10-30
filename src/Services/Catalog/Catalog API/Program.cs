var builder = WebApplication.CreateBuilder(args);

//Add Services to the container
builder.Services.AddCarter(null, config => 
{ 

});
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddMarten(config => {

    config.Connection(builder.Configuration.GetConnectionString("Database")!);

}).UseLightweightSessions();

var app = builder.Build();

//Configure the HTTP request pipeline.
app.MapCarter();

app.Run();
