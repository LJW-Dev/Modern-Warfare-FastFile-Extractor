# Modern-Warfare-FastFile-Extractor
A uility to extract raw FastFiles and patch them from compressed FastFiles.

Currently only works with Modern Warfare 2022 FastFiles.

# Usage

`MW2 FF Extractor.exe <path to folder or .ff>`

All files ending with .ff will be parsed if it s a path to a folder. The file must end with ".ff" if its a path to a file as it is required to check if there is a patch file.

Use CascView (http://www.zezula.net/en/casc/main.html) if you are using blizzard launcher to open the .000 - .XXX files.

Two DLLs are required, `oo2core_8_win64.dll` for decompression and `MWBDiff.dll` (https://github.com/LJW-Dev/Modern-Warfare-BDiff) for patching.
