dotnet \
    ef migrations add initial \
    --project FindMyIp.Infrastructure \
    --startup-project FindMyIp.Api \
    --context AppDbContext  \
    --output-dir Data/Migrations
    

dotnet \
    ef database update \
    --project FindMyIp.Infrastructure \
    --context AppDbContext  \
    --startup-project FindMyIp.Api