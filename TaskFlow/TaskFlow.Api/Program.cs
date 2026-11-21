var builder = WebApplication.CreateBuilder(args);

// rejestracja us³ug

var app = builder.Build();

// konfiguracja middleware i endpointów

app.Run();