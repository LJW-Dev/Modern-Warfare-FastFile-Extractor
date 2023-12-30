# Modern Warfare FastFile Extractor
A uility to extract raw FastFiles and patch them from compressed FastFiles.

Works with warzone fastfiles, and should work with MW2 fastfiles.

# Usage

`COD FF Extractor.exe <GAME> <PATH>`

`GAME` can be either: `MW2` or `WZ`.
`PATH` is a path to a FastFile or a path to a folder of fastfiles.

All output files are written to the `output` folder.

All files ending with .ff will be parsed if it's a path to a folder. The file must end with ".ff" if its a path to a file as it is required to check if there is a patch file.

Use CascView (http://www.zezula.net/en/casc/main.html) if you are using blizzard launcher to open the .000 - .XXX files.

Two DLLs are required, `oo2core_8_win64.dll` for decompression and `MWBDiff.dll` (https://github.com/LJW-Dev/Modern-Warfare-BDiff) for patching.
