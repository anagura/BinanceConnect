cd /d %~dp0

del /Q /F /S .\bin\debug\netcoreapp2.1
del /Q /F /S .\*.zip

dotnet publish -r linux-arm

powershell -File zip.ps1

scp ./publish.zip pi@192.168.2.37:/home/pi/netcore/
