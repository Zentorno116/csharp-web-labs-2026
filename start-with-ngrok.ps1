param(
    [ValidateSet("taskflow", "chat")]
    [string]$App = "taskflow",
    [switch]$SkipFrontendBuild
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$env:DOTNET_ROOT = "$env:LOCALAPPDATA\Microsoft\dotnet"
$env:PATH = "$env:DOTNET_ROOT;$env:USERPROFILE\.dotnet\tools;C:\Program Files\nodejs;$env:PATH"

$ngrok = Join-Path $root "04_TaskFlowClient\tools\ngrok.exe"
if (!(Test-Path $ngrok)) {
    throw "ngrok.exe не найден в $ngrok. Распакуйте ngrok-v3-stable-windows-amd64.zip туда."
}

if ($App -eq "taskflow") {
    if (-not $SkipFrontendBuild) {
        Write-Host "[1/3] Сборка React-клиента в wwwroot Web API..." -ForegroundColor Cyan
        Push-Location (Join-Path $root "04_TaskFlowClient")
        npm run build:api
        Pop-Location
    }
    $port = 5015
    $csproj = Join-Path $root "03_TaskFlowApi\TaskFlowApi\TaskFlowApi.csproj"
} else {
    $port = 5020
    $csproj = Join-Path $root "02_SignalRChat\SignalRChat\SignalRChat.csproj"
}

Write-Host "[2/3] Запуск ASP.NET Core на http://localhost:$port ..." -ForegroundColor Cyan
$server = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$csproj`" --urls http://localhost:$port" -PassThru

try {
    Start-Sleep -Seconds 3
    Write-Host "[3/3] Запуск ngrok-туннеля (Ctrl+C для остановки)..." -ForegroundColor Green
    Write-Host "Если ngrok попросит authtoken: & `"$ngrok`" config add-authtoken <ВАШ_ТОКЕН>" -ForegroundColor Yellow
    & $ngrok http $port
} finally {
    if ($server -and !$server.HasExited) {
        Write-Host "Остановка сервера..." -ForegroundColor Yellow
        Stop-Process -Id $server.Id -Force
    }
}
