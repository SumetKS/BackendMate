# BackendMate
Test connection on your machine or server with dependency server (Telnet).

## Prerequisite
- Net core SDK 2.0 or above

## How to use
1. edit *'config.txt'* in BackendMateConsole folder
2. add host or ip and port that you want to test in that file
   - `github 443`
then save file
3. use CLI go to BackendMateConsole folder run following command
   - `dotnet build`
   - `dotnet .\bin\Debug\netcoreapp2.0\BackendMateConsole.dll`
4. Result will display in CLI windows and save in text file in .\bin\Debug\netcoreapp2.0\TestResult_xxxx.txt
