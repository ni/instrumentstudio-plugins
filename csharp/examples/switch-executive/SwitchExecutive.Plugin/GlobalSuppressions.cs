// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Correctness", "LRT001:AllTypesAreInNationalInstrumentsNamespace", Justification = "Public example code", Scope = "module")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Match IVI constants", Scope = "type", Target = "~T:SwitchExecutive.Plugin.Internal.DriverOperations.Internal.IVIConstants")]
[assembly: SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Constants used in XAML", Scope = "type", Target = "~T:SwitchExecutive.Plugin.Internal.Constants")]
