cd /d "%~dp0"
xcopy "%~dp0hosts.temp" "C:\Windows\System32\drivers\etc\hosts"
ipconfig /flushdns
pause