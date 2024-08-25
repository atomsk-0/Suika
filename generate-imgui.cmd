@echo off

cd %~dp0

if not exist vendor\Mochi.DearImGui\ (
    echo Mochi.DearImgui not found, did you forget to clone recursively? 1>&2
    exit /B 1
)

cd vendor\Mochi.DearImGui
generate.cmd