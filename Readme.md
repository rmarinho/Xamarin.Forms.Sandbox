- Root Directory.Builds.Props file settings
  - FromSource (if you change this setting you will most likely need to obj/bin clean and reset VS)
    - false means build from nugets
    - true means build from sources
  - XamarinFormsSource
    - location on your drive of Xamarin Forms source files
  - XamarinFormsVersion
    - Version of Xamarin Forms Nuget you want to use
- Each project has specify *targets* that apply to that platform
  - Nuget.targets
    - Targets used to import nugets
  - Source.Targets
    - Targets used to import source code
  
  
  ### Using with your own project
  - Drop these global files here into a directory above your main projects https://github.com/PureWeen/Xamarin.Forms.Sandbox/tree/master/Sandbox
  - Drop the platform specific *.target files https://github.com/PureWeen/Xamarin.Forms.Sandbox/tree/master/Sandbox/Xamarin.Forms.Sandbox.Android into your platform project folders
  - Remove the references to the Xamarin.Forms nugets from your platform csproj files
  
  

## Cloning the repository

Clone repositories using recursive parameter:

	git clone https://github.com/PureWeen/Xamarin.Forms.Sandbox.git --recursive

Or Update

	git submodule update --init --recursive
	
### Cloning dotnetconf demo
```bash
git clone https://github.com/PureWeen/Xamarin.Forms.Sandbox.git 
git checkout  dotnet_conf
git submodule update --init --recursive
```

## Updating the submodule
After you have pulled down the submodule it's good to make sure the submodule is using the latest source code
or whatever branch you are wanting to test against

```bash
cd Xamarin.Forms
git checkout 4.5.0
```
