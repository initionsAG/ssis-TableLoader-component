using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
[assembly: AssemblyTitle("TableLoader 3")]
#if     (SQL2008)
[assembly: AssemblyDescription("for Integration Services 2008")]
#elif   (SQL2012)
[assembly: AssemblyDescription("for Integration Services 2012")]
#elif   (SQL2014)
[assembly: AssemblyDescription("for Integration Services 2014")]
#else
[assembly: AssemblyDescription("for Integration Services 2008")]
#endif
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("innovative IT solutions AG")]
[assembly: AssemblyProduct("TableLoader 3")]
[assembly: AssemblyCopyright("Copyright © initions AG 2011-2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten. Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird

// Set FileVersion and typelib GUID

#if     (Sql2008)
[assembly: Guid("2a197ffb-cff6-429e-be7d-f64fc64a9c7e")]
#elif   (SQL2012)
[assembly: Guid("70041C53-DF1C-4CE8-B4B8-FF8931857890")]
#elif   (SQL2014)
[assembly: Guid("B2CF245C-70BE-42F8-A0F9-E6A35C353D98")]
#else
[assembly: Guid("2a197ffb-cff6-429e-be7d-f64fc64a9c7e")]
#endif

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("3.2.27.1")]
