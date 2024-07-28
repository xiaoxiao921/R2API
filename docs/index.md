---
_layout: landing
---

# Developing mods using R2API

Since the R2API `5.0.0` version update, mod creators should ideally only reference the packages they need in their C# projects and their thunderstore packages.

In the dependency array of their thunderstore manifest, they should reference the R2API packages from the `RiskofThunder` team, for example: `RiskofThunder-R2API_LobbyConfig-1.0.0`

In their C# projects, they should only get the corresponding R2API submodules dll they need.

On top of the `BaseUnityPlugin`, you should add a `BepInDependency`, for example `[BepInDependency(DirectorAPI.PluginGUID)]`

For then adding the dependency on the C# Project level, you can do that in a multitude of ways, depending on your workflow:

- Nuget Package Manager:
   -  You can access this in Visual Studio by right clicking your project within the Solution Explorer, and installing packages through that manager directly:
      -  The packages are available under the `RiskofThunder` nuget account, and you can find them through the search bar by typing `R2API`.
      
- Manually adding nuget: 
  - [We have a wiki page that also explains it](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/C%23-Programming/Assembly-References/)
  - Edit your .csproj file, and in your ItemGroup ([something like this](https://github.com/xiaoxiao921/R2Boilerplate/blob/master/ExamplePlugin/ExamplePlugin.csproj#L15-L27)), add PackageReference lines for each R2API module you depend on:  
   ```xml
   <PackageReference Include="R2API.Networking" Version="1.0.2" />
   ```
   - You can find these lines for each package on the [Risk of Thunder nuget account](https://www.nuget.org/profiles/RiskofThunder).
   
-  Download the dlls directly from thunderstore: 
   -  Add them through the Solution Explorer, right clicking your project, Add -> Project Reference, and selecting the wanted .dll files.
   
-  If you use Unity: 
   -  You can download the dlls, and drag and drop the modules' dll directly into your Unity Project under any folders that are under the root Assets/ folder.


Further information may be on the dedicated [R2API wiki](https://github.com/risk-of-thunder/R2API/wiki).
TODO: Move r2api wiki stuff to here.

Do not hesitate to ask questions in [the modding discord](https://discord.gg/5MbXZvd)!
