dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o distrib
esign distrib/*.exe
start distrib