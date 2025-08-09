using MelonLoader;

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTrademark(NEP.MagPerception.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: MelonInfo(typeof(NEP.MagPerception.Main), NEP.MagPerception.BuildInfo.Name, NEP.MagPerception.BuildInfo.Version, NEP.MagPerception.BuildInfo.Author, NEP.MagPerception.BuildInfo.DownloadLink)]

// Adds some basic information to the file
[assembly: AssemblyTitle(NEP.MagPerception.BuildInfo.Name)]
[assembly: AssemblyProduct(NEP.MagPerception.BuildInfo.Name)]
[assembly: AssemblyVersion(NEP.MagPerception.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NEP.MagPerception.BuildInfo.Version)]
[assembly: AssemblyInformationalVersion(NEP.MagPerception.BuildInfo.Version)]
[assembly: AssemblyCompany(NEP.MagPerception.BuildInfo.Company)]

// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
// The mod references the LabFusion assembly and uses some of its methods to prevent the MagazineUI from switching to a magazine or gun that's not being hold by the local player
[assembly: MelonOptionalDependencies("LabFusion")]