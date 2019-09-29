# iorg
Reorganize images from a source directory into a directory tree - cli project

## Compile for Standalone

    dotnet publish -r win-x64 -c release

_OR_

    dotnet publish -r osx.10.14-x64 -c release 

The cli call prints out wher to find the resulting binary - in my case: 

<samp><small>`iorg -> ~/ProjectsCore/imgorg/iorg/bin/Release/netcoreapp3.0/osx.10.14-x64/publish/`</small></samp>

And the self contained binary for macOS I find there is named `iorg` (~70MB in size):

<small>`ls -la ~/ProjectsCore/imgorg/iorg/bin/Release/netcoreapp3.0/osx.10.14-x64/publish/`</small>
| 1  | 1 | 1 | 1 | 1 | 1 | 1 | 
 | --- | --- | --- | --- | ---: | --- | --- |
drwxr-xr-x | 4   | RSB | staff |      128 | Sep 29 14:47 | . |
drwxr-xr-x | 254 | RSB | staff |     8128 | Sep 29 14:46 | .. |
-rwxr-xr-x |   1 | RSB | staff | 73927287 | Sep 29 14:47 | **iorg** |
-rw-r--r-- |   1 | RSB | staff |      812 | Sep 29 14:46 | iorg.pdb |

For a more detailed description see [ImgOrg.Tests](https://github.com/Burkhardt/ImgOrg.Test).