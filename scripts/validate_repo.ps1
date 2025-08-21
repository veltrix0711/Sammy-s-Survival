param()
$missing = @()
@("Game",".github","Docs") | ForEach-Object {
  if (-Not (Test-Path $_)) { $missing += $_ }
}
if ($missing.Count -gt 0) {
  Write-Host "Missing dirs: $($missing -join ', ')"
  exit 1
}
exit 0


