# Script PowerShell para ejecutar pruebas
param(
    [string]$TestFilter = "*",
    [switch]$Coverage = $false,
    [string]$OutputPath = "TestResults"
)

Write-Host "🧪 Ejecutando Pruebas de Hilos - Sistema MIVET" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green

# Limpiar resultados anteriores
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# Configurar argumentos de dotnet test
$testArgs = @(
    "test"
    "--logger", "trx;LogFileName=TestResults.trx"
    "--logger", "html;LogFileName=TestResults.html"
    "--results-directory", $OutputPath
    "--verbosity", "normal"
)

if ($TestFilter -ne "*") {
    $testArgs += "--filter", $TestFilter
}

if ($Coverage) {
    $testArgs += "--collect", "XPlat Code Coverage"
}

# Ejecutar pruebas
Write-Host "🔍 Filtro de pruebas: $TestFilter" -ForegroundColor Yellow
Write-Host "📊 Cobertura de código: $Coverage" -ForegroundColor Yellow
Write-Host ""

try {
    $startTime = Get-Date
    & dotnet @testArgs
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    Write-Host ""
    Write-Host "✅ Pruebas completadas en $($duration.TotalSeconds.ToString('F2')) segundos" -ForegroundColor Green
    Write-Host "📁 Resultados guardados en: $OutputPath" -ForegroundColor Cyan
    
    # Mostrar resumen de archivos generados
    Get-ChildItem $OutputPath -Recurse | ForEach-Object {
        Write-Host "   📄 $($_.Name)" -ForegroundColor Gray
    }
}
catch {
    Write-Host "❌ Error ejecutando pruebas: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}