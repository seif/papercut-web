@echo off
pushd "%~dp0"

:: process Content and Scripts by default
if "%*" == "" (
    node bundler.js ../Content ../Scripts ../App
) else (
    node bundler.js %*
)

popd

exit %ERRORLEVEL%